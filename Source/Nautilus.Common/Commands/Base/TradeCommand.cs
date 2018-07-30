//--------------------------------------------------------------------------------------------------
// <copyright file="TradeCommand.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The base class for all trade commands.
    /// </summary>
    [Immutable]
    public abstract class TradeCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TradeCommand"/> class.
        /// </summary>
        /// <param name="tradeSymbol">The commands trade symbol.</param>
        /// <param name="tradeId">The commands trade identifier.</param>
        /// <param name="commandId">The commands identifier (cannot be default).</param>
        /// <param name="commandTimestamp">The commands timestamp (cannot be default).</param>
        protected TradeCommand(
            Symbol tradeSymbol,
            TradeId tradeId,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(commandId, commandTimestamp)
        {
            Debug.NotNull(tradeSymbol, nameof(tradeSymbol));
            Debug.NotNull(tradeId, nameof(tradeId));
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.TradeSymbol = tradeSymbol;
            this.TradeId = tradeId;
        }

        /// <summary>
        /// Gets the commands trade symbol.
        /// </summary>
        public Symbol TradeSymbol { get; }

        /// <summary>
        /// Gets the commands trade identifier.
        /// </summary>
        public TradeId TradeId { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="CancelOrder"/> command message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{base.ToString()}-{this.TradeSymbol}-{this.TradeId}";
    }
}
