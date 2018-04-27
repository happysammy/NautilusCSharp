// -------------------------------------------------------------------------------------------------
// <copyright file="CandleBody.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Indicators.Enums
{
    /// <summary>
    /// The candle body size.
    /// </summary>
    public enum CandleBody
    {
        /// <summary>
        /// No candle body (doji).
        /// </summary>
        None = 0,

        /// <summary>
        /// A very small candle body.
        /// </summary>
        Small = 1,

        /// <summary>
        /// A medium candle body.
        /// </summary>
        Medium = 3,

        /// <summary>
        /// A large candle body.
        /// </summary>
        Large = 4,

        /// <summary>
        /// A trend type candle body.
        /// </summary>
        Trend = 5
    }
}