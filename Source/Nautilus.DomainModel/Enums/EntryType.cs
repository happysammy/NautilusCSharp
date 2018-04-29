//--------------------------------------------------------------------------------------------------
// <copyright file="EntryType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

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