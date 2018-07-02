﻿//--------------------------------------------------------------------------------------------------
// <copyright file="EventMessage.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// The message wrapper for all <see cref="Event"/>(s) messages system.
    /// </summary>
    [Immutable]
    public sealed class EventMessage : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventMessage"/> class.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="id">The event message identifier.</param>
        /// <param name="timestamp">The event message timestamp.</param>
        public EventMessage(
            Event @event,
            Guid id,
            ZonedDateTime timestamp)
            : base(id, timestamp)
        {
            Debug.NotNull(@event, nameof(@event));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Event = @event;
        }

        /// <summary>
        /// Gets the event.
        /// </summary>
        public Event Event { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="EventMessage"/>.
        /// </summary>
        /// <returns>The event name.</returns>
        public override string ToString() => this.Event.ToString();
    }
}
