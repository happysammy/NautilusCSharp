// -------------------------------------------------------------------------------------------------
// <copyright file="StartSystem.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using Nautilus.Common.Messages.Commands.Base;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using NodaTime;

    /// <summary>
    /// Represents a command message to start the system.
    /// </summary>
    [Immutable]
    public sealed class StartSystem : SystemCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartSystem"/> class.
        /// </summary>
        /// <param name="identifier">The commands identifier.</param>
        /// <param name="timestamp">The commands timestamp.</param>
        public StartSystem(Guid identifier, ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotNull(identifier, nameof(identifier));
            Debug.NotNull(timestamp, nameof(timestamp));
        }
    }
}
