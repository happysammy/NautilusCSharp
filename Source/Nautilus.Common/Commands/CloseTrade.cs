//--------------------------------------------------------------------------------------------------
// <copyright file="CloseTradeUnit.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a command to close a trade position by trade unit id.
    /// </summary>
    [Immutable]
    public sealed class CloseTradeUnit : TradeCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseTradeUnit"/> class.
        /// </summary>
        /// <param name="tradeSymbol">The command trade symbol to close.</param>
        /// <param name="tradeId">The commands for trade identifier.</param>
        /// <param name="tradeUnitId">The commands trade unit identifier.</param>
        /// <param name="commandId">The commands identifier (cannot be default).</param>
        /// <param name="commandTimestamp">The commands timestamp (cannot be default).</param>
        public CloseTradeUnit(
            Symbol tradeSymbol,
            EntityId tradeId,
            EntityId tradeUnitId,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                tradeSymbol,
                tradeId,
                commandId,
                commandTimestamp)
        {
            Debug.NotNull(tradeSymbol, nameof(tradeSymbol));
            Debug.NotNull(tradeId, nameof(tradeId));
            Debug.NotNull(tradeUnitId, nameof(tradeUnitId));
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.TradeUnitId = tradeUnitId;
        }

        /// <summary>
        /// Gets the commands trade unit identifier to close.
        /// </summary>
        public EntityId TradeUnitId { get; }
    }
}
