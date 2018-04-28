//--------------------------------------------------------------
// <copyright file="UnsubscribeSymbolDataType.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Messages.SystemCommands
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="UnsubscribeSymbolDataType"/> class.
    /// </summary>
    [Immutable]
    public sealed class UnsubscribeSymbolDataType : CommandMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsubscribeSymbolDataType"/> class.
        /// </summary>
        /// <param name="symbol">The message symbol.</param>
        /// <param name="tradeType">The message trade type.</param>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public UnsubscribeSymbolDataType(
            Symbol symbol,
            TradeType tradeType,
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotNull(tradeType, nameof(tradeType));
            Validate.NotDefault(messageId, nameof(messageId));
            Validate.NotDefault(messageTimestamp, nameof(messageTimestamp));

            this.Symbol = symbol;
            this.TradeType = tradeType;
        }

        /// <summary>
        /// Gets the messages instrument.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the messages trade type.
        /// </summary>
        public TradeType TradeType { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="UnsubscribeSymbolDataType"/> service
        /// message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(UnsubscribeSymbolDataType);
    }
}
