//--------------------------------------------------------------------------------------------------
// <copyright file="BrokerageDisconnected.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Events
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using NodaTime;

    /// <summary>
    /// Represents an event where a brokerage has been disconnected.
    /// </summary>
    [Immutable]
    public sealed class BrokerageDisconnected : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerageDisconnected"/> class.
        /// </summary>
        /// <param name="broker">The events brokerage disconnected from.</param>
        /// <param name="session">The events session information.</param>
        /// <param name="identifier">The event identifier (cannot be default).</param>
        /// <param name="timestamp">The event timestamp (cannot be default).</param>
        public BrokerageDisconnected(
            Broker broker,
            string session,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Broker = broker;
            this.Session = session;
        }

        /// <summary>
        /// Gets the events brokerage.
        /// </summary>
        public Broker Broker { get; }

        /// <summary>
        /// Gets the events session information.
        /// </summary>
        public string Session { get; }
    }
}
