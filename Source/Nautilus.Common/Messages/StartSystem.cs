// -------------------------------------------------------------------------------------------------
// <copyright file="StartSystem.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages
{
    using System;
    using NodaTime;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Represents a command message to start the system.
    /// </summary>
    [Immutable]
    public sealed class StartSystem : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartSystem"/> class.
        /// </summary>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        public StartSystem(Guid identifier, ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotNull(identifier, nameof(identifier));
            Debug.NotNull(timestamp, nameof(timestamp));
        }
    }
}
