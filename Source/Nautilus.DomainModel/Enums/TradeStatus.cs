// -------------------------------------------------------------------------------------------------
// <copyright file="TradeStatus.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The trade status.
    /// </summary>
    public enum TradeStatus
    {
        /// <summary>
        /// The trade status cannot be determined.
        /// </summary>
        Unknown,

        /// <summary>
        /// The trade has been initialized.
        /// </summary>
        Initialized,

        /// <summary>
        /// The trade has pending entry orders.
        /// </summary>
        Pending,

        /// <summary>
        /// The trade is active in the market.
        /// </summary>
        Active,

        /// <summary>
        /// The trade has been completed.
        /// </summary>
        Completed
    }
}