// -------------------------------------------------------------------------------------------------
// <copyright file="RabbitMQServer.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.RabbitMQ
{
    using System;
    using Akka.Actor;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.Core.Validation;
    using global::RabbitMQ.Client;
    using global::RabbitMQ.Client.Events;

    /// <summary>
    /// Provides a RabbitMQ message broker implementation.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class RabbitMQServer : ActorComponentBusConnectedBase
    {
        private readonly ICommandSerializer commandSerializer;
        private readonly IEventSerializer eventSerializer;
        private readonly IConnection commandConnection;
        private readonly IConnection eventConnection;

        public RabbitMQServer(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            ICommandSerializer commandSerializer,
            IEventSerializer eventSerializer,
            IConnection commandConnection,
            IConnection eventConnection)
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
            Validate.NotNull(commandConnection, nameof(commandConnection));
            Validate.NotNull(eventConnection, nameof(eventConnection));

            this.commandSerializer = commandSerializer;
            this.eventSerializer = eventSerializer;
            this.commandConnection = commandConnection;
            this.eventConnection = eventConnection;

            try
            {
                using (var commandChannel = this.commandConnection.CreateModel())
                {
                    commandChannel.ExchangeDeclare(
                        RabbitConstants.ExecutionCommandsExchange,
                        ExchangeType.Direct,
                        durable: true,
                        autoDelete: false);
                    this.Log.Information($"Exchange {RabbitConstants.ExecutionCommandsExchange} declared.");

                    commandChannel.QueueDeclare(
                        RabbitConstants.InvTraderCommandsQueue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false);
                    this.Log.Information($"Queue {RabbitConstants.InvTraderCommandsQueue} declared.");

                    commandChannel.QueueBind(
                        RabbitConstants.InvTraderCommandsQueue,
                        RabbitConstants.ExecutionCommandsExchange,
                        RabbitConstants.InvTraderCommandsQueue);
                    this.Log.Information($"Queue {RabbitConstants.InvTraderCommandsQueue} bound to {RabbitConstants.ExecutionCommandsExchange}.");

                    var consumer = new EventingBasicConsumer(commandChannel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var command = this.commandSerializer.Deserialize(body);

                        this.Send(NautilusService.Execution, command);
                    };
                    this.Log.Information($"Basic event consumer created.");
                }

                using (var eventChannel = this.eventConnection.CreateModel())
                {
                    eventChannel.ExchangeDeclare(
                        RabbitConstants.ExecutionEventsExchange,
                        ExchangeType.Fanout,
                        durable: true,
                        autoDelete: false);
                    this.Log.Information($"Exchange {RabbitConstants.ExecutionEventsExchange} declared.");

                    eventChannel.QueueDeclare(
                        RabbitConstants.InvTraderEventsQueue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false);
                    this.Log.Information($"Queue {RabbitConstants.InvTraderEventsQueue} declared.");

                    eventChannel.QueueBind(
                        RabbitConstants.InvTraderEventsQueue,
                        RabbitConstants.ExecutionEventsExchange,
                        RabbitConstants.InvTraderEventsQueue);
                    this.Log.Information($"Queue {RabbitConstants.InvTraderEventsQueue} bound to {RabbitConstants.ExecutionEventsExchange}.");
                }
            }
            catch (Exception ex)
            {
                this.Log.Error($"Error {ex.Message}", ex);
                throw ex;
            }

            // Event messages
            this.Receive<OrderEvent>(msg => this.OnMessage(msg, this.Sender));
        }

        private void OnMessage(OrderEvent @event, IActorRef sender)
        {
            Debug.NotNull(@event, nameof(@event));
            Debug.NotNull(sender, nameof(sender));

            this.Log.Debug($"Event {@event} received.");

            using (var channel = this.eventConnection.CreateModel())
            {
                channel.BasicPublish(
                    RabbitConstants.ExecutionEventsExchange,
                    RabbitConstants.InvTraderEventsQueue,
                    mandatory: false,
                    basicProperties: null,
                    body: this.eventSerializer.Serialize(@event));

                this.Log.Debug($"Published event {@event}.");
            }
        }
    }
}
