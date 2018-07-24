//--------------------------------------------------------------------------------------------------
// <copyright file="ModifyStopLoss.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Core;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a command to modify an order.
    /// </summary>
    [Immutable]
    public sealed class ModifyOrder : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyOrder"/> class.
        /// </summary>
        /// <param name="forTrade">The message for trade.</param>
        /// <param name="stopLossModificationsIndex">The message stop-loss modifications index.</param>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public ModifyOrder(
            Trade forTrade,
            Dictionary<Order, Price> stopLossModificationsIndex,
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Validate.NotNull(forTrade, nameof(forTrade));
            Validate.CollectionNotNullOrEmpty(stopLossModificationsIndex, nameof(stopLossModificationsIndex));
            Validate.NotDefault(messageId, nameof(messageId));
            Validate.NotDefault(messageTimestamp, nameof(messageTimestamp));

            this.ForTrade = forTrade;
            this.StopLossModificationsIndex = stopLossModificationsIndex.ToImmutableDictionary();
        }

        /// <summary>
        /// Gets the messages for trade.
        /// </summary>
        public Trade ForTrade { get; }

        /// <summary>
        /// Gets the messages stop-loss modifications index.
        /// </summary>
        public IReadOnlyDictionary<Order, Price> StopLossModificationsIndex { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="ModifyOrder"/> command message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{base.ToString()}-{this.ForTrade.Symbol}-{this.ForTrade}";
    }
}
