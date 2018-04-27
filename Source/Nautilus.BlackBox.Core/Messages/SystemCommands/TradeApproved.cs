// -------------------------------------------------------------------------------------------------
// <copyright file="TradeApproved.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Messages.SystemCommands
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Base;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="TradeApproved"/> class.
    /// </summary>
    [Immutable]
    public sealed class TradeApproved : DocumentMessage
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
        /// The messages symbol.
        /// </summary>
        public Symbol Symbol => this.OrderPacket.Symbol;

        /// <summary>
        /// Gets the messages order packet.
        /// </summary>
        public AtomicOrderPacket OrderPacket { get; }

        /// <summary>
        /// Gets the messages bars valid for entry.
        /// </summary>
        public int BarsValid { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="TradeApproved"/> service message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(TradeApproved);
    }
}