//--------------------------------------------------------------------------------------------------
// <copyright file="SubmitAtomicOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Messages.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Identifiers;
    using NodaTime;

    /// <summary>
    /// Represents a command to submit an order to the execution system.
    /// </summary>
    [Immutable]
    public sealed class SubmitAtomicOrder : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitAtomicOrder"/> class.
        /// </summary>
        /// <param name="atomicOrder">The atomic order to submit.</param>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="strategyId">The strategy identifier.</param>
        /// <param name="positionId">The position identifier.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public SubmitAtomicOrder(
            AtomicOrder atomicOrder,
            TraderId traderId,
            StrategyId strategyId,
            PositionId positionId,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                typeof(SubmitAtomicOrder),
                commandId,
                commandTimestamp)
        {
            this.AtomicOrder = atomicOrder;
            this.HasTakeProfit = atomicOrder.HasTakeProfit;
            this.TraderId = traderId;
            this.StrategyId = strategyId;
            this.PositionId = positionId;
        }

        /// <summary>
        /// Gets the commands atomic order.
        /// </summary>
        public AtomicOrder AtomicOrder { get; }

        /// <summary>
        /// Gets a value indicating whether the commands atomic order has a take profit.
        /// </summary>
        public bool HasTakeProfit { get; }

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
    }
}
