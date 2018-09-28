// -------------------------------------------------------------------------------------------------
// <copyright file="SystemStatusResponse.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using Nautilus.Common.Enums;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// A document containing the status response of a system service.
    /// </summary>
    [Immutable]
    public sealed class SystemStatusResponse : Document
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemStatusResponse"/> class.
        /// </summary>
        /// <param name="componentName">The component name.</param>
        /// <param name="status">The status.</param>
        /// <param name="identifier">The documents identifier.</param>
        /// <param name="timestamp">The documents timestamp.</param>
        public SystemStatusResponse(
            Label componentName,
            Status status,
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
        /// Gets the documents component name.
        /// </summary>
        public Label ComponentName { get; }

        /// <summary>
        /// Gets the documents system status.
        /// </summary>
        public Status Status { get; }

        /// <summary>
        /// Gets a string representation of the <see cref="SystemStatusResponse"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{base.ToString()}-{this.ComponentName}={this.Status}";
    }
}
