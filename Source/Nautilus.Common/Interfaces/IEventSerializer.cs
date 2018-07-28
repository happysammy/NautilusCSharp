//--------------------------------------------------------------------------------------------------
// <copyright file="IEventSerializer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.Core;

    /// <summary>
    /// Provides an interface for event serializers.
    /// </summary>
    public interface IEventSerializer
    {
        /// <summary>
        /// Serialize the given event.
        /// </summary>
        /// <param name="event">The event to serialize.</param>
        /// <returns>The serialized event.</returns>
        byte[] Serialize(Event @event);

        /// <summary>
        /// Deserialize the given event bytes.
        /// </summary>
        /// <param name="eventBytes">The event bytes to deserialize.</param>
        /// <returns>The deserialized event.</returns>
        Event Deserialize(byte[] eventBytes);
    }
}
