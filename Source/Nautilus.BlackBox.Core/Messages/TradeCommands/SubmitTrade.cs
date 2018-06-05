//--------------------------------------------------------------------------------------------------
// <copyright file="SubmitTrade.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Messages.TradeCommands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Entities;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="SubmitTrade"/> class.
    /// </summary>
    [Immutable]
    public sealed class SubmitTrade : CommandMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitTrade"/> class.
        /// </summary>
        /// <param name="orderPacket">The message order packet.</param>
        /// <param name="minStopDistanceEntry">The message minimum stop distance (cannot be negative).</param>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public SubmitTrade(
            AtomicOrderPacket orderPacket,
            decimal minStopDistanceEntry,
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Validate.NotNull(orderPacket, nameof(orderPacket));
            Validate.DecimalNotOutOfRange(minStopDistanceEntry, nameof(minStopDistanceEntry), decimal.Zero, decimal.MaxValue);
            Validate.NotDefault(messageId, nameof(messageId));
            Validate.NotDefault(messageTimestamp, nameof(messageTimestamp));

            this.OrderPacket = orderPacket;
            this.MinStopDistanceEntry = minStopDistanceEntry;
        }

        /// <summary>
        /// Gets the messages atomic order packet.
        /// </summary>
        public AtomicOrderPacket OrderPacket { get; }

        /// <summary>
        /// Gets the messages minimum stop distance for entry.
        /// </summary>
        public decimal MinStopDistanceEntry { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="SubmitTrade"/> command message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(SubmitTrade)}-{this.OrderPacket.OrderPacketId}";
    }
}
