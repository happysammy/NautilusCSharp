//--------------------------------------------------------------------------------------------------
// <copyright file="LogId.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Logging
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Represents a <see cref="Nautilus"/> specific log event identifier.
    /// </summary>
    public static class LogId
    {
        /// <summary>
        /// Gets the event identifier for component operation events.
        /// </summary>
        public static EventId Operation { get; } = new EventId(0, nameof(Operation));

        /// <summary>
        /// Gets the event identifier for disk input-output events.
        /// </summary>
        public static EventId Disk { get; } = new EventId(1, nameof(Disk));

        /// <summary>
        /// Gets the event identifier for networking events.
        /// </summary>
        public static EventId Networking { get; } = new EventId(2, nameof(Networking));

        /// <summary>
        /// Gets the event identifier for database events.
        /// </summary>
        public static EventId Database { get; } = new EventId(3, nameof(Database));

        /// <summary>
        /// Gets the event identifier for trading events.
        /// </summary>
        public static EventId Trading { get; } = new EventId(4, nameof(Trading));
    }
}
