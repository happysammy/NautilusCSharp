//--------------------------------------------------------------------------------------------------
// <copyright file="CommandRejected.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Events
{
    using System;
    using Nautilus.Core;
    using NodaTime;

    /// <summary>
    /// Represents an event where a command was rejected.
    /// </summary>
    public class CommandRejected : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRejected"/> class.
        /// </summary>
        /// <param name="rejected">The rejected command.</param>
        /// <param name="reason">The rejection reason.</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="timestamp">The event timestamp.</param>
        public CommandRejected(
            Command rejected,
            string reason,
            Guid id,
            ZonedDateTime timestamp)
            : base(typeof(CommandRejected), id, timestamp)
        {
            this.Rejected = rejected;
            this.Reason = reason;
        }

        /// <summary>
        /// Gets the events rejected command.
        /// </summary>
        public Command Rejected { get; }

        /// <summary>
        /// Gets the events rejection reason.
        /// </summary>
        public string Reason { get; }
    }
}
