//--------------------------------------------------------------------------------------------------
// <copyright file="Status.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Enums
{
    /// <summary>
    /// Represents the status of the system or system component.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// The system or component is running normally.
        /// </summary>
        Running = 0,

        /// <summary>
        /// The system or component has gracefully stopped.
        /// </summary>
        Stopped = 1,

        /// <summary>
        /// The system or component is temporarily suspended (has not failed).
        /// </summary>
        Suspended = 2,

        /// <summary>
        /// The system or component has failed and will not process further work.
        /// </summary>
        Failed = 3,
    }
}
