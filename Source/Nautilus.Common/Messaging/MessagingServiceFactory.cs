// -------------------------------------------------------------------------------------------------
// <copyright file="MessagingServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.MessageStore;
    using Nautilus.Core;

    /// <summary>
    /// Provides a factory to create the systems messaging service.
    /// </summary>
    public static class MessagingServiceFactory
    {
        /// <summary>
        /// Creates a new message service and returns its <see cref="IMessagingAdapter"/> interface.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="store">The message store.</param>
        /// <returns>A <see cref="IMessagingAdapter"/>.</returns>
        public static MessagingAdapter Create(IComponentryContainer container, IMessageStore store)
        {
            var messageStorer = new MessageStorer(container, store);

            var commandBus = new MessageBus<Command>(container, messageStorer.Endpoint);
            var eventBus = new MessageBus<Event>(container, messageStorer.Endpoint);
            var documentBus = new MessageBus<Document>(container, messageStorer.Endpoint);

            return new MessagingAdapter(
                commandBus.Endpoint,
                eventBus.Endpoint,
                documentBus.Endpoint);
        }
    }
}
