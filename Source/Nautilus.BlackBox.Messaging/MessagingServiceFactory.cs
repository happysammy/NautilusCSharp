// -------------------------------------------------------------------------------------------------
// <copyright file="MessagingServiceFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Messaging
{
    using Akka.Actor;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Messaging.MessageStore;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging.Base;

    /// <summary>
    /// The immutable sealed <see cref="MessagingServiceFactory"/>
    /// </summary>
    [Immutable]
    public sealed class MessagingServiceFactory : IMessagingServiceFactory
    {
        /// <summary>
        /// Creates a new message service and returns its <see cref="IMessagingAdapter"/> interface.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="clock">The clock.</param>
        /// <param name="loggerFactory">The logging adapter.</param>
        /// <returns>A <see cref="IMessagingAdapter"/>.</returns>
        /// <exception cref="ValidationException">Throws if any class argument is null.</exception>
        public IMessagingAdapter Create(
            ActorSystem actorSystem,
            BlackBoxEnvironment environment,
            IZonedClock clock,
            ILoggerFactory loggerFactory)
        {
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(clock, nameof(clock));
            Validate.NotNull(loggerFactory, nameof(loggerFactory));

            var messageStoreRef = actorSystem.ActorOf(Props.Create(() => new MessageStorer(new MessageWarehouse())));

            var commandBusRef = actorSystem.ActorOf(Props.Create(() => new MessageBus<CommandMessage>(
                new Label(BlackBoxService.CommandBus.ToString()),
                environment,
                clock,
                loggerFactory,
                messageStoreRef)));

            var eventBusRef = actorSystem.ActorOf(Props.Create(() => new MessageBus<EventMessage>(
                new Label(BlackBoxService.EventBus.ToString()),
                environment,
                clock,
                loggerFactory,
                messageStoreRef)));

            var documentBusRef = actorSystem.ActorOf(Props.Create(() => new MessageBus<DocumentMessage>(
                new Label(BlackBoxService.ServiceBus.ToString()),
                environment,
                clock,
                loggerFactory,
                messageStoreRef)));

           return new MessagingAdapter(commandBusRef, eventBusRef, documentBusRef);
        }
    }
}
