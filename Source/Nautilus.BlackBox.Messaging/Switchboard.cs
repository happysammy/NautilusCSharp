// -------------------------------------------------------------------------------------------------
// <copyright file="Switchboard.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Akka.Actor;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Messaging.Base;

    /// <summary>
    /// The immutable sealed <see cref="Switchboard"/> class. Represents a messaging switchboard of
    /// all <see cref="IActorRef"/> addresses within the <see cref="BlackBox"/> system.
    /// </summary>
    [Immutable]
    public sealed class Switchboard : ISwitchboard
    {
        private readonly ImmutableDictionary<Enum, IActorRef> addresses;

        /// <summary>
        /// Initializes a new instance of the <see cref="Switchboard"/> class.
        /// </summary>
        /// <param name="addresses">The actor addresses for the system.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
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
        public void SendToReceivers<T>(Envelope<T> envelope) where T : Message
        {
            Validate.NotNull(envelope, nameof(envelope));

            foreach (var receiver in envelope.Receivers)
            {
                this.RouteEnvelope(receiver, envelope);
            }
        }

        private void RouteEnvelope<T>(Enum receiver, Envelope<T> envelope) where T : Message
        {
            Debug.NotNull(envelope, nameof(envelope));

            if (!this.addresses.ContainsKey(receiver))
            {
                throw new InvalidOperationException($"Message receiver address unknown = {receiver}.");
            }

            this.addresses[receiver].Tell(envelope);
        }
    }
}
