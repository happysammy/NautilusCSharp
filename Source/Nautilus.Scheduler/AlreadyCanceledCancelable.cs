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
        private static readonly AlreadyCanceledCancelable _instance = new AlreadyCanceledCancelable();

        private AlreadyCanceledCancelable() { }

        /// <summary>
        /// TBD
        /// </summary>
        public void Cancel()
        {
            //Intentionally left blank
        }

        /// <summary>
        /// TBD
        /// </summary>
        public bool IsCancellationRequested { get { return true; } }

        /// <summary>
        /// TBD
        /// </summary>
        public static ICancelable Instance { get { return _instance; } }

        /// <summary>
        /// TBD
        /// </summary>
        public CancellationToken Token
        {
            get { return new CancellationToken(true); }
        }

        void ICancelable.CancelAfter(TimeSpan delay)
        {
            //Intentionally left blank
        }

        void ICancelable.CancelAfter(int millisecondsDelay)
        {
            //Intentionally left blank
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="throwOnFirstException">TBD</param>
        public void Cancel(bool throwOnFirstException)
        {
            //Intentionally left blank
        }
    }
}

