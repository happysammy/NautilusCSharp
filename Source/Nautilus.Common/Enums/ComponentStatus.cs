//--------------------------------------------------------------------------------------------------
// <copyright file="SystemStatus.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Enums
{
    /// <summary>
    /// The system status enumeration.
    /// </summary>
    public enum ComponentStatus
    {
        /// <summary>
        /// The component is running normally.
        /// </summary>
        OK = 0,

        /// <summary>
        /// The component is suspended (has not failed).
        /// </summary>
        Suspended = 1,

        /// <summary>
        /// The component has failed and will not process further work.
        /// </summary>
        Failed = 2,
    }
}
