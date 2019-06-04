// -------------------------------------------------------------------------------------------------
// <copyright file="CommandRejected.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Responses
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using NodaTime;

    /// <summary>
    /// A response indicating that a command has been rejected.
    /// </summary>
    [Immutable]
    public sealed class CommandRejected : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRejected"/> class.
        /// </summary>
        /// <param name="commandType">The command type.</param>
        /// <param name="rejectedReason">The rejected reason.</param>
        /// <param name="correlationId">The request correlation identifier.</param>
        /// <param name="id">The documents identifier.</param>
        /// <param name="timestamp">The documents timestamp.</param>
        public CommandRejected(
            string commandType,
            string rejectedReason,
            Guid correlationId,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                typeof(StatusResponse),
                correlationId,
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.CommandType = commandType;
            this.RejectedReason = rejectedReason;
        }

        /// <summary>
        /// Gets the responses component name.
        /// </summary>
        public string CommandType { get; }

        /// <summary>
        /// Gets the responses rejected reason.
        /// </summary>
        public string RejectedReason { get; }
    }
}
