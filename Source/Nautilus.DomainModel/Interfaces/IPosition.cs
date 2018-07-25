//--------------------------------------------------------------------------------------------------
// <copyright file="IPosition.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Interfaces
{
    using Nautilus.Core;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides a read-only interface for market positions.
    /// </summary>
    public interface IPosition
    {
        /// <summary>
        /// Gets the positions symbol.
        /// </summary>
        Symbol Symbol { get; }

        /// <summary>
        /// Gets the positions entry order identifier.
        /// </summary>
        EntityId FromEntryOrderId { get; }

        /// <summary>
        /// Gets the positions quantity.
        /// </summary>
        Quantity Quantity { get; }

        /// <summary>
        /// Returns the positions market position.
        /// </summary>
        MarketPosition MarketPosition { get; }

        /// <summary>
        /// Gets the positions entry time (optional).
        /// </summary>
        Option<ZonedDateTime?> EntryTime { get; }

        /// <summary>
        /// Gets the positions average entry price (optional).
        /// </summary>
        Option<Price> AverageEntryPrice { get; }
    }
}
