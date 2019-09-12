//--------------------------------------------------------------------------------------------------
// <copyright file="SubmitAtomicOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Commands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;

    /// <summary>
    /// Represents a command to submit an <see cref="AtomicOrder"/>.
    /// </summary>
    [Immutable]
    public sealed class SubmitAtomicOrder : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmitAtomicOrder"/> class.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="strategyId">The strategy identifier.</param>
        /// <param name="positionId">The position identifier.</param>
        /// <param name="atomicOrder">The atomic order to submit.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public SubmitAtomicOrder(
            TraderId traderId,
            AccountId accountId,
            StrategyId strategyId,
            PositionId positionId,
            AtomicOrder atomicOrder,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                typeof(SubmitAtomicOrder),
                commandId,
                commandTimestamp)
        {
            this.TraderId = traderId;
            this.AccountId = accountId;
            this.StrategyId = strategyId;
            this.PositionId = positionId;
            this.AtomicOrder = atomicOrder;
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
        /// Gets the commands strategy identifier.
        /// </summary>
        public StrategyId StrategyId { get; }

        /// <summary>
        /// Gets the commands position identifier.
        /// </summary>
        public PositionId PositionId { get; }

        /// <summary>
        /// Gets the commands atomic order.
        /// </summary>
        public AtomicOrder AtomicOrder { get; }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(SubmitAtomicOrder)}(" +
                                             $"TraderId={this.TraderId.Value}, " +
                                             $"AccountId={this.AccountId.Value}, " +
                                             $"StrategyId={this.StrategyId.Value}, " +
                                             $"PositionId={this.PositionId.Value}, " +
                                             $"AtomicOrderId={this.AtomicOrder.Id.Value})";
    }
}
