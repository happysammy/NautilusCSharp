//--------------------------------------------------------------------------------------------------
// <copyright file="SubmitOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Messages.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Identifiers;
    using NodaTime;

    /// <summary>
    /// Represents a command to submit an <see cref="Order"/>.
    /// </summary>
    [Immutable]
    public sealed class SubmitOrder : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitOrder"/> class.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="strategyId">The strategy identifier.</param>
        /// <param name="positionId">The position identifier.</param>
        /// <param name="order">The order to submit.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public SubmitOrder(
            TraderId traderId,
            StrategyId strategyId,
            PositionId positionId,
            Order order,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                typeof(SubmitOrder),
                commandId,
                commandTimestamp)
        {
            this.TraderId = traderId;
            this.StrategyId = strategyId;
            this.PositionId = positionId;
            this.Order = order;
        }

        /// <summary>
        /// Gets the commands trader identifier.
        /// </summary>
        public TraderId TraderId { get; }

        /// <summary>
        /// Gets the commands strategy identifier.
        /// </summary>
        public StrategyId StrategyId { get; }

        /// <summary>
        /// Gets the commands position identifier.
        /// </summary>
        public PositionId PositionId { get; }

        /// <summary>
        /// Gets the commands order.
        /// </summary>
        public Order Order { get; }
    }
}
