// -------------------------------------------------------------------------------------------------
// <copyright file="MessageServer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using Akka.Actor;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Factories;
    using Nautilus.Messaging.Network;

    /// <summary>
    /// Provides a messaging server using the ZeroMQ protocol.
    /// </summary>
    [PerformanceOptimized]
    public class MessageServer : ActorComponentBusConnectedBase
    {
        private readonly IEndpoint commandConsumer;
        private readonly IEndpoint eventPublisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageServer"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="commandSerializer">The command serializer.</param>
        /// <param name="eventSerializer">The event serializer.</param>
        /// <param name="serverAddress">The server address.</param>
        /// <param name="commandsPort">The commands port.</param>
        /// <param name="eventsPort">The events port.</param>
        public MessageServer(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            ICommandSerializer commandSerializer,
            IEventSerializer eventSerializer,
            NetworkAddress serverAddress,
            Port commandsPort,
            Port eventsPort)
            : base(
                NautilusService.Messaging,
                LabelFactory.Create(nameof(MessageServer)),
                container,
                messagingAdapter)
        {
            Precondition.NotNull(container, nameof(container));
            Precondition.NotNull(messagingAdapter, nameof(messagingAdapter));
            Precondition.NotNull(commandSerializer, nameof(commandSerializer));
            Precondition.NotNull(eventSerializer, nameof(eventSerializer));
            Precondition.NotNull(serverAddress, nameof(serverAddress));
            Precondition.NotNull(commandsPort, nameof(commandsPort));
            Precondition.NotNull(eventsPort, nameof(eventsPort));

            this.commandConsumer = new ActorEndpoint(
                Context.ActorOf(Props.Create(
                    () => new CommandConsumer(
                        container,
                        commandSerializer,
                        new ActorEndpoint(Context.Self),
                        serverAddress,
                        commandsPort))));

            this.eventPublisher = new ActorEndpoint(
                Context.ActorOf(Props.Create(
                    () => new EventPublisher(
                        container,
                        eventSerializer,
                        serverAddress,
                        eventsPort))));

            // Command messages.
            this.Receive<SubmitOrder>(this.OnMessage);
            this.Receive<CancelOrder>(this.OnMessage);
            this.Receive<ModifyOrder>(this.OnMessage);
            this.Receive<CollateralInquiry>(this.OnMessage);

            // Event messages.
            this.Receive<Event>(this.OnMessage);
        }

        /// <summary>
        /// Actions to be performed when the component is stopping.
        /// </summary>
        protected override void PostStop()
        {
            this.commandConsumer.Send(new ShutdownSystem(this.NewGuid(), this.TimeNow()));
            this.eventPublisher.Send(new ShutdownSystem(this.NewGuid(), this.TimeNow()));
            base.PostStop();
        }

        private void OnMessage(SubmitOrder message)
        {
            Debug.NotNull(message, nameof(message));

            this.Send(ExecutionServiceAddress.OrderManager, message);
        }

        private void OnMessage(CancelOrder message)
        {
            this.Send(ExecutionServiceAddress.OrderManager, message);
        }

        private void OnMessage(ModifyOrder message)
        {
            this.Send(ExecutionServiceAddress.OrderManager, message);
        }

        private void OnMessage(CollateralInquiry message)
        {
            this.Send(ServiceAddress.Execution, message);
        }

        private void OnMessage(Event @event)
        {
            Debug.NotNull(@event, nameof(@event));

            this.eventPublisher.Send(@event);
            this.Log.Debug($"Published event {@event}.");
        }
    }
}
