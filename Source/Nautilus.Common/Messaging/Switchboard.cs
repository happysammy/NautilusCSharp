// -------------------------------------------------------------------------------------------------
// <copyright file="Switchboard.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Exceptions;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Provides a messaging switchboard for all addresses within the service.
    /// </summary>
    public sealed class Switchboard
    {
        private readonly ImmutableDictionary<Address, IEndpoint> addresses;
        private Action<object> deadLetterHandler = DoNothing;

        /// <summary>
        /// Initializes a new instance of the <see cref="Switchboard"/> class.
        /// </summary>
        /// <param name="addresses">The component addresses.</param>
        private Switchboard(Dictionary<Address, IEndpoint> addresses)
        {
            this.addresses = addresses.ToImmutableDictionary();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Switchboard"/> class.
        /// </summary>
        /// <returns>The empty switchboard.</returns>
        public static Switchboard Empty()
        {
            return new Switchboard(new Dictionary<Address, IEndpoint>());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Switchboard"/> class.
        /// </summary>
        /// <param name="addressDictionary">The component addresses.</param>
        /// <returns>The switchboard.</returns>
        public static Switchboard Create(Dictionary<Address, IEndpoint> addressDictionary)
        {
            Condition.NotEmpty(addressDictionary, nameof(addressDictionary));

            return new Switchboard(addressDictionary);
        }

        /// <summary>
        /// Returns a value indicating whether the given envelope was sent to its receiver address (True),
        /// or if the given envelope was sent to the dead letters channel (False).
        /// </summary>
        /// <param name="envelope">The envelope to send.</param>
        /// <returns>True if the envelope was sent to the receiver, False if sent to Dead Letters.</returns>
        public bool SendToReceiver(IEnvelope envelope)
        {
            if (envelope.Receiver is null)
            {
                // Receiver address not found
                this.deadLetterHandler(envelope);
                return false;
            }

            if (this.addresses.TryGetValue((Address)envelope.Receiver, out var receiver))
            {
                receiver.Send(envelope);
                return true;
            }
            else
            {
                // Receiver address not found
                this.deadLetterHandler(envelope);
                return false;
            }
        }

        /// <summary>
        /// Registers the given delegate to receive dead letters.
        /// </summary>
        /// <param name="handler">The dead letter handler.</param>
        public void RegisterDeadLetterChannel(Action<object> handler)
        {
            this.deadLetterHandler = handler;
        }

        private static void DoNothing(object message)
        {
            throw new DesignTimeException("A dead letter handler has not been implemented.");
        }
    }
}
