//--------------------------------------------------------------
// <copyright file="CandleSize.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Indicators.Enums
{
    /// <summary>
    /// The candle size.
    /// </summary>
    public enum CandleSize
    {
        /// <summary>
        /// No candle size (doji).
        /// </summary>
        None = 0,

        /// <summary>
        /// A very small candle size.
        /// </summary>
        VerySmall = 1,

        /// <summary>
        /// A small candle size.
        /// </summary>
        Small = 2,

        /// <summary>
        /// A medium candle size.
        /// </summary>
        Medium = 3,

        /// <summary>
        /// A large candle size.
        /// </summary>
        Large = 4,

        /// <summary>
        /// A very large candle size.
        /// </summary>
        VeryLarge = 5,

        /// <summary>
        /// An extremely large candle size.
        /// </summary>
        ExtremelyLarge = 6
    }
}