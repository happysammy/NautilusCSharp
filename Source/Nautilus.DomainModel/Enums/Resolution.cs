//--------------------------------------------------------------------------------------------------
// <copyright file="Resolution.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// Represents the granularity of a time resolution.
    /// </summary>
    public enum Resolution
    {
        /// <summary>
        /// The tick resolution.
        /// </summary>
        TICK = 0,

        /// <summary>
        /// The second resolution.
        /// </summary>
        SECOND = 1,

        /// <summary>
        /// The minute resolution.
        /// </summary>
        MINUTE = 2,

        /// <summary>
        /// The hourly resolution.
        /// </summary>
        HOUR = 3,

        /// <summary>
        /// The daily resolution.
        /// </summary>
        DAY = 4,
    }
}
