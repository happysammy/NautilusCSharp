//--------------------------------------------------------------------------------------------------
// <copyright file="InitializeSwitchboard.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// Represents a command to initialize the messaging system switchboard.
    /// </summary>
    [Immutable]
    public sealed class InitializeSwitchboard : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeSwitchboard"/> class.
        /// </summary>
        /// <param name="switchboard">The command switchboard.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public InitializeSwitchboard(
            Switchboard switchboard,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(commandId, commandTimestamp)
        {
            Precondition.NotNull(switchboard, nameof(switchboard));
            Precondition.NotDefault(commandId, nameof(commandId));
            Precondition.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.Switchboard = switchboard;
        }

        /// <summary>
        /// Gets the messages switchboard.
        /// </summary>
        public Switchboard Switchboard { get; }
    }
}
