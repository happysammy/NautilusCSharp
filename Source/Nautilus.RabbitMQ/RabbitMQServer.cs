// -------------------------------------------------------------------------------------------------
// <copyright file="RabbitMQServer.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.RabbitMQ
{
    using Akka.Actor;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.Core.Validation;
    using global::RabbitMQ.Client;
    using global::RabbitMQ.Client.Events;
    using global::RabbitMQ.Client.Framing;

    /// <summary>
    /// Represents a RabbitMQ message broker.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class RabbitMQServer : ActorComponentBusConnectedBase
    {
        private readonly ICommandSerializer commandSerializer;
        private readonly IEventSerializer eventSerializer;
        private readonly IConnection connection;
        private readonly IModel commandChannel;
        private readonly IModel eventChannel;

        public RabbitMQServer(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            ICommandSerializer commandSerializer,
            IEventSerializer eventSerializer,
            IConnection connection)
            : base(
                NautilusService.Messaging,
                LabelFactory.Component(nameof(RabbitMQServer)),
                container,
                messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(commandSerializer, nameof(commandSerializer));
            Validate.NotNull(eventSerializer, nameof(eventSerializer));
            Validate.NotNull(connection, nameof(connection));

            this.commandSerializer = commandSerializer;
            this.eventSerializer = eventSerializer;
            this.connection = connection;

            using (this.commandChannel = this.connection.CreateModel())
            {
                this.commandChannel.ExchangeDeclare(
                    RabbitConstants.ExecutionCommandsExchange,
                    RabbitConstants.Direct,
                    durable: true,
                    autoDelete: false);

                this.commandChannel.QueueDeclare(
                    RabbitConstants.InvarianceTraderQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                var consumer = new EventingBasicConsumer(this.commandChannel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var command = this.commandSerializer.Deserialize(body);

                    this.Send(NautilusService.Execution, command);
                };
            }

            using (this.eventChannel = this.connection.CreateModel())
            {
                this.eventChannel.ExchangeDeclare(
                    RabbitConstants.ExecutionEventsExchange,
                    RabbitConstants.FanOut,
                    durable: true,
                    autoDelete: false);

                this.eventChannel.QueueDeclare(
                    RabbitConstants.InvarianceTraderQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);
            }

            // Event messages
            this.Receive<OrderEvent>(msg => this.OnMessage(msg, this.Sender));
        }

        private void OnMessage(OrderEvent @event, IActorRef sender)
        {
            Debug.NotNull(@event, nameof(@event));
            Debug.NotNull(sender, nameof(sender));

            using (var channel = this.eventChannel)
            {
                channel.BasicPublish(
                    RabbitConstants.ExecutionEventsExchange,
                    RabbitConstants.InvarianceTraderQueue,
                    mandatory: false,
                    basicProperties: null,
                    body: this.eventSerializer.Serialize(@event));
            }
        }
    }
}
