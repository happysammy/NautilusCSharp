//--------------------------------------------------------------------------------------------------
// <copyright file="ClosePosition.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Core.Collections;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a command to close a trade position by execution tickets.
    /// </summary>
    [Immutable]
    public sealed class ClosePosition : OrderCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseTradeUnit"/> class.
        /// </summary>
        /// <param name="orderSymbol">The commands position symbol to close.</param>
        /// <param name="fromOrderId">The commands position from entry order identifier.</param>
        /// <param name="tickets">The commands position tickets to close.</param>
        /// <param name="commandId">The commands identifier (cannot be default).</param>
        /// <param name="commandTimestamp">The commands timestamp (cannot be default).</param>
        public ClosePosition(
            Symbol orderSymbol,
            EntityId fromOrderId,
            ReadOnlyList<EntityId> tickets,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                orderSymbol,
                fromOrderId,
                commandId,
                commandTimestamp)
        {
            Debug.NotNull(tickets, nameof(tickets));
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.Tickets = tickets;
        }

        /// <summary>
        /// Gets the commands for trade unit.
        /// </summary>
        public ReadOnlyList<EntityId> Tickets { get; }
    }
}
