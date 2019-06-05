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
    using System.Collections.Immutable;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
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
        /// Gets the switchboards registered addresses.
        /// </summary>
        public IEnumerable<Address> Addresses => this.addresses.Keys;

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
        /// Sends the given envelope to its receiver address.
        /// </summary>
        /// <param name="envelope">The envelope to send.</param>
        /// <typeparam name="T">The envelope message type.</typeparam>
        public void SendToReceiver<T>(Envelope<T> envelope)
            where T : Message
        {
            if (envelope.Receiver is null)
            {
                // Receiver address not found.
                this.deadLetterHandler(envelope);
                return;
            }

            if (!this.addresses.ContainsKey((Address)envelope.Receiver))
            {
                // Receiver address not found.
                this.deadLetterHandler(envelope);
                return;
            }

            this.addresses[(Address)envelope.Receiver].Send(envelope);
        }

        /// <summary>
        /// Registers the given delegate to receive dead letters.
        /// </summary>
        /// <param name="handler">The dead letter handler.</param>
        public void RegisterDeadLetterChannel(Action<object> handler)
        {
            this.deadLetterHandler = handler;
        }

        /// <summary>
        /// Do nothing with the given message.
        /// </summary>
        /// <param name="message">The message.</param>
        private static void DoNothing(object message)
        {
        }
    }
}
