//--------------------------------------------------------------------------------------------------
// <copyright file="UnsubscribeSymbolDataType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The command message to unsubscribe from bar data.
    /// </summary>
    [Immutable]
    public sealed class UnsubscribeBarData : CommandMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsubscribeBarData"/> class.
        /// </summary>
        /// <param name="symbol">The message symbol.</param>
        /// <param name="barSpecs">The message bar specifications.</param>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public UnsubscribeBarData(
            Symbol symbol,
            IReadOnlyList<BarSpecification> barSpecs,
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Validate.NotNull(symbol, nameof(symbol));
            Validate.ReadOnlyCollectionNotNullOrEmpty(barSpecs, nameof(barSpecs));
            Validate.NotDefault(messageId, nameof(messageId));
            Validate.NotDefault(messageTimestamp, nameof(messageTimestamp));

            this.Symbol = symbol;
            this.BarSpecifications = barSpecs;
        }

        /// <summary>
        /// Gets the messages symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the messages bar specifications.
        /// </summary>
        public IReadOnlyList<BarSpecification> BarSpecifications { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="UnsubscribeBarData"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(UnsubscribeBarData);
    }
}
