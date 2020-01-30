//--------------------------------------------------------------------------------------------------
// <copyright file="PriceType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents a price type.
    /// </summary>
    public enum PriceType
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The price is based on the bid.
        /// </summary>
        Bid = 1,

        /// <summary>
        /// The price is based on the ask.
        /// </summary>
        Ask = 2,

        /// <summary>
        /// The price is based on the mid point between bid and ask.
        /// </summary>
        Mid = 3,

        /// <summary>
        /// The price is based on the last quote change.
        /// </summary>
        Last = 4,
    }
}
