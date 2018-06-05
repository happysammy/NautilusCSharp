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
    using System.Collections.Immutable;
    using Akka.Actor;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// Represents a messaging switchboard of all addresses within the system.
    /// </summary>
    [Immutable]
    public sealed class Switchboard : ISwitchboard
    {
        private readonly ImmutableDictionary<Enum, IActorRef> addresses;

        /// <summary>
        /// Initializes a new instance of the <see cref="Switchboard"/> class.
        /// </summary>
        /// <param name="addresses">The system addresses.</param>
        /// <exception cref="ValidationException">If the validation fails.</exception>
        public Switchboard(IReadOnlyDictionary<Enum, IActorRef> addresses)
        {
            Validate.NotNull(addresses, nameof(addresses));

            this.addresses = addresses.ToImmutableDictionary();
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
            Validate.NotNull(envelope, nameof(envelope));

            foreach (var receiver in envelope.Receivers)
            {
                this.RouteEnvelope(receiver, envelope);
            }
        }

        private void RouteEnvelope<T>(Enum receiver, Envelope<T> envelope)
            where T : Message
        {
            Debug.NotNull(envelope, nameof(envelope));

            if (!this.addresses.ContainsKey(receiver))
            {
                throw new InvalidOperationException("Message envelope receiver address unknown.");
            }

            this.addresses[receiver].Tell(envelope);
        }
    }
}
