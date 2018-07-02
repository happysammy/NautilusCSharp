//--------------------------------------------------------------------------------------------------
// <copyright file="IJobEvent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Scheduler.Events
{
    using Quartz;

    /// <summary>
    /// The base interface for all job events.
    /// </summary>
    internal interface IJobEvent
    {
        /// <summary>
        /// Gets the job key.
        /// </summary>
        JobKey JobKey { get; }

        /// <summary>
        /// Gets the trigger key.
        /// </summary>
        TriggerKey TriggerKey { get; }
    }
}
