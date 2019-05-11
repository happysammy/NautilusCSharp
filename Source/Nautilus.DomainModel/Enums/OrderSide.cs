//--------------------------------------------------------------------------------------------------
// <copyright file="OrderSide.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Aggregates;

    /// <summary>
    /// Represents the execution direction of an <see cref="Order"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Enums can be capitalized.")]
    public enum OrderSide
    {
        /// <summary>
        /// An undefined order side.
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// A buy order side.
        /// </summary>
        BUY = 1,

        /// <summary>
        /// A sell order side.
        /// </summary>
        SELL = 2,
    }
}
