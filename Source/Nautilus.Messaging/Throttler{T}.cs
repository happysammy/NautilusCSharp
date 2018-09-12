// -------------------------------------------------------------------------------------------------
// <copyright file="Throttler{T}.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Factories;
    using NodaTime;

    /// <summary>
    /// Provides a message throttling component.
    /// </summary>
    /// <typeparam name="T">The message type to throttle.</typeparam>
    [PerformanceOptimized]
    public sealed class Throttler<T> : ActorComponentBase
    {
        private const string TimerReset = "TR";

        private readonly IEndpoint receiver;
        private readonly TimeSpan interval;
        private readonly int limit;
        private readonly Queue<T> queue;
        private readonly ZonedDateTime startTime;

        private bool isIdle;
        private int vouchers;
        private int totalCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="Throttler{T}"/> class.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="container">The setup container.</param>
        /// <param name="receiver">The receivers endpoint.</param>
        /// <param name="interval">The throttle timer interval.</param>
        /// <param name="limit">The message limit per second.</param>
        public Throttler(
            NautilusService serviceContext,
            IComponentryContainer container,
            IEndpoint receiver,
            Duration interval,
            int limit)
            : base(
                serviceContext,
                LabelFactory.Component(nameof(Throttler<T>)),
                container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(receiver, nameof(receiver));
            Validate.NotDefault(interval, nameof(interval));
            Validate.PositiveInt32(limit, nameof(limit));

            this.receiver = receiver;
            this.interval = interval.ToTimeSpan();
            this.limit = limit;
            this.queue = new Queue<T>();
            this.startTime = this.TimeNow();

            this.isIdle = true;
            this.vouchers = limit;
            this.totalCount = 0;

            // Setup message handling.
            this.Receive<T>(msg => this.OnMessage(msg));
            this.Receive<TimeSpan>(msg => this.OnMessage(msg));
        }

        private void OnMessage(T message)
        {
            Debug.NotNull(message, nameof(message));

            this.queue.Enqueue(message);

            this.totalCount++;
            this.ProcessQueue();
        }

        private void OnMessage(TimeSpan message)
        {
            Debug.NotNull(message, nameof(message));

            this.vouchers = this.limit;

            if (this.queue.Count > 0)
            {
                this.isIdle = false;
                this.RunTimer().PipeTo(this.Self);
                this.ProcessQueue();
            }
            else
            {
                this.Log.Debug("Is Idle.");
            }
        }

        private void ProcessQueue()
        {
            if (this.isIdle)
            {
                this.Log.Debug("Is Active.");

                this.isIdle = false;
                this.RunTimer().PipeTo(this.Self);
            }

            if (this.vouchers <= 0)
            {
                // At message limit.
                this.Log.Debug($"At message limit of {this.limit} per {this.interval} (queueing_count={this.queue.Count}).");
                return;
            }

            while (this.vouchers > 0 & this.queue.Count > 0)
            {
                var message = this.queue.Dequeue();
                this.receiver.Send(message);
                this.Log.Debug($"Sent message {message} (total_count={this.totalCount}).");
                this.vouchers--;
            }
        }

        private Task<TimeSpan> RunTimer()
        {
            Task.Delay(this.interval).Wait();

            return Task.FromResult(this.interval);
        }
    }
}
