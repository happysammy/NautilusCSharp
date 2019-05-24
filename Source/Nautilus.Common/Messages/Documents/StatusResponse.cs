// -------------------------------------------------------------------------------------------------
// <copyright file="StatusResponse.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Documents
{
    using System;
    using Nautilus.Common.Enums;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// A document containing the status response of a component.
    /// </summary>
    [Immutable]
    public sealed class StatusResponse : Document
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusResponse"/> class.
        /// </summary>
        /// <param name="componentName">The component name.</param>
        /// <param name="status">The status.</param>
        /// <param name="id">The documents identifier.</param>
        /// <param name="timestamp">The documents timestamp.</param>
        public StatusResponse(
            Label componentName,
            Status status,
            Guid id,
            ZonedDateTime timestamp)
            : base(typeof(StatusResponse), id, timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.ComponentName = componentName;
            this.Status = status;
        }

        /// <summary>
        /// Gets the responses component name.
        /// </summary>
        public Label ComponentName { get; }

        /// <summary>
        /// Gets the responses component status.
        /// </summary>
        public Status Status { get; }

        /// <summary>
        /// Gets a string representation of the <see cref="StatusResponse"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{base.ToString()}-{this.ComponentName}={this.Status}";
    }
}
