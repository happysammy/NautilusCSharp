// -------------------------------------------------------------------------------------------------
// <copyright file="RabbitMQServer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.RabbitMQ
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using global::RabbitMQ.Client;
    using Nautilus.Common.Commands;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// Provides a RabbitMQ message broker implementation.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression OK.")]
    [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable", Justification = "Reviewed. Suppression OK.")]
    [PerformanceOptimized]
    public class RabbitMQServer : ActorComponentBusConnectedBase
    {
        private readonly ICommandSerializer commandSerializer;
        private readonly IEventSerializer eventSerializer;
        private readonly IConnection commandConnection;
        private readonly IConnection eventConnection;
        private readonly IModel commandChannel;
        private readonly IModel eventChannel;
        private readonly IBasicProperties eventProps;
        private readonly CommandConsumer commandConsumer;
        private readonly List<Order> orders;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMQServer"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="commandSerializer">The command serializer.</param>
        /// <param name="eventSerializer">The event serializer.</param>
        /// <param name="commandConnection">The command connection.</param>
        /// <param name="eventConnection">The event connection.</param>
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
            this.orders = new List<Order>();

            // Setup message handling.
            this.Receive<SubmitOrder>(msg => this.OnMessage(msg));
            this.Receive<CancelOrder>(msg => this.OnMessage(msg));
            this.Receive<ModifyOrder>(msg => this.OnMessage(msg));
            this.Receive<Event>(msg => this.OnMessage(msg));

            try
            {
                this.commandChannel = this.commandConnection.CreateModel();
                this.commandChannel.ExchangeDeclare(
                    RabbitConstants.ExecutionCommandsExchange,
                    ExchangeType.Direct,
                    durable: true,
                    autoDelete: false,
                    arguments: null);
                this.Log.Information($"Exchange {RabbitConstants.ExecutionCommandsExchange} declared.");

                this.commandChannel.QueueDeclare(
                    RabbitConstants.InvTraderCommandsQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                this.Log.Information($"Queue {RabbitConstants.InvTraderCommandsQueue} declared.");

                this.commandChannel.QueueBind(
                    RabbitConstants.InvTraderCommandsQueue,
                    RabbitConstants.ExecutionCommandsExchange,
                    RabbitConstants.InvTraderCommandsQueue);
                this.Log.Information($"Queue {RabbitConstants.InvTraderCommandsQueue} bound to {RabbitConstants.ExecutionCommandsExchange}.");

                this.commandChannel.ConfirmSelect();

                this.commandConsumer = new CommandConsumer(
                        container,
                        commandSerializer,
                        this.commandChannel,
                        new ActorEndpoint(Context.Self));

                Task.Run(() => this.commandChannel.BasicConsume(
                    queue: RabbitConstants.InvTraderCommandsQueue,
                    autoAck: false,
                    consumerTag: "NautilusExecutor",
                    consumer: this.commandConsumer));

                this.Log.Information($"Command consumer created.");

                this.eventChannel = this.eventConnection.CreateModel();
                this.eventChannel.ExchangeDeclare(
                    RabbitConstants.ExecutionEventsExchange,
                    ExchangeType.Fanout,
                    durable: true,
                    autoDelete: false,
                    arguments: null);
                this.Log.Information($"Exchange {RabbitConstants.ExecutionEventsExchange} declared.");

                this.eventChannel.QueueDeclare(
                    RabbitConstants.InvTraderEventsQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);
                this.Log.Information($"Queue {RabbitConstants.InvTraderEventsQueue} declared.");

                this.eventChannel.QueueBind(
                    RabbitConstants.InvTraderEventsQueue,
                    RabbitConstants.ExecutionEventsExchange,
                    RabbitConstants.InvTraderEventsQueue);
                this.Log.Information($"Queue {RabbitConstants.InvTraderEventsQueue} bound to {RabbitConstants.ExecutionEventsExchange}.");

                this.eventProps = this.eventChannel.CreateBasicProperties();
                this.eventProps.AppId = "NautilusExecutor";
            }
            catch (Exception ex)
            {
                this.Log.Error($"Error {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Actions to be performed prior to stopping the <see cref="RabbitMQServer"/>.
        /// </summary>
        protected override void PostStop()
        {
            this.commandChannel.Dispose();
            this.Log.Information("Disposed of command channel.");
            this.eventChannel.Dispose();
            this.Log.Information("Disposed of event channel.");

            this.commandConnection.Dispose();
            this.Log.Information("Disposed of command connection.");
            this.eventConnection.Dispose();
            this.Log.Information("Disposed of event connection.");

            this.Log.Information("Disposed of command channel.");
        }

        private void OnMessage(SubmitOrder message)
        {
            Debug.NotNull(message, nameof(message));

            var orderToSubmit = message.Order;
            var orderToAdd = new Order(
                orderToSubmit.Symbol,
                orderToSubmit.Id,
                orderToSubmit.Label,
                orderToSubmit.Side,
                orderToSubmit.Type,
                orderToSubmit.Quantity,
                orderToSubmit.Price,
                orderToSubmit.TimeInForce,
                orderToSubmit.ExpireTime,
                orderToSubmit.Timestamp);

            this.orders.Add(orderToAdd);
            this.Log.Debug($"Order {orderToAdd.Id} added to order list.");

            this.Send(NautilusService.Execution, message);
        }

        private void OnMessage(CancelOrder message)
        {
            var order = this.orders.FirstOrDefault(o => o.Id.Equals(message.Order.Id));

            if (order is null)
            {
                this.Log.Warning(
                    $"Order not found for CancelOrder (command order_id={message.Order.Id}).");
                return;
            }

            var cancelOrder = new CancelOrder(
                order,
                message.Reason,
                message.Id,
                message.Timestamp);

            this.Send(NautilusService.Execution, cancelOrder);
        }

        private void OnMessage(ModifyOrder message)
        {
            var order = this.orders.FirstOrDefault(o => o.Id.Equals(message.Order.Id));

            if (order is null)
            {
                this.Log.Warning(
                    $"Order not found for ModifyOrder (command order_id={message.Order.Id}).");
                return;
            }

            var modifyOrder = new ModifyOrder(
                order,
                message.ModifiedPrice,
                message.Id,
                message.Timestamp);

            this.Send(NautilusService.Execution, modifyOrder);
        }

        private void OnMessage(Event @event)
        {
            Debug.NotNull(@event, nameof(@event));

            this.Log.Debug($"Event {@event} received.");

            if (@event is OrderEvent orderEvent)
            {
                var order = this.orders.FirstOrDefault(o => o.Id == orderEvent.OrderId);

                if (order is null)
                {
                    this.Log.Warning($"Order not found for OrderEvent (event order_id={orderEvent.Id}).");
                    return;
                }

                order.Apply(orderEvent);
                this.Log.Debug($"Applied {@event} to {order.Id}.");
            }

            this.eventChannel.BasicPublish(
                RabbitConstants.ExecutionEventsExchange,
                RabbitConstants.InvTraderEventsQueue,
                mandatory: false,
                basicProperties: this.eventProps,
                body: this.eventSerializer.Serialize(@event));

            this.Log.Debug($"Published event {@event}.");
        }
    }
}
