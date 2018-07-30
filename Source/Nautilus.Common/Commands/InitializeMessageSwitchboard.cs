//--------------------------------------------------------------------------------------------------
// <copyright file="InitializeMessageSwitchboard.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Commands
{
    using System;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// The command to initialize the messaging system switchboard.
    /// </summary>
    [Immutable]
    public sealed class InitializeMessageSwitchboard : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeMessageSwitchboard"/> class.
        /// </summary>
        /// <param name="switchboard">The message switchboard.</param>
        /// <param name="commandId">The message identifier (cannot be default).</param>
        /// <param name="commandTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public InitializeMessageSwitchboard(
            Switchboard switchboard,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(commandId, commandTimestamp)
        {
            Validate.NotNull(switchboard, nameof(switchboard));
            Validate.NotDefault(commandId, nameof(commandId));
            Validate.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.Switchboard = switchboard;
        }

        /// <summary>
        /// Gets the messages account.
        /// </summary>
        public Switchboard Switchboard { get; }
    }
}
