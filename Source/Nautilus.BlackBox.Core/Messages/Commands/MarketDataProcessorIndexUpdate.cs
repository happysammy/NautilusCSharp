//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataProcessorIndexUpdate.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Messages.Commands
{
    using System;
    using System.Collections.Generic;
    using Akka.Actor;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Core;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="MarketDataProcessorIndexUpdate"/> class.
    /// </summary>
    [Immutable]
    public sealed class MarketDataProcessorIndexUpdate : Command
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

    }
}
