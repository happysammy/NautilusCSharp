//--------------------------------------------------------------------------------------------------
// <copyright file="Resolution.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
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
        /// The tick resolution value.
        /// </summary>
        TICK = 0,

        /// <summary>
        /// The second resolution value.
        /// </summary>
        SECOND = 1,

        /// <summary>
        /// The minute resolution value.
        /// </summary>
        MINUTE = 2,

        /// <summary>
        /// The hourly resolution value.
        /// </summary>
        HOUR = 3,

        /// <summary>
        /// The daily resolution value.
        /// </summary>
        DAY = 4,
    }
}
