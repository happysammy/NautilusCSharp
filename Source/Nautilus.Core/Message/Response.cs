//--------------------------------------------------------------------------------------------------
// <copyright file="Response.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Message
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Types;
    using NodaTime;

    /// <summary>
    /// The base class for all <see cref="Response"/> messages.
    /// </summary>
    [Immutable]
    public abstract class Response : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="type">The response type.</param>
        /// <param name="correlationId">The response correlation identifier.</param>
        /// <param name="id">The response identifier.</param>
        /// <param name="timestamp">The response timestamp.</param>
        protected Response(
            Type type,
            Guid correlationId,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                type,
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.CorrelationId = correlationId;
        }

        /// <summary>
        /// Gets the responses correlation identifier.
        /// </summary>
        public Guid CorrelationId { get; }

        /// <summary>
        /// Returns a string representation of this <see cref="Message"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{this.Type.Name}(CorrelationId={this.CorrelationId})";
    }
}
