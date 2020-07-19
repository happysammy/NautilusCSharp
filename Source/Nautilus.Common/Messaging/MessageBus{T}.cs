// -------------------------------------------------------------------------------------------------
// <copyright file="MessageBus{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nautilus.Common.Componentry;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Messages.Commands;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Core.Types;
using Nautilus.Messaging;
using Nautilus.Messaging.Interfaces;

namespace Nautilus.Common.Messaging
{
    /// <summary>
    /// Provides a generic message bus.
    /// </summary>
    /// <typeparam name="T">The bus message type.</typeparam>
    public sealed class MessageBus<T> : MessagingComponent
        where T : Message
    {
        private readonly List<object> deadLetters;
        private readonly List<Mailbox> subscriptionsAll;
        private readonly Dictionary<Type, List<Mailbox>> subscriptions;

        private Switchboard switchboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBus{T}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        public MessageBus(IComponentryContainer container)
        : base(container)
        {
            this.deadLetters = new List<object>();
            this.subscriptionsAll = new List<Mailbox>();
            this.subscriptions = new Dictionary<Type, List<Mailbox>>();

            this.switchboard = Switchboard.Empty();

            this.RegisterHandler<IEnvelope>(this.OnReceive);
            this.RegisterHandler<InitializeSwitchboard>(this.OnMessage);
            this.RegisterHandler<Subscribe<Type>>(this.OnMessage);
            this.RegisterHandler<Unsubscribe<Type>>(this.OnMessage);
        }

        /// <summary>
        /// Gets the message bus type.
        /// </summary>
        public Type BusType { get; } = typeof(T);

        /// <summary>
        /// Gets the list of dead letters (undelivered messages).
        /// </summary>
        public IReadOnlyCollection<object> DeadLetters => this.deadLetters;

        /// <summary>
        /// Gets the message bus subscriptions.
        /// </summary>
        /// <returns>The subscribers by type dictionary.</returns>
        public IReadOnlyDictionary<Type, IReadOnlyCollection<Address>> Subscriptions => this.BuildSubscribers();

        /// <summary>
        /// Gets the message bus total subscriptions count.
        /// </summary>
        public int SubscriptionCount => this.BuildSubscribers().Select(kvp => kvp.Value.Count).Sum();

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            // No actions to perform
        }

        /// <inheritdoc />
        protected override void OnStop(Stop message)
        {
            foreach (var letter in this.deadLetters)
            {
                this.Logger.LogWarning($"[DEAD LETTER] {letter}.");
            }
        }

        private void OnMessage(InitializeSwitchboard message)
        {
            this.switchboard = message.Switchboard;
            this.switchboard.RegisterDeadLetterChannel(this.AddToDeadLetters);

            this.Logger.LogDebug("Switchboard initialized.");
        }

        private void OnMessage(Subscribe<Type> message)
        {
            var type = message.Subscription;
            if (type == this.BusType)
            {
                this.Subscribe(message, this.subscriptionsAll);
                return;
            }

            if (!type.IsSubclassOf(this.BusType))
            {
                this.Logger.LogError($"Cannot subscribe to {type.Name} type messages " +
                               $"(only all {this.BusType.Name} or type of {this.BusType.Name} messages).");
                return;
            }

            if (!this.subscriptions.ContainsKey(type))
            {
                this.subscriptions[type] = new List<Mailbox>();
            }

            this.Subscribe(message, this.subscriptions[type]);
        }

        private void OnMessage(Unsubscribe<Type> message)
        {
            var type = message.Subscription;
            if (type == this.BusType)
            {
                this.Unsubscribe(message, this.subscriptionsAll);
                return;
            }

            if (!type.IsSubclassOf(this.BusType))
            {
                this.Logger.LogError($"Cannot unsubscribe from {type.Name} type messages " +
                               $"(only all {this.BusType.Name} or type of {this.BusType.Name} messages).");
                return;
            }

            this.Unsubscribe(message, this.subscriptions[type]);

            if (this.subscriptions[type].Count == 0)
            {
                // No longer subscribers for this type
                this.subscriptions.Remove(type);
            }
        }

        private void Subscribe(Subscribe<Type> command, List<Mailbox> subscribers)
        {
            var subscriber = command.Subscriber;
            if (subscribers.Contains(subscriber))
            {
                this.Logger.LogWarning($"The {subscriber} is already subscribed to {command.Subscription.Name} messages.");
                return;
            }

            subscribers.Add(subscriber);

            this.Logger.LogInformation($"Subscribed {subscriber} to {command.Subscription.Name} messages.");
        }

        private void Unsubscribe(Unsubscribe<Type> command, List<Mailbox> subscribers)
        {
            var subscriber = command.Subscriber;
            if (!subscribers.Contains(subscriber))
            {
                this.Logger.LogWarning($"The {subscriber} is not subscribed to {command.Subscription.Name} messages.");
                return;
            }

            subscribers.Remove(subscriber);

            this.Logger.LogInformation($"Unsubscribed {subscriber} from {command.Subscription.Name} messages.");
        }

        private void OnReceive(IEnvelope envelope)
        {
            Debug.True(envelope.Message is T, nameof(envelope.Message)); // Design time error

            if (envelope.Receiver is null)
            {
                // Publish to subscribers
                this.Publish(envelope);
                return;
            }

            // Send point-to-point
            if (this.switchboard.SendToReceiver(envelope))
            {
#if DEBUG
                this.Logger.LogTrace($"Sent[{this.ProcessedCount}] {envelope.Sender} -> {envelope} -> {envelope.Receiver}");
#endif
            }
            else
            {
                this.Logger.LogError($"{envelope.Receiver.Value} address unknown to switchboard.");
                this.Logger.LogError($"Sent[{this.ProcessedCount}] {envelope.Sender} -> {envelope} -> DeadLetters");
            }
        }

        [PerformanceOptimized]
        private void Publish(IEnvelope envelope)
        {
            if (this.subscriptionsAll.Count > 0)
            {
                for (var i = 0; i < this.subscriptionsAll.Count; i++)
                {
                    this.subscriptionsAll[i].Endpoint.Send(envelope);
#if DEBUG
                    this.Logger.LogTrace($"Published[{this.ProcessedCount}] {envelope.Sender} -> {envelope} -> {this.subscriptionsAll[i].Address}");
#endif
                }
            }

            if (this.subscriptions.TryGetValue(envelope.MessageType, out var subscribers) && subscribers.Count > 0)
            {
                for (var i = 0; i < subscribers.Count; i++)
                {
                    subscribers[i].Endpoint.Send(envelope);
#if DEBUG
                    this.Logger.LogTrace($"Published[{this.ProcessedCount}] {envelope.Sender} -> {envelope} -> {subscribers[i].Address}");
#endif
                }
            }
        }

        private void AddToDeadLetters(object message)
        {
            this.deadLetters.Add(message);

            this.Logger.LogError($"Undeliverable message {message}.");
        }

        private IReadOnlyDictionary<Type, IReadOnlyCollection<Address>> BuildSubscribers()
        {
            var dict = new Dictionary<Type, IReadOnlyCollection<Address>>();

            if (this.subscriptionsAll.Count > 0)
            {
                dict.Add(this.BusType, this.subscriptionsAll
                    .Select(s => s.Address)
                    .ToList()
                    .AsReadOnly());
            }

            foreach (var (key, value) in this.subscriptions)
            {
                dict.Add(key, value
                    .Select(m => m.Address)
                    .ToList()
                    .AsReadOnly());
            }

            return dict;
        }
    }
}
