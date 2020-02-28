//--------------------------------------------------------------------------------------------------
// <copyright file="ModifyOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Commands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a command to modify an order.
    /// </summary>
    [Immutable]
    public sealed class ModifyOrder : Command
    {
        private static readonly Type CommandType = typeof(ModifyOrder);

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyOrder"/> class.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="modifiedQuantity">The modified quantity.</param>
        /// <param name="modifiedPrice">The modified price.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public ModifyOrder(
            TraderId traderId,
            AccountId accountId,
            OrderId orderId,
            Quantity modifiedQuantity,
            Price modifiedPrice,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                CommandType,
                commandId,
                commandTimestamp)
        {
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.TraderId = traderId;
            this.AccountId = accountId;
            this.OrderId = orderId;
            this.ModifiedQuantity = modifiedQuantity;
            this.ModifiedPrice = modifiedPrice;
        }

        /// <summary>
        /// Gets the commands trader identifier.
        /// </summary>
        public TraderId TraderId { get; }

        /// <summary>
        /// Gets the commands account identifier.
        /// </summary>
        public AccountId AccountId { get; }

        /// <summary>
        /// Gets the commands order identifier.
        /// </summary>
        public OrderId OrderId { get; }

        /// <summary>
        /// Gets the commands modified order quantity.
        /// </summary>
        public Quantity ModifiedQuantity { get; }

        /// <summary>
        /// Gets the commands modified order price.
        /// </summary>
        public Price ModifiedPrice { get; }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}(" +
                                             $"TraderId={this.TraderId.Value}, " +
                                             $"AccountId={this.AccountId.Value}, " +
                                             $"OrderId={this.OrderId.Value}, " +
                                             $"Quantity={this.ModifiedQuantity.ToStringFormatted()}, " +
                                             $"Price={this.ModifiedPrice})";
    }
}
