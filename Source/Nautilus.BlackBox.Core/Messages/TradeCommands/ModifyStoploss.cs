// -------------------------------------------------------------------------------------------------
// <copyright file="ModifyStopLoss.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Messages.TradeCommands
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Base;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="ModifyStopLoss"/> class.
    /// </summary>
    [Immutable]
    public sealed class ModifyStopLoss : CommandMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyStopLoss"/> class.
        /// </summary>
        /// <param name="forTrade">The message for trade.</param>
        /// <param name="stopLossModificationsIndex">The message stop-loss modifications index.</param>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public ModifyStopLoss(
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
        /// Returns a string representation of the <see cref="ModifyStopLoss"/> command message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(ModifyStopLoss)}-{this.ForTrade.Symbol}-{this.ForTrade}";
    }
}
