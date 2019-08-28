//--------------------------------------------------------------------------------------------------
// <copyright file="DisconnectFix.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Messages;
    using NodaTime;

    /// <summary>
    /// Represents a scheduled command to disconnect a FIX session.
    /// </summary>
    [Immutable]
    public sealed class DisconnectFix : Command, IScheduledJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectFix"/> class.
        /// </summary>
        /// <param name="scheduledTime">The commands scheduled time.</param>
        /// <param name="id">The commands identifier.</param>
        /// <param name="timestamp">The commands creation timestamp.</param>
        public DisconnectFix(
            ZonedDateTime scheduledTime,
            Guid id,
            ZonedDateTime timestamp)
            : base(typeof(DisconnectFix), id, timestamp)
        {
            Debug.NotDefault(scheduledTime, nameof(scheduledTime));

            this.ScheduledTime = scheduledTime;
        }

        /// <inheritdoc />
        public ZonedDateTime ScheduledTime { get; }
    }
}
