//--------------------------------------------------------------------------------------------------
// <copyright file="IMessagingAdapter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Collections;

    /// <summary>
    /// An adapter to facilitate service components sending messages to other service components via
    /// the messaging service.
    /// </summary>
    public interface IMessagingAdapter
    {
        /// <summary>
        /// Sends the given message to the given receiver marked from the given sender.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="receiver">The message receiver.</param>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        void Send<T>(Enum receiver, T message, Enum sender)
            where T : Message;

        /// <summary>
        /// Sends the given message to the given receivers marked from the given sender.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="receivers">The message receivers.</param>
        /// <param name="message">The message.</param>
        /// <param name="sender">The sender.</param>
        void Send<T>(ReadOnlyList<Enum> receivers, T message, Enum sender)
            where T : Message;
    }
}
