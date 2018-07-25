//--------------------------------------------------------------------------------------------------
// <copyright file="ModifyOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Commands
{
    using System;
    using Nautilus.Common.Commands.Base;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel;
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
        /// <param name="orderSymbol">The command order symbol.</param>
        /// <param name="forOrderId">The command for order identifier.</param>
        /// <param name="modifiedPrice">The command modified price.</param>
        /// <param name="commandId">The command identifier (cannot be default).</param>
        /// <param name="commandTimestamp">The command timestamp (cannot be default).</param>
        public ModifyOrder(
            Symbol orderSymbol,
            EntityId forOrderId,
            Price modifiedPrice,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                orderSymbol,
                forOrderId,
                commandId,
                commandTimestamp)
        {
            Debug.NotNull(orderSymbol, nameof(orderSymbol));
            Debug.NotNull(forOrderId, nameof(forOrderId));
            Debug.NotNull(modifiedPrice, nameof(modifiedPrice));
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.ForOrderId = forOrderId;
            this.ModifiedPrice = modifiedPrice;
        }

        /// <summary>
        /// Gets the commands for order identifier.
        /// </summary>
        public EntityId ForOrderId { get; }

        /// <summary>
        /// Gets the commands modified order price.
        /// </summary>
        public Price ModifiedPrice { get; }
    }
}
