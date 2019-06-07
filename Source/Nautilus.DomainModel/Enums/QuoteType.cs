//--------------------------------------------------------------------------------------------------
// <copyright file="QuoteType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// Represents a price quote type.
    /// </summary>
    public enum QuoteType
    {
        /// <summary>
        /// The bid price quote type.
        /// </summary>
        BID = 0,

        /// <summary>
        /// The ask price quote type.
        /// </summary>
        ASK = 1,

        /// <summary>
        /// The mid price quote type.
        /// </summary>
        MID = 2,

        /// <summary>
        /// The last price quote type.
        /// </summary>
        LAST = 3,
    }
}
