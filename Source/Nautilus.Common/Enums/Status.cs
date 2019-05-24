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
    /// Represents the status of a service component or module.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// The component or module is running normally.
        /// </summary>
        Running = 0,

        /// <summary>
        /// The component or module has gracefully stopped.
        /// </summary>
        Stopped = 1,

        /// <summary>
        /// The component or module is temporarily suspended (has not failed).
        /// </summary>
        Suspended = 2,

        /// <summary>
        /// The component or module has failed and will not process further work.
        /// </summary>
        Failed = 3,
    }
}
