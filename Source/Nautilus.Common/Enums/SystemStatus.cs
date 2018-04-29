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
    public enum SystemStatus
    {
        /// <summary>
        /// The system is running normally.
        /// </summary>
        OK,

        /// <summary>
        /// The system is suspended (has not failed).
        /// </summary>
        Suspended,

        /// <summary>
        /// The system has failed and will not process further work.
        /// </summary>
        Failure,
    }
}
