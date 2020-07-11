// -------------------------------------------------------------------------------------------------
// <copyright file="Throttler{T}.cs" company="Nautech Systems Pty Ltd">
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
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nautilus.Common.Componentry;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Messages.Commands;
using Nautilus.Core.Correctness;
using NodaTime;

namespace Nautilus.Common.Messaging
{
    /// <summary>
    /// Provides a generic object throttler.
    /// </summary>
    /// <typeparam name="T">The throttled object type.</typeparam>
    public sealed class Throttler<T> : MessagingComponent
    {
        private readonly Action<T> receiver;
        private readonly TimeSpan interval;
        private readonly RefreshVouchers refresh;
        private readonly int limit;
        private readonly Queue<T> queue;

        private int vouchers;
        private int totalCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="Throttler{T}"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="receiver">The throttled object receiver.</param>
        /// <param name="intervalDuration">The throttle timer interval.</param>
        /// <param name="limit">The message limit per interval.</param>
        /// <param name="subName">The sub-name for the throttler.</param>
        public Throttler(
            IComponentryContainer container,
            Action<T> receiver,
            Duration intervalDuration,
            int limit,
            string subName)
            : base(container, subName)
        {
            Condition.PositiveInt32((int)intervalDuration.TotalMilliseconds, nameof(intervalDuration.TotalMilliseconds));
            Condition.PositiveInt32(limit, nameof(limit));

            this.receiver = receiver;
            this.interval = intervalDuration.ToTimeSpan();
            this.refresh = new RefreshVouchers();
            this.limit = limit;
            this.queue = new Queue<T>();

            this.IsIdle = true;
            this.vouchers = limit;
            this.totalCount = 0;

            this.RegisterHandler<RefreshVouchers>(this.OnMessage);
            this.RegisterHandler<T>(this.OnMessage);
        }

        /// <summary>
        /// Gets the queue count for the throttler.
        /// </summary>
        public int QueueCount => this.queue.Count;

        /// <summary>
        /// Gets a value indicating whether the throttler is active.
        /// </summary>
        public bool IsActive => !this.IsIdle;

        /// <summary>
        /// Gets a value indicating whether the throttler is idle.
        /// </summary>
        public bool IsIdle { get; private set; }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
        }

        /// <inheritdoc />
        protected override void OnStop(Stop message)
        {
        }

        private void OnMessage(RefreshVouchers message)
        {
            this.vouchers = this.limit;

            if (this.queue.Count <= 0)
            {
                this.IsIdle = true;
#if DEBUG
                this.Logger.LogTrace("Idle.");
#endif

                return;
            }

            Task.Run(this.RunTimer);

            if (this.IsIdle)
            {
                this.IsIdle = false;
#if DEBUG
                this.Logger.LogTrace("Active.");
#endif
            }

            this.ProcessQueue();
        }

        private void OnMessage(T message)
        {
            this.queue.Enqueue(message);

            this.totalCount++;
            this.ProcessQueue();
        }

        private void ProcessQueue()
        {
            if (this.IsIdle)
            {
                Task.Run(this.RunTimer);
                this.IsIdle = false;
#if DEBUG
                this.Logger.LogTrace("Active.");
#endif
            }

            while (this.vouchers > 0 && this.queue.Count > 0)
            {
                var message = this.queue.Dequeue();

                if (message == null)
                {
                    continue;  // Cannot send a null message
                }

                this.receiver(message);
                this.vouchers--;

#if DEBUG
                this.Logger.LogTrace($"Sent message {message} (total_count={this.totalCount}).");
#endif
            }

            if (this.vouchers <= 0 && this.queue.Count > 0)
            {
                // At message limit
#if DEBUG
                this.Logger.LogTrace($"At message limit of {this.limit} per {this.interval} (queued_count={this.queue.Count}).");
#endif
            }
        }

        private void RunTimer()
        {
            Task.Delay(this.interval).Wait();

            this.SendToSelf(this.refresh);
        }

        private sealed class RefreshVouchers
        {
        }
    }
}
