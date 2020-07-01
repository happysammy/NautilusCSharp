// -------------------------------------------------------------------------------------------------
// <copyright file="DataBus{T}.cs" company="Nautech Systems Pty Ltd">
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
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;
using Nautilus.Common.Componentry;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Messages.Commands;
using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.Messaging;

namespace Nautilus.Common.Data
{
    /// <summary>
    /// Provides a generic data bus.
    /// </summary>
    /// <typeparam name="T">The bus data type.</typeparam>
    public sealed class DataBus<T> : MessagingComponent
        where T : class
    {
        // The BroadcastBlock<T> ensures that the current element is broadcast to any linked targets
        // before allowing the element to be overwritten.
        private readonly CancellationTokenSource cancellationSource = new CancellationTokenSource();
        private readonly BroadcastBlock<object> pipeline;
        private readonly List<Mailbox> subscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBus{T}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        public DataBus(IComponentryContainer container)
        : base(container)
        {
            Condition.HasAttribute<ImmutableAttribute>(typeof(T), nameof(T));

            this.pipeline = new BroadcastBlock<object>(
                CloneData,
                new ExecutionDataflowBlockOptions
            {
                CancellationToken = this.cancellationSource.Token,
                BoundedCapacity = (int)Math.Pow(2, 22),
            });

            this.subscriptions = new List<Mailbox>();

            this.RegisterHandler<Subscribe<Type>>(this.OnMessage);
            this.RegisterHandler<Unsubscribe<Type>>(this.OnMessage);
        }

        /// <summary>
        /// Gets the bus data type.
        /// </summary>
        public Type BusType { get; } = typeof(T);

        /// <summary>
        /// Gets the data bus subscriptions.
        /// </summary>
        public IReadOnlyCollection<Address> Subscriptions => this.subscriptions.Select(m => m.Address).ToList().AsReadOnly();

        /// <summary>
        /// Posts the given data onto the data bus.
        /// </summary>
        /// <param name="data">The data to post.</param>
        public void PostData(T data)
        {
            this.pipeline.Post(data!); // data cannot be null (checked in constructor)
        }

        /// <summary>
        /// Gracefully stops the message processor by waiting for all currently accepted messages to
        /// be processed. Messages received after this command will not be processed.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        public Task<bool> StopData()
        {
            try
            {
                this.pipeline.Complete();
                Task.WhenAll(this.pipeline.Completion).Wait();

                return Task.FromResult(true);
            }
            catch (AggregateException)
            {
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Immediately kills the message processor.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        public Task KillData()
        {
            try
            {
                this.cancellationSource.Cancel();
                Task.WhenAll(this.pipeline.Completion).Wait();
            }
            catch (AggregateException)
            {
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
        }

        private static object CloneData(object data)
        {
            return data;
        }

        private void OnMessage(Subscribe<Type> message)
        {
            var type = message.Subscription;
            if (message.Subscription != this.BusType)
            {
                this.Logger.LogError($"Cannot subscribe to {type.Name} data (only {this.BusType.Name} data).");
                return;
            }

            var subscriber = message.Subscriber;

            if (this.subscriptions.Contains(subscriber))
            {
                this.Logger.LogWarning($"The {subscriber} is already subscribed to {this.BusType.Name} data.");
                return;  // Design time error
            }

            this.pipeline.LinkTo(subscriber.Endpoint.GetLink());
            this.subscriptions.Add(subscriber);
            this.Logger.LogInformation($"Subscribed {subscriber} to {this.BusType.Name} data.");
        }

        private void OnMessage(Unsubscribe<Type> message)
        {
            var type = message.Subscription;
            if (message.Subscription != this.BusType)
            {
                this.Logger.LogError($"Cannot unsubscribe from {type.Name} data (only {this.BusType.Name} data).");
                return;
            }

            var subscriber = message.Subscriber;

            if (!this.subscriptions.Contains(subscriber))
            {
                this.Logger.LogWarning($"The {subscriber} is not subscribed to {this.BusType.Name} data.");
                return;
            }

            // TODO: Currently can't easily 'unlink' a subscriber from the broadcast block
            this.subscriptions.Remove(subscriber);
            this.Logger.LogInformation($"Unsubscribed {subscriber} from {this.BusType.Name} data.");
        }
    }
}
