//--------------------------------------------------------------------------------------------------
// <copyright file="SubmitTrade.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Commands
{
    using System;
    using Nautilus.Common.Commands.Base;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;

    /// <summary>
    /// Represents a command to submit a trade comprising of a packet of atomic orders.
    /// </summary>
    [Immutable]
    public sealed class SubmitTrade : TradeCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitTrade"/> class.
        /// </summary>
        /// <param name="orderPacket">The commands order packet.</param>
        /// <param name="minStopDistanceEntry">The commands minimum stop distance (cannot be negative).</param>
        /// <param name="commandId">The commands identifier (cannot be default).</param>
        /// <param name="commandTimestamp">The commands timestamp (cannot be default).</param>
        public SubmitTrade(
            AtomicOrderPacket orderPacket,
            decimal minStopDistanceEntry,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                orderPacket.Symbol,
                new TradeId(orderPacket.Id.Value),
                commandId,
                commandTimestamp)
        {
            Debug.NotNull(orderPacket, nameof(orderPacket));
            Debug.DecimalNotOutOfRange(minStopDistanceEntry, nameof(minStopDistanceEntry), decimal.Zero, decimal.MaxValue);
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.OrderPacket = orderPacket;
            this.MinStopDistanceEntry = minStopDistanceEntry;
        }

        /// <summary>
        /// Gets the commands atomic order packet.
        /// </summary>
        public AtomicOrderPacket OrderPacket { get; }

        /// <summary>
        /// Gets the commands minimum stop distance for entry.
        /// </summary>
        public decimal MinStopDistanceEntry { get; }
    }
}
