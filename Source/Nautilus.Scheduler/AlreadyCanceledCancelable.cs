// -------------------------------------------------------------------------------------------------
// <copyright file="AlreadyCanceledCancelable.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    using System;
    using System.Threading;

    /// <summary>
    /// A <see cref="ICancelable"/> that is already canceled.
    /// </summary>
    public class AlreadyCanceledCancelable : ICancelable
    {
        private static readonly AlreadyCanceledCancelable InternalInstance = new AlreadyCanceledCancelable();

        private AlreadyCanceledCancelable()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static ICancelable Instance => InternalInstance;

        /// <summary>
        /// Gets a value indicating whether cancellation is requested.
        /// </summary>
        public bool IsCancellationRequested => true;

        /// <summary>
        /// Gets the token.
        /// </summary>
        public CancellationToken Token => new CancellationToken(true);

        /// <summary>
        /// TBD.
        /// </summary>
        public void Cancel()
        {
            // Intentionally left blank.
        }

        /// <inheritdoc/>
        void ICancelable.CancelAfter(TimeSpan delay)
        {
            // Intentionally left blank.
        }

        /// <inheritdoc/>
        void ICancelable.CancelAfter(int millisecondsDelay)
        {
            // Intentionally left blank.
        }

        /// <inheritdoc />
        public void Cancel(bool throwOnFirstException)
        {
            // Intentionally left blank.
        }
    }
}
