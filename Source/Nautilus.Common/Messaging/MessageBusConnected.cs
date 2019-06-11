//--------------------------------------------------------------------------------------------------
// <copyright file="MessageBusConnected.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// The base class for all components which are connected to the message bus.
    /// </summary>
    public abstract class MessageBusConnected : Component
    {
        private readonly IMessageBusAdapter messageBusAdapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBusConnected"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        protected MessageBusConnected(
            IComponentryContainer container,
            IMessageBusAdapter messageBusAdapter)
            : base(container)
        {
            this.messageBusAdapter = messageBusAdapter;

            this.RegisterHandler<IEnvelope>(this.Open);
        }

        /// <summary>
        /// Subscribe to the given message type with the message bus.
        /// </summary>
        /// <typeparam name="T">The message type to subscribe to.</typeparam>
        protected void Subscribe<T>()
            where T : Message
        {
            this.messageBusAdapter.Subscribe<T>(
                this.Endpoint,
                this.NewGuid(),
                this.TimeNow());
        }

        /// <summary>
        /// Unsubscribe from the given message type with the message bus.
        /// </summary>
        /// <typeparam name="T">The message type to unsubscribe from.</typeparam>
        protected void Unsubscribe<T>()
            where T : Message
        {
            this.messageBusAdapter.Unsubscribe<T>(
                this.Endpoint,
                this.NewGuid(),
                this.TimeNow());
        }

        /// <summary>
        /// Sends the given message to the given receiver via the message bus.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="receiver">The message receiver.</param>
        /// <typeparam name="T">The message type.</typeparam>
        protected void Send<T>(T message, Address receiver)
            where T : Message
        {
            this.messageBusAdapter.Send(
                message,
                receiver,
                this.Address,
                this.TimeNow());
        }

        /// <summary>
        /// Sends the given message to all given receivers via the message bus.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="receivers">The message receivers.</param>
        /// <typeparam name="T">The message type.</typeparam>
        protected void SendAll<T>(T message, List<Address> receivers)
            where T : Message
        {
            for (var i = 0; i < receivers.Count; i++)
            {
                this.messageBusAdapter.Send(
                    message,
                    receivers[i],
                    this.Address,
                    this.TimeNow());
            }
        }

        /// <summary>
        /// Sends the given message to the message bus to be published.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="message">The message.</param>
        protected void SendToBus<T>(T message)
            where T : Message
        {
            this.messageBusAdapter.SendToBus(message, this.Address, this.TimeNow());
        }
    }
}
