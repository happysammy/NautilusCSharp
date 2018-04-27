// -------------------------------------------------------------------------------------------------
// <copyright file="BarTimeFrame.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The bars time frame.
    /// </summary>
    public enum BarTimeFrame
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