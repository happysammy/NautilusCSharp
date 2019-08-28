//--------------------------------------------------------------------------------------------------
// <copyright file="Event.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
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
    /// The base class for all <see cref="Event"/> messages.
    /// </summary>
    [Immutable]
    public abstract class Event : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="type">The event type.</param>
        /// <param name="id">The event identifier.</param>
        /// <param name="timestamp">The event timestamp.</param>
        protected Event(
            Type type,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                type,
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));
        }
    }
}
