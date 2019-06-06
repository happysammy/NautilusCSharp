//--------------------------------------------------------------------------------------------------
// <copyright file="ComponentBusConnected.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Componentry
{
    using System.Collections.Generic;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Messaging;

    /// <summary>
    /// The base class for all components which are connected to the message bus.
    /// </summary>
    public abstract class ComponentBusConnected : Component
    {
        private readonly IMessagingAdapter messagingAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBusConnected"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        protected ComponentBusConnected(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter)
            : base(container)
        {
            this.messagingAdapter = messagingAdapter;

            this.RegisterHandler<Envelope<Command>>(this.Open);
            this.RegisterHandler<Envelope<Event>>(this.Open);
            this.RegisterHandler<Envelope<Document>>(this.Open);
        }

        /// <summary>
        /// Sends the given message to the given address via the message bus.
        /// </summary>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message to send.</param>
        /// <typeparam name="T">The message type.</typeparam>
        protected void Send<T>(Address receiver, T message)
            where T : Message
        {
            this.messagingAdapter.Send(
                message,
                receiver,
                this.Address,
                this.TimeNow());
        }

        /// <summary>
        /// Sends the given message to the given address via the message bus.
        /// </summary>
        /// <param name="receivers">The message receivers.</param>
        /// <param name="message">The message to send.</param>
        /// <typeparam name="T">The message type.</typeparam>
        protected void SendAll<T>(List<Address> receivers, T message)
            where T : Message
        {
            for (var i = 0; i < receivers.Count; i++)
            {
                this.messagingAdapter.Send(
                    message,
                    receivers[i],
                    this.Address,
                    this.TimeNow());
            }
        }

        private void Open<T>(Envelope<T> envelope)
            where T : Message
        {
            this.SendToSelf(envelope.Message);
        }
    }
}
