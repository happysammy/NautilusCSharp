// -------------------------------------------------------------------------------------------------
// <copyright file="SystemStatusRequest.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using NodaTime;
    using Nautilus.Common.Messaging;

    [Immutable]
    public sealed class SystemStatusRequest : Message
    {
        public SystemStatusRequest(
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Validate.NotDefault(identifier, nameof(identifier));
            Validate.NotDefault(timestamp, nameof(timestamp));
        }

        /// <summary>
        /// Gets a string representation of the <see cref="SystemStatusRequest"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(SystemStatusRequest)}-{this.Id}";
    }
}
