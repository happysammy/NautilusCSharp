//--------------------------------------------------------------------------------------------------
// <copyright file="ClosePosition.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.Aggregates;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="ClosePosition"/> class.
    /// </summary>
    [Immutable]
    public sealed class ClosePosition : CommandMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClosePosition"/> class.
        /// </summary>
        /// <param name="forTradeUnit">The message for trade unit.</param>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public ClosePosition(
            TradeUnit forTradeUnit,
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Validate.NotNull(forTradeUnit, nameof(forTradeUnit));
            Validate.NotDefault(messageId, nameof(messageId));
            Validate.NotDefault(messageTimestamp, nameof(messageTimestamp));

            this.ForTradeUnit = forTradeUnit;
        }

        /// <summary>
        /// Gets the messages for trade unit.
        /// </summary>
        public TradeUnit ForTradeUnit { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="ClosePosition"/> command message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{base.ToString()}-{this.ForTradeUnit.Symbol}-{this.ForTradeUnit}";
    }
}
