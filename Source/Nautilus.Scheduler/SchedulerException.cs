// -------------------------------------------------------------------------------------------------
// <copyright file="SchedulerException.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler
{
    using System;

    /// <summary>
    /// An exception that is thrown by the <see cref="IScheduler">Schedule*</see> methods
    /// when scheduling is not possible, e.g. after shutting down the <see cref="IScheduler"/>.
    /// </summary>
    public sealed class SchedulerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SchedulerException(string message)
            : base(message)
        {
        }
    }
}
