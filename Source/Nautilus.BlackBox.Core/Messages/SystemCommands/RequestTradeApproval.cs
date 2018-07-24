//--------------------------------------------------------------------------------------------------
// <copyright file="RequestTradeApproval.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Messages.SystemCommands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.DomainModel.Entities;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="RequestTradeApproval"/> class.
    /// </summary>
    [Immutable]
    public sealed class RequestTradeApproval : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestTradeApproval"/> class.
        /// </summary>
        /// <param name="orderPacket">The message order packet.</param>
        /// <param name="signal">The message entry signal.</param>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public RequestTradeApproval(
            AtomicOrderPacket orderPacket,
            EntrySignal signal,
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Validate.NotNull(orderPacket, nameof(orderPacket));
            Validate.NotNull(signal, nameof(signal));
            Validate.NotDefault(messageId, nameof(messageId));
            Validate.NotDefault(messageTimestamp, nameof(messageTimestamp));

            this.OrderPacket = orderPacket;
            this.Signal = signal;
        }

        /// <summary>
        /// Gets the messages order packet.
        /// </summary>
        public AtomicOrderPacket OrderPacket { get; }

        /// <summary>
        /// Gets the messages symbol.
        /// </summary>
        public EntrySignal Signal { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="RequestTradeApproval"/>
        /// service message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{base.ToString()}-{this.Signal}";
    }
}
