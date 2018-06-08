// -------------------------------------------------------------------------------------------------
// <copyright file="MarketDataDelivery.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Messages
{
    using Nautilus.Common.Messaging;
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Data.Types;
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
        /// <param name="barData">The message market data.</param>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        public MarketDataDelivery(
            BarDataFrame barData,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Validate.NotNull(barData, nameof(barData));
            Validate.NotDefault(identifier, nameof(barData));
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.BarData = barData;
        }

        /// <summary>
        /// Gets the messages market data.
        /// </summary>
        public BarDataFrame BarData { get; }

        /// <summary>
        /// Gets a string representation of the <see cref="MarketDataDelivery"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(MarketDataDelivery)}-{this.Id}";
    }
}
