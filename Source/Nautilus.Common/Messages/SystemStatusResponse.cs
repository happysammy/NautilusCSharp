// -------------------------------------------------------------------------------------------------
// <copyright file="SystemStatusResponse.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Messaging;
    using NodaTime;

    /// <summary>
    /// The system status request message.
    /// </summary>
    [Immutable]
    public sealed class SystemStatusResponse : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemStatusResponse"/> class.
        /// </summary>
        /// <param name="componentName">The component name.</param>
        /// <param name="status">The status.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public SystemStatusResponse(
            string componentName,
            SystemStatus status,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotNull(componentName, nameof(componentName));
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.ComponentName = componentName;
            this.Status = status;
        }

        /// <summary>
        /// Gets the messages component name.
        /// </summary>
        public string ComponentName { get; }

        /// <summary>
        /// Gets the messages system status.
        /// </summary>
        public SystemStatus Status { get; }

        /// <summary>
        /// Gets a string representation of the <see cref="SystemStatusResponse"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{base.ToString()}-{this.ComponentName}={this.Status}";
    }
}
