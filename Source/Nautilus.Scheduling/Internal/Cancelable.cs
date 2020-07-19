// -------------------------------------------------------------------------------------------------
// <copyright file="Cancelable.cs" company="Nautech Systems Pty Ltd">
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
using System.Threading;
using Nautilus.Core.Correctness;

namespace Nautilus.Scheduling.Internal
{
    /// <summary>
    /// A <see cref="ICancelable"/> that wraps a <see cref="CancellationTokenSource"/>.
    /// When canceling this instance the underlying <see cref="CancellationTokenSource"/> is canceled as well.
    /// </summary>
    internal sealed class Cancelable : ICancelable, IDisposable
    {
        private readonly IActionScheduler scheduler;
        private readonly CancellationTokenSource source;

        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cancelable"/> class that will be cancelled
        /// after the specified amount of time.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="delay">The delay before the cancelable is canceled.</param>
        internal Cancelable(IActionScheduler scheduler, TimeSpan delay)
            : this(scheduler)
        {
            this.CancelAfter(delay);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cancelable"/> class that will be cancelled
        /// after the specified amount of time.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="delay">The delay before the cancelable is canceled.</param>
        internal Cancelable(IScheduler scheduler, TimeSpan delay)
            : this(scheduler)
        {
            this.CancelAfter(delay);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cancelable"/> class that will be cancelled
        /// after the specified amount of milliseconds.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="millisecondsDelay">The delay in milliseconds.</param>
        internal Cancelable(IScheduler scheduler, int millisecondsDelay)
            : this(scheduler)
        {
            this.CancelAfter(millisecondsDelay);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cancelable"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        internal Cancelable(IActionScheduler scheduler)
            : this(scheduler, new CancellationTokenSource())
        {
            // Intentionally left blank.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cancelable"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="source">The cancellation source.</param>
        private Cancelable(IActionScheduler scheduler, CancellationTokenSource source)
        {
            this.source = source;
            this.scheduler = scheduler;
        }

        /// <summary>
        /// Gets a value indicating whether cancellation has been requested.
        /// </summary>
        public bool IsCancellationRequested => this.source.IsCancellationRequested;

        /// <summary>
        /// Gets the cancellation token.
        /// </summary>
        public CancellationToken Token => this.source.Token;

        /// <inheritdoc />
        public void Cancel()
        {
            this.Cancel(false);
        }

        /// <summary>
        /// Communicates a request for cancellation, and specifies whether remaining callbacks and
        /// cancelable operations should be processed.
        /// </summary>
        /// <param name="throwOnFirstException"><c>true</c> if exceptions should immediately propagate; otherwise, <c>false</c>.</param>
        /// <remarks>
        /// The associated cancelable will be notified of the cancellation and will transition to a state where
        /// <see cref="IsCancellationRequested" /> returns <c>true</c>.
        /// Any callbacks or cancelable operations registered with the cancelable will be executed.
        /// Cancelable operations and callbacks registered with the token should not throw exceptions.
        /// If <paramref name="throwOnFirstException" /> is <c>true</c>, an exception will immediately propagate out of
        /// the call to Cancel, preventing the remaining callbacks and cancelable operations from being processed.
        /// If <paramref name="throwOnFirstException" /> is <c>false</c>, this overload will aggregate any exceptions
        /// thrown into an <see cref="AggregateException" />, such that one callback throwing an exception will not
        /// prevent other registered callbacks from being executed.
        /// The <see cref="ExecutionContext" /> that was captured when each callback was registered will be reestablished when the callback is invoked.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">
        /// This exception is thrown if this cancelable has already been disposed.
        /// </exception>
        public void Cancel(bool throwOnFirstException)
        {
            this.ThrowIfDisposed();
            this.source.Cancel(throwOnFirstException);
        }

        /// <summary>
        /// Schedules a cancel operation on this cancelable after the specified delay.
        /// </summary>
        /// <param name="delay">The delay before this instance is canceled.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="delay"/> is negative.</exception>
        /// <exception cref="ObjectDisposedException">This exception is thrown if this cancelable
        /// has already been disposed.</exception>
        public void CancelAfter(TimeSpan delay)
        {
            Condition.NotNegativeInt32(delay.Milliseconds, nameof(delay));

            this.InternalCancelAfter(delay);
        }

        /// <summary>
        /// Schedules a cancel operation on this cancelable after the specified number of milliseconds.
        /// </summary>
        /// <param name="millisecondsDelay">The delay in milliseconds before this instance is canceled.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="millisecondsDelay"/> is negative.</exception>
        /// <exception cref="ObjectDisposedException">If this cancelable has already been disposed.</exception>
        public void CancelAfter(int millisecondsDelay)
        {
            Condition.NotNegativeInt32(millisecondsDelay, nameof(millisecondsDelay));

            this.InternalCancelAfter(TimeSpan.FromMilliseconds(millisecondsDelay));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <param name="disposing">if set to <c>true</c> the method has been called directly or indirectly by a
        /// user's code. Managed and unmanaged resources will be disposed.<br />
        /// if set to <c>false</c> the method has been called by the runtime from inside the finalizer and only
        /// unmanaged resources can be disposed.</param>
        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    // Clean up managed resources.
                    this.source.Dispose();
                }

                // Clean up unmanaged resources.
            }

            this.isDisposed = true;
        }

        private void InternalCancelAfter(TimeSpan delay)
        {
            this.ThrowIfDisposed();
            if (this.source.IsCancellationRequested)
            {
                return;
            }

            this.source.CancelAfter(delay);
        }

        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(nameof(HashedWheelTimerScheduler), "The cancelable has already been disposed");
            }
        }
    }
}
