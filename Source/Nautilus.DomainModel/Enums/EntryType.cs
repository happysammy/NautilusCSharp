//--------------------------------------------------------------
// <copyright file="EntryType.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The entry type.
    /// </summary>
    public enum EntryType
    {
        /// <summary>
        /// Entry by market order.
        /// </summary>
        Market = 0,

        /// <summary>
        /// Entry by stop market order.
        /// </summary>
        StopMarket = 1
    }
}