//--------------------------------------------------------------------------------------------------
// <copyright file="CandleDirection.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Indicators.Enums
{
    /// <summary>
    /// The candle direction.
    /// </summary>
    public enum CandleDirection
    {
        /// <summary>
        /// No candle direction.
        /// </summary>
        None = 0,

        /// <summary>
        /// A bull candle (up close).
        /// </summary>
        Bull = 1,

        /// <summary>
        /// A bear candle (down close).
        /// </summary>
        Bear = 2,
    }
}