// -------------------------------------------------------------------------------------------------
// <copyright file="CommandOk.cs" company="Nautech Systems Pty Ltd">
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
    /// A response acknowledging receipt of a command.
    /// </summary>
    [Immutable]
    public sealed class CommandOk : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandOk"/> class.
        /// </summary>
        /// <param name="commandType">The command type.</param>
        /// <param name="correlationId">The request correlation identifier.</param>
        /// <param name="id">The documents identifier.</param>
        /// <param name="timestamp">The documents timestamp.</param>
        public CommandOk(
            string commandType,
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
        }

        /// <summary>
        /// Gets the responses component name.
        /// </summary>
        public string CommandType { get; }
    }
}
