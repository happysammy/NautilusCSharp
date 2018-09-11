// -------------------------------------------------------------------------------------------------
// <copyright file="MessageServer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
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
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Messaging.Network;

    /// <summary>
    /// Provides a messaging server using the ZeroMQ protocol.
    /// </summary>
    [PerformanceOptimized]
    public class MessageServer : ActorComponentBusConnectedBase
    {
        private readonly IEndpoint commandConsumer;
        private readonly IEndpoint eventPublisher;
        private readonly List<Order> orders;
        private readonly Dictionary<OrderId, List<ModifyOrder>> modifyCache;

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

            this.orders = new List<Order>();
            this.modifyCache = new Dictionary<OrderId, List<ModifyOrder>>();

            // Setup message handling.
            this.Receive<SubmitOrder>(msg => this.OnMessage(msg));
            this.Receive<CancelOrder>(msg => this.OnMessage(msg));
            this.Receive<ModifyOrder>(msg => this.OnMessage(msg));
            this.Receive<CollateralInquiry>(msg => this.OnMessage(msg));
            this.Receive<Event>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Actions to be performed when the component is stopping.
        /// </summary>
        protected override void PostStop()
        {
            this.commandConsumer.Send(new SystemShutdown(this.NewGuid(), this.TimeNow()));
            this.eventPublisher.Send(new SystemShutdown(this.NewGuid(), this.TimeNow()));
            base.PostStop();
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

            if (orderToSubmit.Price.HasValue && !this.modifyCache.ContainsKey(orderToSubmit.Id))
            {
                // Buffer modification cache preemptively.
                this.modifyCache.Add(orderToSubmit.Id, new List<ModifyOrder>());
            }
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

            if (!this.modifyCache.ContainsKey(order.Id))
            {
                // No cache for order id (this should never happen).
                this.Log.Warning($"No modification cache found for {order.Id}.");
                return;
            }

            if (this.modifyCache[order.Id].Count == 0)
            {
                this.Send(NautilusService.Execution, modifyOrder);
                this.AddToCache(modifyOrder);
            }
            else
            {
                this.AddToCache(modifyOrder);
            }
        }

        private void OnMessage(CollateralInquiry message)
        {
            this.Send(NautilusService.Execution, message);
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

                if (order.IsComplete)
                {
                    if (this.modifyCache.ContainsKey(order.Id))
                    {
                        // Flush cache.
                        this.modifyCache.Remove(order.Id);
                        this.Log.Debug($"Order {order.Id} complete, cache flushed.");
                    }
                }

                if (@event is OrderModified orderModified)
                {
                    this.ProcessCache(order);
                }
            }

            this.eventPublisher.Send(@event);
            this.Log.Debug($"Published event {@event}.");
        }

        private void AddToCache(ModifyOrder modifyOrder)
        {
            Debug.NotNull(modifyOrder, nameof(modifyOrder));

            var orderId = modifyOrder.Order.Id;
            if (!this.modifyCache.ContainsKey(orderId))
            {
                // No cache for order id (this should never happen).
                this.Log.Warning($"Cannot process modification cache, no cache for {orderId}.");
                return;
            }

            // Any cached modify order command price equals the new modify order command price?
            if (this.modifyCache[orderId].Any(command => command.ModifiedPrice.Equals(modifyOrder.ModifiedPrice)))
            {
                // No need to cache.
                this.Log.Debug($"Duplicate price for {modifyOrder}, not cached.");
                return;
            }

            this.modifyCache[orderId].Add(modifyOrder);
            this.Log.Debug($"Added {modifyOrder} to cache.");
        }

        private void ProcessCache(Order order)
        {
            Debug.NotNull(order, nameof(order));

            if (!this.modifyCache.ContainsKey(order.Id))
            {
                // Cannot process - no cache for order id (this should never happen).
                this.Log.Warning($"Cannot process modification cache, no cache for {order.Id}.");
                return;
            }

            if (order.Price.HasNoValue)
            {
                // Cannot process - no price for order (this should never happen).
                this.Log.Warning($"Cannot process modification cache, no price {order.Id}.");
                return;
            }

            this.Log.Verbose($"Processing modify order cache...");

            foreach (var command in this.modifyCache[order.Id].ToList())
            {
                if (order.Price.Equals(command.ModifiedPrice))
                {
                    this.modifyCache[order.Id].Remove(command);
                    this.Log.Verbose($"Removed {command} from cache.");
                }
            }

            if (this.modifyCache[order.Id].Count > 0)
            {
                var command = this.modifyCache[order.Id][0];

                this.Send(NautilusService.Execution, command);
                this.Log.Debug($"Sent cached {command}.");
            }
        }
    }
}
