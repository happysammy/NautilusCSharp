//--------------------------------------------------------------------------------------------------
// <copyright file="IMessageBusAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System;
    using Nautilus.Core;
    using Nautilus.Messaging;
    using NodaTime;

    /// <summary>
    /// Provides a means for components to send messages to other components via the message bus.
    /// </summary>
    public interface IMessageBusAdapter
    {
        /// <summary>
        /// Subscribe the given subscriber to the given message type with the message bus.
        /// </summary>
        /// <typeparam name="T">The message type to subscribe to.</typeparam>
        /// <param name="subscriber">The subscriber mailbox.</param>
        /// <param name="id">The subscription identifier.</param>
        /// <param name="timestamp">The subscription timestamp.</param>
        void Subscribe<T>(Mailbox subscriber, Guid id, ZonedDateTime timestamp)
            where T : Message;

        /// <summary>
        /// Unsubscribe the given subscriber from the given message type with the message bus.
        /// </summary>
        /// <typeparam name="T">The message type to unsubscribe from.</typeparam>
        /// <param name="subscriber">The subscriber mailbox.</param>
        /// <param name="id">The subscription identifier.</param>
        /// <param name="timestamp">The subscription timestamp.</param>
        void Unsubscribe<T>(Mailbox subscriber, Guid id, ZonedDateTime timestamp)
            where T : Message;

        /// <summary>
        /// Sends the given message to the given receiver in an <see cref="Envelope{T}"/> marked
        /// from the given sender.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="receiver">The receiver address.</param>
        /// <param name="sender">The sender address.</param>
        /// <param name="timestamp">The send timestamp.</param>
        void Send<T>(T message, Address receiver, Address sender, ZonedDateTime timestamp)
            where T : Message;

        /// <summary>
        /// Sends the given message to the message bus to be published marked from the given sender.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender address.</param>
        /// <param name="timestamp">The send timestamp.</param>
        void SendToBus<T>(T message, Address? sender, ZonedDateTime timestamp)
            where T : Message;
    }
}
