//--------------------------------------------------------------------------------------------------
// <copyright file="CancelOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Messages.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Identifiers;
    using NodaTime;

    /// <summary>
    /// Represents a command to cancel an order.
    /// </summary>
    [Immutable]
    public sealed class CancelOrder : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelOrder"/> class.
        /// </summary>
        /// <param name="traderId">The trader identifier.</param>
        /// <param name="strategyId">The strategy identifier.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="cancelReason">The cancel reason.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public CancelOrder(
            TraderId traderId,
            StrategyId strategyId,
            OrderId orderId,
            string cancelReason,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                typeof(CancelOrder),
                commandId,
                commandTimestamp)
        {
            Debug.NotEmptyOrWhiteSpace(cancelReason, nameof(cancelReason));
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.TraderId = traderId;
            this.StrategyId = strategyId;
            this.OrderId = orderId;
            this.CancelReason = cancelReason;
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
        /// Gets the commands order identifier.
        /// </summary>
        public OrderId OrderId { get; }

        /// <summary>
        /// Gets the commands cancel reason.
        /// </summary>
        public string CancelReason { get; }
    }
}
