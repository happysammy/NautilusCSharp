//--------------------------------------------------------------
// <copyright file="MarketDataProcessorIndexUpdate.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Messages.SystemCommands
{
    using System;
    using System.Collections.Generic;
    using Akka.Actor;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="MarketDataProcessorIndexUpdate"/> class.
    /// </summary>
    [Immutable]
    public sealed class MarketDataProcessorIndexUpdate : CommandMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDataProcessorIndexUpdate"/> class.
        /// </summary>
        /// <param name="marketDataProcessorsIndex">The message market data processors index.</param>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public MarketDataProcessorIndexUpdate(
            IDictionary<Symbol, IActorRef> marketDataProcessorsIndex,
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Validate.NotNull(marketDataProcessorsIndex, nameof(marketDataProcessorsIndex));
            Validate.NotDefault(messageId, nameof(messageId));
            Validate.NotDefault(messageTimestamp, nameof(messageTimestamp));

            this.MarketDataProcessorsIndex = marketDataProcessorsIndex;
        }

        /// <summary>
        /// Gets the market data processors index.
        /// </summary>
        public IDictionary<Symbol, IActorRef> MarketDataProcessorsIndex { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="MarketDataProcessorIndexUpdate"/>
        /// service message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(MarketDataProcessorIndexUpdate);
    }
}