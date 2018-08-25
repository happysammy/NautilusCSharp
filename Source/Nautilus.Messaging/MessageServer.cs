// -------------------------------------------------------------------------------------------------
// <copyright file="MessageServer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging
{
    using System.Collections.Generic;
    using System.Linq;
    using Akka.Actor;
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
    /// Provides a messaging server using the ZeroMQ protocol.
    /// </summary>
    [PerformanceOptimized]
    public class MessageServer : ActorComponentBusConnectedBase
    {
        private readonly IEndpoint commandConsumer;
        private readonly IEndpoint eventPublisher;
        private readonly List<Order> orders;

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
            string serverAddress,
            int commandsPort,
            int eventsPort)
            : base(
                NautilusService.Messaging,
                LabelFactory.Component(nameof(MessageServer)),
                container,
                messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(commandSerializer, nameof(commandSerializer));
            Validate.NotNull(eventSerializer, nameof(eventSerializer));

            this.commandConsumer = new ActorEndpoint(
                Context.ActorOf(Props.Create(
                    () => new CommandConsumer(
                        container,
                        commandSerializer,
                        serverAddress,
                        commandsPort))));

            this.eventPublisher = new ActorEndpoint(
                Context.ActorOf(Props.Create(
                    () => new EventPublisher(
                        container,
                        eventSerializer,
                        serverAddress,
                        eventsPort))));

            this.orders = new List<Order>();
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

            this.eventPublisher.Send(@event);
            this.Log.Debug($"Published event {@event}.");
        }
    }
}
