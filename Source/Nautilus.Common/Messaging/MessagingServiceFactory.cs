// -------------------------------------------------------------------------------------------------
// <copyright file="MessagingServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using Akka.Actor;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.MessageStore;
    using Nautilus.Core;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Provides a factory to create the systems messaging service.
    /// </summary>
    public static class MessagingServiceFactory
    {
        /// <summary>
        /// Creates a new message service and returns its <see cref="IMessagingAdapter"/> interface.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="container">The container.</param>
        /// <param name="store">The message store.</param>
        /// <returns>A <see cref="IMessagingAdapter"/>.</returns>
        public static MessagingAdapter Create(
            ActorSystem actorSystem,
            IComponentryContainer container,
            IMessageStore store)
        {
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(store, nameof(store));

            var messageStoreRef = new ActorEndpoint(actorSystem.ActorOf(Props.Create(() => new MessageStorer(store))));

            var commandBus = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(() => new MessageBus<Command>(
                container,
                messageStoreRef))));

            var eventBusRef = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(() => new MessageBus<Event>(
                container,
                messageStoreRef))));

            var serviceBusRef = new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(() => new MessageBus<Document>(
                container,
                messageStoreRef))));

            return new MessagingAdapter(commandBus, eventBusRef, serviceBusRef);
        }
    }
}
