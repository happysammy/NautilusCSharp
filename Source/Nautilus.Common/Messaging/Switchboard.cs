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
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Collections;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Represents a messaging switchboard of all addresses within the system.
    /// </summary>
    [Immutable]
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
            Validate.CollectionNotNullOrEmpty(addresses, nameof(addresses));

            this.addresses = new ReadOnlyDictionary<NautilusService, IEndpoint>(addresses);
        }

        /// <summary>
        /// Sends the given envelope to its receiver address(s).
        /// </summary>
        /// <param name="envelope">The envelope.</param>
        /// <typeparam name="T">The message type.</typeparam>
        /// <exception cref="ValidationException">Throws if the envelope is null.</exception>
        public void SendToReceivers<T>(Envelope<T> envelope)
            where T : Message
        {
            Debug.NotNull(envelope, nameof(envelope));

            foreach (var receiver in envelope.Receivers)
            {
                this.RouteEnvelope(receiver, envelope);
            }
        }

        private void RouteEnvelope<T>(NautilusService receiver, Envelope<T> envelope)
            where T : Message
        {
            Debug.NotNull(receiver, nameof(receiver));
            Debug.NotNull(envelope, nameof(envelope));

            if (!this.addresses.ContainsKey(receiver))
            {
                throw new InvalidOperationException(
                    "Cannot send message (envelope receiver endpoint address unknown).");
            }

            this.addresses[receiver].Send(envelope);
        }
    }
}
