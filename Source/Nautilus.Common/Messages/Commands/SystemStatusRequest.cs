// -------------------------------------------------------------------------------------------------
// <copyright file="SystemStatusRequest.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a command requesting .
    /// </summary>
    [Immutable]
    public sealed class SystemStatusRequest : SystemCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemStatusRequest"/> class.
        /// </summary>
        /// <param name="identifier">The commands identifier.</param>
        /// <param name="timestamp">The commands timestamp.</param>
        public SystemStatusRequest(
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));
        }
    }
}
