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
    using System.Reflection.Metadata;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Core;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Provides a generic message bus.
    /// </summary>
    /// <typeparam name="T">The message bus type.</typeparam>
    public sealed class MessageBus<T> : Component
        where T : Message
    {
        private readonly List<object> deadLetters;
        private readonly Dictionary<Type, List<IEndpoint>> subscriptions;

        private Switchboard switchboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBus{T}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        public MessageBus(IComponentryContainer container)
        : base(container)
        {
            this.deadLetters = new List<object>();
            this.subscriptions = new Dictionary<Type, List<IEndpoint>>();
            this.switchboard = Switchboard.Empty();

            this.RegisterHandler<InitializeSwitchboard>(this.OnMessage);
            this.RegisterHandler<ISubscribe>(this.OnMessage);
            this.RegisterHandler<IUnsubscribe>(this.OnMessage);
            this.RegisterHandler<Envelope<T>>(this.OnEnvelope);
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

        private void OnMessage(ISubscribe message)
        {
            var type = message.SubscriptionType;
            var subscriber = message.Subscriber;

            if (!this.subscriptions.ContainsKey(type))
            {
                this.subscriptions[type] = new List<IEndpoint>();
            }

            if (this.subscriptions[type].Contains(subscriber))
            {
                this.Log.Warning($"{subscriber} is already subscribed to {type}.");
                return;
            }

            this.subscriptions[type].Append(subscriber);
        }

        private void OnMessage(IUnsubscribe message)
        {
            var type = message.SubscriptionType;
            var subscriber = message.Subscriber;

            if (!this.subscriptions.ContainsKey(type))
            {
                this.Log.Warning($"{subscriber} is already unsubscribed from {type} messages.");
                return;
            }

            if (!this.subscriptions[type].Contains(subscriber))
            {
                this.Log.Warning($"{subscriber} is already unsubscribed from {type} messages.");
                return;
            }

            this.subscriptions[type].Remove(subscriber);
        }

        private void OnEnvelope(Envelope<T> envelope)
        {
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

        private void Publish(Envelope<T> envelope)
        {
            if (!this.subscriptions.ContainsKey(envelope.MessageType))
            {
                // No subscribers for this message type.
                return;
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
