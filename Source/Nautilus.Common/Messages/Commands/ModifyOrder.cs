//--------------------------------------------------------------------------------------------------
// <copyright file="ModifyOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using Nautilus.Common.Messages.Commands.Base;
    using Nautilus.Core.Annotations;
    using Nautilus.Core;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a command to modify an order.
    /// </summary>
    [Immutable]
    public sealed class ModifyOrder : OrderCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyOrder"/> class.
        /// </summary>
        /// <param name="order">The commands order to modify.</param>
        /// <param name="modifiedPrice">The modified price.</param>
        /// <param name="commandId">The command identifier (cannot be default).</param>
        /// <param name="commandTimestamp">The command timestamp (cannot be default).</param>
        public ModifyOrder(
            Order order,
            Price modifiedPrice,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                order,
                commandId,
                commandTimestamp)
        {
            Debug.NotNull(order, nameof(order));
            Debug.NotNull(modifiedPrice, nameof(modifiedPrice));
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.ModifiedPrice = modifiedPrice;
        }

        /// <summary>
        /// Gets the commands modified order price.
        /// </summary>
        public Price ModifiedPrice { get; }
    }
}
