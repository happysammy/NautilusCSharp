//--------------------------------------------------------------------------------------------------
// <copyright file="OrderCommand.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands.Base
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Interfaces;
    using NodaTime;

    /// <summary>
    /// The base class for all order commands.
    /// </summary>
    [Immutable]
    public abstract class OrderCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderCommand"/> class.
        /// </summary>
        /// <param name="order">The commands order.</param>
        /// <param name="commandId">The commands identifier (cannot be default).</param>
        /// <param name="commandTimestamp">The commands timestamp (cannot be default).</param>
        protected OrderCommand(
            IOrder order,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(commandId, commandTimestamp)
        {
            Debug.NotNull(order, nameof(order));
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.Order = order;
        }

        /// <summary>
        /// Gets the commands order symbol.
        /// </summary>
        public IOrder Order { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="CancelOrder"/> command message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{base.ToString()}-{this.Order.Symbol}-{this.Order.Id}";
    }
}
