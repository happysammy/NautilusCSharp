//--------------------------------------------------------------------------------------------------
// <copyright file="IMessagingAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.Core;
    using Nautilus.Messaging;

    /// <summary>
    /// Provides a means for components to send messages to other components via the message bus.
    /// </summary>
    public interface IMessagingAdapter
    {
        /// <summary>
        /// Sends the given message to the given receiver in an <see cref="Envelope{T}"/> marked
        /// from the given sender.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="receiver">The receiver address.</param>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender address.</param>
        void Send<T>(Address receiver, T message, Address sender)
            where T : Message;
    }
}
