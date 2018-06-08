// -------------------------------------------------------------------------------------------------
// <copyright file="MarketDataPersisted.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Messages.Events
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// A message representing that all bar data has been persisted.
    /// </summary>
    [Immutable]
    public sealed class MarketDataPersisted : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDataPersisted"/> class.
        /// </summary>
        /// <param name="symbolBarSpec">The message symbol bar specification.</param>
        /// <param name="lastBarTime">The message last bar time.</param>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="timestamp">THe message timestamp</param>
        public MarketDataPersisted(
            SymbolBarSpec symbolBarSpec,
            ZonedDateTime lastBarTime,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Validate.NotNull(symbolBarSpec, nameof(symbolBarSpec));
            Validate.NotDefault(lastBarTime, nameof(lastBarTime));
            Validate.NotDefault(identifier, nameof(identifier));
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.SymbolBarSpec = symbolBarSpec;
            this.LastBarTime = lastBarTime;
        }

        /// <summary>
        /// Gets the messages symbol bar specification.
        /// </summary>
        public SymbolBarSpec SymbolBarSpec { get; }

        /// <summary>
        /// Gets the messages last bar time.
        /// </summary>
        public ZonedDateTime LastBarTime { get; }

        /// <summary>
        /// Gets a string representation of the <see cref="MarketDataPersisted"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(MarketDataPersisted)}-{this.Id}";
    }
}
