// -------------------------------------------------------------------------------------------------
// <copyright file="Switchboard.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Collections;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Represents a messaging switchboard of all addresses within the system.
    /// </summary>
    [PerformanceOptimized]
    public sealed class Switchboard
    {
        private readonly ReadOnlyDictionary<Address, IEndpoint> addresses;

        /// <summary>
        /// Initializes a new instance of the <see cref="Switchboard"/> class.
        /// </summary>
        /// <param name="addresses">The system addresses.</param>
        public Switchboard(Dictionary<Address, IEndpoint> addresses)
        {
            Debug.NotNullOrEmpty(addresses, nameof(addresses));

            this.addresses = new ReadOnlyDictionary<Address, IEndpoint>(addresses);
            this.Addresses = new ReadOnlyList<Address>(addresses.Keys);
        }

        /// <summary>
        /// Gets the switchboards registered addresses.
        /// </summary>
        public ReadOnlyList<Address> Addresses { get; }

        /// <summary>
        /// Sends the given envelope to its receiver address.
        /// </summary>
        /// <param name="envelope">The envelope to send.</param>
        /// <typeparam name="T">The envelope message type.</typeparam>
        /// <exception cref="InvalidOperationException">If the envelope receiver address is unknown.</exception>
        public void SendToReceiver<T>(Envelope<T> envelope)
            where T : Message
        {
            Debug.NotNull(envelope, nameof(envelope));

            if (!this.addresses.ContainsKey(envelope.Receiver))
            {
                throw new InvalidOperationException("Cannot send message (envelope receiver address unknown).");
            }

            this.addresses[envelope.Receiver].Send(envelope);
        }
    }
}
