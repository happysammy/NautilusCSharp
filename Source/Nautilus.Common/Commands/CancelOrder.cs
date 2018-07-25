//--------------------------------------------------------------------------------------------------
// <copyright file="CancelOrder.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.Interfaces;
    using NodaTime;

    /// <summary>
    /// Represents a command to cancel an order.
    /// </summary>
    [Immutable]
    public sealed class CancelOrder : OrderCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelOrder"/> class.
        /// </summary>
        /// <param name="order">The commands order to cancel.</param>
        /// <param name="commandId">The commands identifier (cannot be default).</param>
        /// <param name="commandTimestamp">The commands timestamp (cannot be default).</param>
        public CancelOrder(
            IOrder order,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                order,
                commandId,
                commandTimestamp)
        {
            Debug.NotNull(order, nameof(order));
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));
        }
    }
}
