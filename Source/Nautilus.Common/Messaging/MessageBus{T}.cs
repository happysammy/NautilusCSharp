// -------------------------------------------------------------------------------------------------
// <copyright file="MessageBus{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Provides a generic message bus.
    /// </summary>
    /// <typeparam name="T">The message bus type.</typeparam>
    public sealed class MessageBus<T> : Component
        where T : Message
    {
        private readonly Type busType;
        private readonly List<object> deadLetters;
        private readonly List<IEndpoint> subscriptionsAll;
        private readonly Dictionary<Type, List<IEndpoint>> subscriptions;

        private Switchboard switchboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBus{T}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        public MessageBus(IComponentryContainer container)
        : base(container)
        {
            this.busType = typeof(T);
            this.deadLetters = new List<object>();
            this.subscriptionsAll = new List<IEndpoint>();
            this.subscriptions = new Dictionary<Type, List<IEndpoint>>();

            this.switchboard = Switchboard.Empty();

            this.RegisterHandler<InitializeSwitchboard>(this.OnMessage);
            this.RegisterHandler<Subscribe<Type>>(this.OnMessage);
            this.RegisterHandler<Unsubscribe<Type>>(this.OnMessage);
            this.RegisterHandler<IEnvelope>(this.OnEnvelope);
        }

        /// <summary>
        /// Gets the list of dead letters (unhandled messages).
        /// </summary>
        public IEnumerable<object> DeadLetters => this.deadLetters;

        /// <inheritdoc />
        protected override void OnStop(Stop message)
        {
            foreach (var letter in this.deadLetters)
            {
                this.Log.Warning($"[DEAD LETTER] {letter}.");
            }
        }

        private void OnMessage(InitializeSwitchboard message)
        {
            this.switchboard = message.Switchboard;
            this.switchboard.RegisterDeadLetterChannel(this.AddToDeadLetters);

            this.Log.Information("Switchboard initialized.");
        }

        private void OnMessage(Subscribe<Type> message)
        {
            var type = message.SubscriptionType;

            if (message.Subscription == this.busType)
            {
                this.Subscribe(message, this.subscriptionsAll);
                return;
            }

            if (!this.subscriptions.ContainsKey(type))
            {
                this.subscriptions[type] = new List<IEndpoint>();
            }

            this.Subscribe(message, this.subscriptions[type]);
        }

        private void OnMessage(Unsubscribe<Type> message)
        {
            var type = message.SubscriptionType;

            if (message.Subscription == this.busType)
            {
                this.Unsubscribe(message, this.subscriptionsAll);
                return;
            }

            this.Unsubscribe(message, this.subscriptions[type]);
        }

        private void Subscribe(Subscribe<Type> command, List<IEndpoint> subscribers)
        {
            if (subscribers.Contains(command.Subscriber))
            {
                this.Log.Warning($"Subscriber is already subscribed to {command.SubscriptionName} messages.");
                return;
            }

            subscribers.Append(command.Subscriber);

            this.Log.Information($"Subscriber subscribed to {command.SubscriptionName} messages.");
        }

        private void Unsubscribe(Unsubscribe<Type> command, List<IEndpoint> subscribers)
        {
            if (!subscribers.Contains(command.Subscriber))
            {
                this.Log.Warning($"Subscriber is not subscribed to {command.SubscriptionName} messages.");
                return;
            }

            subscribers.Remove(command.Subscriber);

            this.Log.Information($"Subscriber unsubscribed from {command.SubscriptionName} messages.");
        }

        private void OnEnvelope(IEnvelope envelope)
        {
            Debug.True(envelope.MessageBase is T, nameof(envelope.MessageBase)); // Design time error

            if (envelope.Receiver is null)
            {
                // Publish to subscribers
                this.Publish(envelope);
                return;
            }

            // Send point-to-point
            this.switchboard.SendToReceiver(envelope);

            this.Log.Verbose($"[{this.ProcessedCount}] {envelope.Sender} -> {envelope} -> {envelope.Receiver}");
        }

        private void Publish(IEnvelope envelope)
        {
            if (this.subscriptionsAll.Count > 0 && envelope.MessageType == this.busType)
            {
                for (var i = 0; i < this.subscriptionsAll.Count; i++)
                {
                    this.subscriptionsAll[i].Send(envelope);

                    this.Log.Verbose(
                        $"[{this.ProcessedCount}] {envelope.Sender} -> {envelope} -> PUBLISHED");
                }

                return;
            }

            if (this.subscriptions[envelope.MessageType].Count == 0)
            {
                return; // No subscribers
            }

            for (var i = 0; i < this.subscriptions[envelope.MessageType].Count; i++)
            {
                this.subscriptions[envelope.MessageType][i].Send(envelope);

                this.Log.Verbose(
                    $"[{this.ProcessedCount}] {envelope.Sender} -> {envelope} -> {envelope.Receiver}");
            }
        }

        private void AddToDeadLetters(object message)
        {
            this.deadLetters.Add(message);

            this.Log.Error($"Undeliverable message [{message}].");
        }
    }
}
