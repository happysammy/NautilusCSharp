//--------------------------------------------------------------------------------------------------
// <copyright file="InitializeSwitchboard.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using NodaTime;

    /// <summary>
    /// Represents a command to initialize a messaging switchboard.
    /// </summary>
    [Immutable]
    public sealed class InitializeSwitchboard : Command
    {
        private static readonly Type EventType = typeof(InitializeSwitchboard);

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeSwitchboard"/> class.
        /// </summary>
        /// <param name="switchboard">The command switchboard.</param>
        /// <param name="id">The command identifier.</param>
        /// <param name="timestamp">The command timestamp.</param>
        public InitializeSwitchboard(
            Switchboard switchboard,
            Guid id,
            ZonedDateTime timestamp)
            : base(EventType, id, timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Switchboard = switchboard;
        }

        /// <summary>
        /// Gets the commands switchboard.
        /// </summary>
        public Switchboard Switchboard { get; }
    }
}
