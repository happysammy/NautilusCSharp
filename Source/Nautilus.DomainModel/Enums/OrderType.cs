//--------------------------------------------------------------
// <copyright file="OrderType.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The order type.
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// The unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Market order.
        /// </summary>
        Market = 1,

        /// <summary>
        /// Limit order.
        /// </summary>
        Limit = 2,

        /// <summary>
        /// Stop Limit order.
        /// </summary>
        StopLimit = 3,

        /// <summary>
        /// Stop Market order.
        /// </summary>
        StopMarket = 4,

        /// <summary>
        /// Market If Touched order.
        /// </summary>
        MarketIfTouched = 5
    }
}