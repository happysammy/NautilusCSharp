// -------------------------------------------------------------------------------------------------
// <copyright file="MessageBusFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Message;

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
                new MessageBus<Command>(container),
                new MessageBus<Event>(container),
                new MessageBus<Document>(container));
        }
    }
}
