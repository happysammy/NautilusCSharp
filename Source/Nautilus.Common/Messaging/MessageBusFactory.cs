// -------------------------------------------------------------------------------------------------
// <copyright file="MessageBusFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;

    /// <summary>
    /// Provides a factory to create the systems message bus.
    /// </summary>
    public static class MessageBusFactory
    {
        /// <summary>
        /// Creates and returns a new message bus adapter.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <returns>The messaging adapter.</returns>
        public static MessageBusAdapter Create(IComponentryContainer container)
        {
            return new MessageBusAdapter(
                new MessageBus<Command>(container).Endpoint,
                new MessageBus<Event>(container).Endpoint,
                new MessageBus<Document>(container).Endpoint);
        }
    }
}
