//--------------------------------------------------------------------------------------------------
// <copyright file="Message.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// The base class for all message types.
    /// </summary>
    [Immutable]
    public abstract class Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="id">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        protected Message(Guid id, ZonedDateTime timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Id = id;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the message timestamp.
        /// </summary>
        public ZonedDateTime Timestamp { get; }
    }
}
