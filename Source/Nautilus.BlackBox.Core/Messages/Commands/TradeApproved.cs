//--------------------------------------------------------------------------------------------------
// <copyright file="TradeApproved.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Messages.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a command which approves the contained trade base on the risk model.
    /// </summary>
    [Immutable]
    public sealed class TradeApproved : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TradeApproved"/> class.
        /// </summary>
        /// <param name="orderPacket">The message order packet.</param>
        /// <param name="barsValid">The message bars valid (cannot be zero or negative).</param>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public TradeApproved(
            AtomicOrderPacket orderPacket,
            int barsValid,
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Validate.NotNull(orderPacket, nameof(orderPacket));
            Validate.Int32NotOutOfRange(barsValid, nameof(barsValid), 0, int.MaxValue, RangeEndPoints.Exclusive);
            Validate.NotDefault(messageId, nameof(messageId));
            Validate.NotDefault(messageTimestamp, nameof(messageTimestamp));

            this.OrderPacket = orderPacket;
            this.BarsValid = barsValid;
        }

        /// <summary>
        /// Gets the commands symbol.
        /// </summary>
        public Symbol Symbol => this.OrderPacket.Symbol;

        /// <summary>
        /// Gets the commands order packet.
        /// </summary>
        public AtomicOrderPacket OrderPacket { get; }

        /// <summary>
        /// Gets the commands bars valid for entry.
        /// </summary>
        public int BarsValid { get; }
    }
}
