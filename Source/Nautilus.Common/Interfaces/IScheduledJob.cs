//--------------------------------------------------------------------------------------------------
// <copyright file="IScheduledJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using NodaTime;

    /// <summary>
    /// Represents a job command to execute at a scheduled time.
    /// </summary>
    public interface IScheduledJob
    {
        /// <summary>
        /// Gets the commands scheduled job time.
        /// </summary>
        ZonedDateTime ScheduledTime { get; }
    }
}
