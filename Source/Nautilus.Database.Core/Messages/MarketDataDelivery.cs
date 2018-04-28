// -------------------------------------------------------------------------------------------------
// <copyright file="MarketDataDelivery.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Core.Messages
{
    using Nautilus.Common.Messaging;
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Database.Core.Types;
    using NodaTime;

    /// <summary>
    /// A delivery message of new market data.
    /// </summary>
    [Immutable]
    public sealed class MarketDataDelivery : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDataDelivery"/> class.
        /// </summary>
        /// <param name="marketData">The message market data.</param>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        public MarketDataDelivery(
            MarketDataFrame marketData,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Validate.NotNull(marketData, nameof(marketData));
            Validate.NotDefault(identifier, nameof(marketData));
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.MarketData = marketData;
        }

        /// <summary>
        /// Gets the messages market data.
        /// </summary>
        public MarketDataFrame MarketData { get; }

        /// <summary>
        /// Gets a string representation of the <see cref="MarketDataDelivery"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(MarketDataDelivery)}-{this.Id}";
    }
}
