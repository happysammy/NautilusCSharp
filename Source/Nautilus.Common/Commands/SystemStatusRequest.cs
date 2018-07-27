// -------------------------------------------------------------------------------------------------
// <copyright file="SystemStatusRequest.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Commands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Core;
    using NodaTime;

    /// <summary>
    /// The system status request message.
    /// </summary>
    [Immutable]
    public sealed class SystemStatusRequest : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemStatusRequest"/> class.
        /// </summary>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
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
