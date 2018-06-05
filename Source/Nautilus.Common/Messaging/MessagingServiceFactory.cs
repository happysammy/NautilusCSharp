// -------------------------------------------------------------------------------------------------
// <copyright file="MessagingServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using Akka.Actor;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.MessageStore;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The immutable sealed <see cref="MessagingServiceFactory"/>
    /// </summary>
    [Immutable]
    public static class MessagingServiceFactory
    {
        /// <summary>
        /// Creates a new message service and returns its <see cref="IMessagingAdapter"/> interface.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="container">The container.</param>
        /// <returns>A <see cref="IMessagingAdapter"/>.</returns>
        /// <exception cref="ValidationException">Throws if any class argument is null.</exception>
        public static MessagingAdapter Create(
            ActorSystem actorSystem,
            IComponentryContainer container)
        {
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(container, nameof(container));

            var messageStoreRef = actorSystem.ActorOf(Props.Create(() => new MessageStorer(new InMemoryMessageStore())));

            var commandBusRef = actorSystem.ActorOf(Props.Create(() => new MessageBus<CommandMessage>(
                ServiceContext.Messaging,
                new Label(ServiceContext.CommandBus.ToString()),
                container,
                messageStoreRef)));

            var eventBusRef = actorSystem.ActorOf(Props.Create(() => new MessageBus<EventMessage>(
                ServiceContext.Messaging,
                new Label(ServiceContext.EventBus.ToString()),
                container,
                messageStoreRef)));

            var serviceBusRef = actorSystem.ActorOf(Props.Create(() => new MessageBus<DocumentMessage>(
                ServiceContext.Messaging,
                new Label(ServiceContext.DocumentBus.ToString()),
                container,
                messageStoreRef)));

           return new MessagingAdapter(commandBusRef, eventBusRef, serviceBusRef);
        }
    }
}
