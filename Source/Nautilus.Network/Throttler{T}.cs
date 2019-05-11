// -------------------------------------------------------------------------------------------------
// <copyright file="Throttler{T}.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Messaging.Interfaces;
    using NodaTime;

    /// <summary>
    /// Provides a message throttling component.
    /// </summary>
    /// <typeparam name="T">The message type to throttle.</typeparam>
    [PerformanceOptimized]
    public sealed class Throttler<T> : ComponentBase
    {
        private readonly IEndpoint receiver;
        private readonly TimeSpan interval;
        private readonly int limit;
        private readonly Queue<T> queue;

        private int vouchers;
        private int totalCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="Throttler{T}"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="receiver">The receiver service.</param>
        /// <param name="interval">The throttle timer interval.</param>
        /// <param name="limit">The message limit per second.</param>
        public Throttler(
            IComponentryContainer container,
            NautilusService serviceContext,
            IEndpoint receiver,
            Duration interval,
            int limit)
            : base(serviceContext, container)
        {
            Precondition.NotDefault(interval, nameof(interval));
            Precondition.PositiveInt32(limit, nameof(limit));

            this.receiver = receiver;
            this.interval = interval.ToTimeSpan();
            this.limit = limit;
            this.queue = new Queue<T>();

            this.IsIdle = true;
            this.vouchers = limit;
            this.totalCount = 0;

            this.RegisterHandler<T>(this.OnMessage);
            this.RegisterHandler<TimeSpan>(this.OnMessage);
        }

        /// <summary>
        /// Gets the queue count for the throttler.
        /// </summary>
        public int QueueCount => this.queue.Count;

        /// <summary>
        /// Gets a value indicating whether the throttler is idle.
        /// </summary>
        public bool IsIdle { get; private set; }

        private void OnMessage(T message)
        {
            this.queue.Enqueue(message);

            this.totalCount++;
            this.ProcessQueue();
        }

        private void OnMessage(TimeSpan message)
        {
            this.vouchers = this.limit;

            if (this.queue.Count <= 0)
            {
                this.IsIdle = true;
                this.Log.Debug("Idle.");

                return;
            }

            Task.Run(this.RunTimer);

            if (this.IsIdle)
            {
                this.IsIdle = false;
                this.Log.Debug("Active.");
            }

            this.ProcessQueue();
        }

        private void ProcessQueue()
        {
            if (this.IsIdle)
            {
                Task.Run(this.RunTimer);
                this.IsIdle = false;
                this.Log.Debug("Active.");
            }

            while (this.vouchers > 0 && this.queue.Count > 0)
            {
                var message = this.queue.Dequeue();

                if (message == null)
                {
                    continue;  // Cannot send a null message.
                }

                this.receiver.Send(message);
                this.vouchers--;

                this.Log.Debug($"Sent message {message} (total_count={this.totalCount}).");
            }

            if (this.vouchers <= 0 && this.queue.Count > 0)
            {
                // At message limit.
                this.Log.Debug($"At message limit of {this.limit} per {this.interval} (queued_count={this.queue.Count}).");
            }
        }

        private void RunTimer()
        {
            Task.Delay(this.interval).Wait();

            this.SendToSelf(this.interval);
        }
    }
}
