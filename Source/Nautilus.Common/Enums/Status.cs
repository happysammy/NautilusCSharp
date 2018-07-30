//--------------------------------------------------------------------------------------------------
// <copyright file="Status.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Enums
{
    /// <summary>
    /// The status enumeration represents the status of the system or system component.
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// The system or component is running normally.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        OK = 0,

        /// <summary>
        /// The system or component is suspended (has not failed).
        /// </summary>
        Suspended = 1,

        /// <summary>
        /// The system or component has failed and will not process further work.
        /// </summary>
        Failed = 2,
    }
}
