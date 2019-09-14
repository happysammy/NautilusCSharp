//--------------------------------------------------------------------------------------------------
// <copyright file="OrderSide.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.Aggregates;

    /// <summary>
    /// Represents the execution direction of an <see cref="Order"/>.
    /// </summary>
    public enum OrderSide
    {
        /// <summary>
        /// The order side is unknown (invalid value).
        /// </summary>
        [InvalidValue]
        UNKNOWN = 0,

        /// <summary>
        /// The buy order side.
        /// </summary>
        BUY = 1,

        /// <summary>
        /// The sell order side.
        /// </summary>
        SELL = 2,
    }
}
