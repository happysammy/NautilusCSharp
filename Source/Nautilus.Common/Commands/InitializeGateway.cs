//--------------------------------------------------------------------------------------------------
// <copyright file="InitializeGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Commands
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// Represents a command to initialize the given execution gateway.
    /// </summary>
    [Immutable]
    public sealed class InitializeGateway : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeGateway"/> class.
        /// </summary>
        /// <param name="executionGateway">The execution gateway.</param>
        /// <param name="commandId">The command identifier (cannot be default).</param>
        /// <param name="commandTimestamp">The command timestamp (cannot be default).</param>
        public InitializeGateway(
            IExecutionGateway executionGateway,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(commandId, commandTimestamp)
        {
            Validate.NotNull(executionGateway, nameof(executionGateway));
            Validate.NotDefault(commandId, nameof(commandId));
            Validate.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.ExecutionGateway = executionGateway;
        }

        /// <summary>
        /// Gets the messages execution gateway.
        /// </summary>
        public IExecutionGateway ExecutionGateway { get; }
    }
}
