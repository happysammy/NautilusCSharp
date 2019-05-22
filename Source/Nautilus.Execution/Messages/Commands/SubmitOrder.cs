//--------------------------------------------------------------------------------------------------
// <copyright file="SubmitOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Messages.Commands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.Execution.Messages.Commands.Base;
    using NodaTime;

    /// <summary>
    /// Represents a command to submit an order to the execution system.
    /// </summary>
    [Immutable]
    public sealed class SubmitOrder : OrderCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitOrder"/> class.
        /// </summary>
        /// <param name="order">The order to submit.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public SubmitOrder(
            Order order,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(order, commandId, commandTimestamp)
        {
        }
    }
}
