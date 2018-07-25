//--------------------------------------------------------------------------------------------------
// <copyright file="OrderCommand.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Commands.Base
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The base class for all order commands.
    /// </summary>
    [Immutable]
    public abstract class OrderCommand : Command
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="OrderCommand"/> abstract base class.
        /// </summary>
        /// <param name="orderSymbol">The commands order symbol.</param>
        /// <param name="orderId">The commands order identifier.</param>
        /// <param name="commandId">The commands identifier (cannot be default).</param>
        /// <param name="commandTimestamp">The commands timestamp (cannot be default).</param>
        protected OrderCommand(
            Symbol orderSymbol,
            EntityId orderId,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(commandId, commandTimestamp)
        {
            Debug.NotNull(orderSymbol, nameof(orderSymbol));
            Debug.NotNull(orderId, nameof(orderId));
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.OrderSymbol = orderSymbol;
            this.OrderId = orderId;
        }

        /// <summary>
        /// Gets the commands order symbol.
        /// </summary>
        public Symbol OrderSymbol { get; }

        /// <summary>
        /// Gets the commands order identifier.
        /// </summary>
        public EntityId OrderId { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="CancelOrder"/> command message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{base.ToString()}-{this.OrderSymbol}-{this.OrderId}";
    }
}
