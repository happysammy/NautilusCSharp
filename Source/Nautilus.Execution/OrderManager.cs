// -------------------------------------------------------------------------------------------------
// <copyright file="OrderManager.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Messages.Commands;

    /// <summary>
    /// Provides an <see cref="Order"/> manager.
    /// </summary>
    [PerformanceOptimized]
    public sealed class OrderManager : MessageBusConnected
    {
        private readonly IFixGateway gateway;
        private readonly OrderRegister register;
        private readonly Dictionary<OrderId, Order> orderBook;
        private readonly Dictionary<OrderId, Order> ordersActive;
        private readonly Dictionary<OrderId, Order> ordersCompleted;
        private readonly Dictionary<OrderId, List<ModifyOrder>> modifyCache;
        private readonly Dictionary<OrderId, ModifyOrder> modifyBuffer;
        private readonly Dictionary<OrderId, CancelOrder> cancelBuffer;

        private int commandCount;
        private int eventCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderManager"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="gateway">The FIX gateway.</param>
        public OrderManager(
            IComponentryContainer container,
            IMessageBusAdapter messageBusAdapter,
            IFixGateway gateway)
            : base(container, messageBusAdapter)
        {
            this.gateway = gateway;
            this.register = new OrderRegister(container);
            this.orderBook = new Dictionary<OrderId, Order>();
            this.ordersActive = new Dictionary<OrderId, Order>();
            this.ordersCompleted = new Dictionary<OrderId, Order>();
            this.modifyCache = new Dictionary<OrderId, List<ModifyOrder>>();
            this.modifyBuffer = new Dictionary<OrderId, ModifyOrder>();
            this.cancelBuffer = new Dictionary<OrderId, CancelOrder>();

            this.commandCount = 0;
            this.eventCount = 0;

            this.RegisterHandler<SubmitOrder>(this.OnMessage);
            this.RegisterHandler<SubmitAtomicOrder>(this.OnMessage);
            this.RegisterHandler<CancelOrder>(this.OnMessage);
            this.RegisterHandler<ModifyOrder>(this.OnMessage);
            this.RegisterHandler<Event>(this.OnMessage);
        }

        /// <summary>
        /// Gets the number of commands received by the order manager.
        /// </summary>
        public int CommandsCount => this.commandCount;

        /// <summary>
        /// Gets the number of events received by the order manager.
        /// </summary>
        public int EventsCount => this.eventCount;

        private void OnMessage(SubmitOrder message)
        {
            this.commandCount++;

            var order = message.Order;

            if (this.CancelOrderInBuffer(order.Id, message.ToString()))
            {
                return; // Warning logged.
            }

            if (this.OrderBookContainsId(order.Id, message.ToString()))
            {
                return; // Error logged.
            }

            this.register.Register(message);
            this.orderBook.Add(order.Id, order);
            this.CreateModifyCache(order);

            this.gateway.SubmitOrder(order);
            this.Log.Debug($"Sent cached {message} to FixGateway.");
        }

        private void OnMessage(SubmitAtomicOrder message)
        {
            this.commandCount++;

            var atomicOrder = message.AtomicOrder;
            var entryOrderId = atomicOrder.Entry.Id;

            if (this.CancelOrderInBuffer(entryOrderId, message.ToString()))
            {
                return; // Warning logged.
            }

            if (this.OrderBookContainsId(atomicOrder.Entry.Id, message.ToString()))
            {
                return; // Error logged.
            }

            if (this.OrderBookContainsId(atomicOrder.StopLoss.Id, message.ToString()))
            {
                return; // Error logged.
            }

            if (atomicOrder.TakeProfit != null &&
                this.OrderBookContainsId(atomicOrder.TakeProfit.Id, message.ToString()))
            {
                return; // Error logged.
            }

            this.register.Register(message);
            this.orderBook.Add(atomicOrder.Entry.Id, atomicOrder.Entry);
            this.orderBook.Add(atomicOrder.StopLoss.Id, atomicOrder.Entry);
            this.CreateModifyCache(atomicOrder.Entry);
            this.CreateModifyCache(atomicOrder.StopLoss);
            if (atomicOrder.TakeProfit != null)
            {
                this.orderBook.Add(atomicOrder.Entry.Id, atomicOrder.Entry);
                this.CreateModifyCache(atomicOrder.TakeProfit);
            }

            this.gateway.SubmitOrder(message.AtomicOrder);
            this.Log.Debug($"Sent cached {message} to FixGateway.");
        }

        private void OnMessage(CancelOrder message)
        {
            this.commandCount++;

            if (this.OrderBookDoesNotContainId(message.OrderId, message.ToString()))
            {
                this.cancelBuffer.Add(message.OrderId, message);
                return; // Error logged.
            }

            this.gateway.CancelOrder(this.orderBook[message.OrderId]);
            this.Log.Debug($"Sent cached {message} to FixGateway.");
        }

        private void OnMessage(ModifyOrder message)
        {
            this.commandCount++;

            if (this.OrderBookDoesNotContainId(message.OrderId, message.ToString()))
            {
                if (this.modifyBuffer.TryGetValue(message.OrderId, out var value))
                {
                    // Remove previously buffered ModifyOrder command.
                    this.modifyBuffer.Remove(message.OrderId);
                }

                this.modifyBuffer.Add(message.OrderId, message);
                return; // Error logged.
            }

            var order = this.orderBook[message.OrderId];

            if (this.modifyCache[order.Id].Count == 0)
            {
                this.gateway.ModifyOrder(order, message.ModifiedPrice);
                this.Log.Debug($"Sent cached {message} to FixGateway.");
            }

            this.AddToModifyCache(message);
        }

        private void OnMessage(Event @event)
        {
            this.eventCount++;

            if (@event is OrderEvent orderEvent)
            {
                if (this.OrderBookDoesNotContainId(orderEvent.OrderId, orderEvent.ToString()))
                {
                    return; // Error logged.
                }

                var order = this.orderBook[orderEvent.OrderId];
                order.Apply(orderEvent);

                this.Log.Debug($"Applied {@event} to {order.Id}.");

                if (orderEvent is OrderWorking working)
                {
                    if (this.modifyBuffer.ContainsKey(working.OrderId))
                    {
                        // Send previously buffered ModifyOrder command to gateway.
                        var buffered = this.modifyBuffer[working.OrderId];
                        this.gateway.ModifyOrder(order, buffered.ModifiedPrice);
                        this.modifyBuffer.Remove(working.OrderId);
                    }
                }

                if (order.IsActive)
                {
                    if (!this.ordersActive.ContainsKey(order.Id))
                    {
                        this.ordersActive.Add(order.Id, order);
                    }
                }

                if (order.IsComplete)
                {
                    if (!this.ordersCompleted.ContainsKey(order.Id))
                    {
                        this.ordersCompleted.Add(order.Id, order);
                    }

                    if (this.ordersActive.ContainsKey(order.Id))
                    {
                        this.ordersActive.Remove(order.Id);
                    }

                    if (this.modifyCache.ContainsKey(order.Id))
                    {
                        // Flush cache.
                        this.modifyCache.Remove(order.Id);
                        this.Log.Debug($"Order {order.Id} complete, cache flushed.");
                    }
                }

                switch (@event)
                {
                    case OrderModified _:
                        this.ProcessCache(order);
                        break;
                    case OrderRejected _:
                    case OrderCancelReject _:
                        this.Log.Warning(@event.ToString());
                        break;
                }
            }

            this.Send(@event, ExecutionServiceAddress.EventPublisher);
            this.Log.Debug($"Sent {@event} to EventServer.");
        }

        private void CreateModifyCache(Order order)
        {
            if (order.Price is null)
            {
                return; // No need to cache.
            }

            if (this.modifyCache.ContainsKey(order.Id))
            {
                this.Log.Error($"Cannot create modify cache for {order} (duplicate key already in cache).");
                return; // This should never happen.
            }

            this.modifyCache.Add(order.Id, new List<ModifyOrder>());
        }

        private void AddToModifyCache(ModifyOrder command)
        {
            var orderId = command.OrderId;
            if (!this.modifyCache.ContainsKey(orderId))
            {
                this.Log.Error($"Cannot add {command} to modify cache (no cache for {orderId}).");
                return; // This should never happen.
            }

            this.modifyCache[orderId].Add(command);
            this.Log.Debug($"Added {command} to cache.");
        }

        private void ProcessCache(Order order)
        {
            if (this.OrderBookDoesNotContainId(order.Id, "modify cache"))
            {
                return; // Error logged.
            }

            foreach (var command in this.modifyCache[order.Id].ToList())
            {
                if (order.Price is null)
                {
                    this.Log.Error($"Cannot process {command} ({order} price is null).");
                    continue;
                }

                if (order.Price.Equals(command.ModifiedPrice))
                {
                    this.modifyCache[order.Id].Remove(command);
                    this.Log.Verbose($"Removed {command} from cache.");
                }
            }

            if (this.modifyCache[order.Id].Count > 0)
            {
                var nextCached = this.modifyCache[order.Id][0];
                this.modifyCache[order.Id].RemoveAt(0);
                this.Log.Verbose($"Removed {nextCached} from cache.");

                this.gateway.ModifyOrder(order, nextCached.ModifiedPrice);
                this.Log.Debug($"Sent cached {nextCached} to FixGateway.");
            }
        }

        private bool OrderBookDoesNotContainId(OrderId id, string message)
        {
            Debug.NotEmptyOrWhiteSpace(message, nameof(message));

            if (!this.orderBook.ContainsKey(id))
            {
                this.Log.Error($"Cannot process {message} (order id not found in order book).");
                return true;
            }

            return false;
        }

        private bool OrderBookContainsId(OrderId id, string message)
        {
            Debug.NotEmptyOrWhiteSpace(message, nameof(message));

            if (this.orderBook.ContainsKey(id))
            {
                this.Log.Error($"Cannot process {message} (duplicate order id in order book).");
                return true;
            }

            return false;
        }

        private bool CancelOrderInBuffer(OrderId id, string message)
        {
            Debug.NotEmptyOrWhiteSpace(message, nameof(message));

            if (this.cancelBuffer.ContainsKey(id))
            {
                this.Log.Warning($"Cannot process {message} ({this.cancelBuffer[id]} already received).");
                this.cancelBuffer.Remove(id);
                return true;
            }

            return false;
        }
    }
}
