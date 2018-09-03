// -------------------------------------------------------------------------------------------------
// <copyright file="Switchboard.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Collections;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Represents a messaging switchboard of all addresses within the system.
    /// </summary>
    [Stateless]
    [PerformanceOptimized]
    public sealed class Switchboard
    {
        private readonly ReadOnlyDictionary<NautilusService, IEndpoint> addresses;

        /// <summary>
        /// Initializes a new instance of the <see cref="Switchboard"/> class.
        /// </summary>
        /// <param name="addresses">The system addresses.</param>
        public Switchboard(Dictionary<NautilusService, IEndpoint> addresses)
        {
            Debug.NotNullOrEmpty(addresses, nameof(addresses));

            this.addresses = new ReadOnlyDictionary<NautilusService, IEndpoint>(addresses);
            this.Services = new ReadOnlyList<NautilusService>(addresses.Keys);
        }

        /// <summary>
        /// Gets the switchboards registered services.
        /// </summary>
        public ReadOnlyList<NautilusService> Services { get; }

        /// <summary>
        /// Sends the given envelope to its receiver address(s).
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        /// <typeparam name="T">The message type.</typeparam>
        /// <exception cref="InvalidOperationException">If the envelope receiver address is unknown.</exception>
        public void SendToReceiver<T>(Envelope<T> envelope)
            where T : Message
        {
            Debug.NotNull(envelope, nameof(envelope));

            var receiver = envelope.Receiver;

            if (!this.addresses.ContainsKey(envelope.Receiver))
            {
                throw new InvalidOperationException(
                    "Cannot send message (envelope receiver endpoint address unknown).");
            }

            this.addresses[receiver].Send(envelope);
        }
    }
}
