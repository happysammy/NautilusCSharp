//--------------------------------------------------------------------------------------------------
// <copyright file="BarResolution.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The time period resolution used for a trade bar.
    /// </summary>
    public enum BarResolution
    {
        /// <summary>
        /// The tick timeframe.
        /// </summary>
        Tick = 0,

        /// <summary>
        /// The second timeframe.
        /// </summary>
        Second = 1,

        /// <summary>
        /// The minute timeframe.
        /// </summary>
        Minute = 2,

        /// <summary>
        /// The hourly timeframe.
        /// </summary>
        Hour = 3,

        /// <summary>
        /// The daily timeframe.
        /// </summary>
        Day = 4,

        /// <summary>
        /// The weekly timeframe.
        /// </summary>
        Week = 5,

        /// <summary>
        /// The monthly timeframe.
        /// </summary>
        Month = 6
    }
}
