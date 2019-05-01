//--------------------------------------------------------------------------------------------------
// <copyright file="ClosePosition.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Aggregates;
    using NodaTime;

    /// <summary>
    /// Represents a command to close a trade position.
    /// </summary>
    [Immutable]
    public sealed class ClosePosition : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClosePosition"/> class.
        /// </summary>
        /// <param name="position">The commands position to close.</param>
        /// <param name="commandId">The commands identifier (cannot be default).</param>
        /// <param name="commandTimestamp">The commands timestamp (cannot be default).</param>
        public ClosePosition(
            Position position,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                commandId,
                commandTimestamp)
        {
            Debug.NotDefault(commandId, nameof(commandId));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.Position = position;
        }

        /// <summary>
        /// Gets the commands for trade unit.
        /// </summary>
        public Position Position { get; }
    }
}
