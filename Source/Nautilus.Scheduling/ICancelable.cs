// -------------------------------------------------------------------------------------------------
// <copyright file="ICancelable.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Scheduling
{
    using System;
    using System.Threading;

    /// <summary>
    /// Signifies something that can be canceled.
    /// </summary>
    public interface ICancelable
    {
        /// <summary>
        /// Gets the cancellation token.
        /// </summary>
        CancellationToken Token { get; }

        /// <summary>
        /// Gets a value indicating whether cancellation has been requested.
        /// </summary>
        bool IsCancellationRequested { get; }

        /// <summary>
        /// Communicates a request for cancellation.
        /// </summary>
        /// <remarks>The associated cancelable will be notified of the cancellation and will transition to a state where
        /// <see cref="IsCancellationRequested"/> returns <c>true</c>.
        /// Any callbacks or cancelable operations registered with the cancelable will be executed.
        /// Cancelable operations and callbacks registered with the token should not throw exceptions.
        /// However, this overload of Cancel will aggregate any exceptions thrown into an
        /// <see cref="AggregateException"/>, such that one callback throwing an exception will not
        /// prevent other registered callbacks from being executed.
        /// The <see cref="ExecutionContext"/> that was captured when each callback was registered will
        /// be reestablished when the callback is invoked.</remarks>
        void Cancel();

        /// <summary>
        /// Schedules a cancel operation on this cancelable after the specified delay.
        /// </summary>
        /// <param name="delay">The delay before this instance is canceled.</param>
        void CancelAfter(TimeSpan delay);

        /// <summary>
        /// Schedules a cancel operation on this cancelable after the specified number of milliseconds.
        /// </summary>
        /// <param name="millisecondsDelay">The delay in milliseconds before this instance is canceled.</param>
        void CancelAfter(int millisecondsDelay);

        /// <summary>
        /// Communicates a request for cancellation, and specifies whether remaining callbacks and cancelable operations should be processed.
        /// </summary>
        /// <param name="throwOnFirstException"><c>true</c> if exceptions should immediately propagate; otherwise, <c>false</c>.</param>
        /// <remarks>The associated cancelable will be notified of the cancellation and will transition to a state where
        /// <see cref="IsCancellationRequested"/> returns <c>true</c>.
        /// Any callbacks or cancelable operations registered with the cancelable will be executed.
        /// Cancelable operations and callbacks registered with the token should not throw exceptions.
        /// If <paramref name="throwOnFirstException"/> is <c>true</c>, an exception will immediately propagate out of
        /// the call to Cancel, preventing the remaining callbacks and cancelable operations from being processed.
        /// If <paramref name="throwOnFirstException"/> is <c>false</c>, this overload will aggregate any exceptions
        /// thrown into an <see cref="AggregateException"/>, such that one callback throwing an exception will not
        /// prevent other registered callbacks from being executed.
        /// The <see cref="ExecutionContext"/> that was captured when each callback was registered will be reestablished when the callback is invoked.</remarks>
        void Cancel(bool throwOnFirstException);
    }
}
