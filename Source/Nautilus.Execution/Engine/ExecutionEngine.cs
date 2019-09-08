// -------------------------------------------------------------------------------------------------
// <copyright file="ExecutionEngine.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Engine
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Interfaces;
    using Nautilus.Execution.Messages.Commands;
    using Nautilus.Messaging.Interfaces;

    /// <summary>
    /// Provides a generic execution engine utilizing an abstract execution database.
    /// </summary>
    public class ExecutionEngine : MessageBusConnected
    {
        private readonly IExecutionDatabase database;
        private readonly ITradingGateway gateway;
        private readonly IEndpoint eventPublisher;

        private readonly Dictionary<OrderId, ModifyOrder> bufferModify;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionEngine"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="messagingAdapter">The message bus adapter.</param>
        /// <param name="database">The execution database.</param>
        /// <param name="gateway">The trading gateway.</param>
        /// <param name="eventPublisher">The event publisher endpoint.</param>
        public ExecutionEngine(
            IComponentryContainer container,
            IMessageBusAdapter messagingAdapter,
            IExecutionDatabase database,
            ITradingGateway gateway,
            IEndpoint eventPublisher)
            : base(container, messagingAdapter)
        {
            this.database = database;
            this.gateway = gateway;
            this.eventPublisher = eventPublisher;

            this.bufferModify = new Dictionary<OrderId, ModifyOrder>();

            // Commands
            this.RegisterHandler<SubmitOrder>(this.OnMessage);
            this.RegisterHandler<SubmitAtomicOrder>(this.OnMessage);
            this.RegisterHandler<CancelOrder>(this.OnMessage);
            this.RegisterHandler<ModifyOrder>(this.OnMessage);
            this.RegisterHandler<AccountInquiry>(this.OnMessage);

            // Events
            this.RegisterHandler<OrderSubmitted>(this.OnMessage);
            this.RegisterHandler<OrderAccepted>(this.OnMessage);
            this.RegisterHandler<OrderRejected>(this.OnMessage);
            this.RegisterHandler<OrderWorking>(this.OnMessage);
            this.RegisterHandler<OrderModified>(this.OnMessage);
            this.RegisterHandler<OrderCancelReject>(this.OnMessage);
            this.RegisterHandler<OrderExpired>(this.OnMessage);
            this.RegisterHandler<OrderCancelled>(this.OnMessage);
            this.RegisterHandler<OrderPartiallyFilled>(this.OnMessage);
            this.RegisterHandler<OrderFilled>(this.OnMessage);
            this.RegisterHandler<AccountStateEvent>(this.OnMessage);

            // Order Events
            this.Subscribe<OrderSubmitted>();
            this.Subscribe<OrderAccepted>();
            this.Subscribe<OrderRejected>();
            this.Subscribe<OrderWorking>();
            this.Subscribe<OrderModified>();
            this.Subscribe<OrderCancelReject>();
            this.Subscribe<OrderExpired>();
            this.Subscribe<OrderCancelled>();
            this.Subscribe<OrderPartiallyFilled>();
            this.Subscribe<OrderFilled>();
            this.Subscribe<AccountStateEvent>();
        }

        /// <summary>
        /// Gets the count of commands executed.
        /// </summary>
        public int CommandCount { get; private set; }

        /// <summary>
        /// Gets the count of events handled.
        /// </summary>
        public int EventCount { get; private set; }

        //-- COMMANDS ------------------------------------------------------------------------------------------------//
        private void OnMessage(SubmitOrder command)
        {
            this.IncrementCounter(command);

            this.database.AddOrder(
                    command.Order,
                    command.TraderId,
                    command.AccountId,
                    command.StrategyId,
                    command.PositionId)
                .OnSuccess(() => this.gateway.SubmitOrder(command.Order))
                .OnFailure(result => this.Log.Error($"Cannot execute {command} {result.Message}"));
        }

        private void OnMessage(SubmitAtomicOrder command)
        {
            this.IncrementCounter(command);

            this.database.AddAtomicOrder(
                command.AtomicOrder,
                command.TraderId,
                command.AccountId,
                command.StrategyId,
                command.PositionId)
                .OnSuccess(() => this.gateway.SubmitOrder(command.AtomicOrder))
                .OnFailure(result => this.Log.Error($"Cannot execute {command} {result.Message}"));
        }

        private void OnMessage(CancelOrder command)
        {
            this.IncrementCounter(command);

            var order = this.database.GetOrder(command.OrderId);
            if (order is null)
            {
                this.Log.Error($"Cannot execute {command} the {command.OrderId} was not found in the memory cache.");
                return;
            }

            this.gateway.CancelOrder(order);
        }

        private void OnMessage(ModifyOrder command)
        {
            this.IncrementCounter(command);

            var order = this.database.GetOrder(command.OrderId);
            if (order is null)
            {
                this.Log.Error($"Cannot execute {command} the {command.OrderId} was not found in the memory cache.");
                return;
            }

            if (!order.IsWorking)
            {
                this.bufferModify[command.OrderId] = command;
                this.Log.Warning($"Buffering {command} as not yet working.");
                return;
            }

            if (this.bufferModify.ContainsKey(command.OrderId))
            {
                this.bufferModify[command.OrderId] = command;
                this.Log.Debug($"Buffering {command} as order already being modified.");
                return;
            }

            // Buffer the command to check in later processing
            this.bufferModify[command.OrderId] = command;
            this.gateway.ModifyOrder(order, command.ModifiedPrice);
        }

        private void OnMessage(AccountInquiry command)
        {
            this.IncrementCounter(command);

            this.gateway.AccountInquiry();
        }

        //-- EVENTS --------------------------------------------------------------------------------------------------//
        private void OnMessage(OrderSubmitted @event)
        {
            this.IncrementCounter(@event);

            this.ProcessOrderEvent(@event);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderAccepted @event)
        {
            this.IncrementCounter(@event);

            this.ProcessOrderEvent(@event);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderRejected @event)
        {
            this.IncrementCounter(@event);

            this.ProcessOrderEvent(@event);
            this.ClearModifyBuffer(@event.OrderId);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderWorking @event)
        {
            this.IncrementCounter(@event);

            var order = this.ProcessOrderEvent(@event);
            if (order != null)
            {
                this.ProcessModifyBuffer(order);
            }

            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderModified @event)
        {
            this.IncrementCounter(@event);

            var order = this.ProcessOrderEvent(@event);
            if (order != null)
            {
                this.ProcessModifyBuffer(order);
            }

            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderCancelReject @event)
        {
            this.IncrementCounter(@event);

            var order = this.ProcessOrderEvent(@event);
            if (order != null)
            {
                this.ProcessModifyBuffer(order);
            }

            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderExpired @event)
        {
            this.IncrementCounter(@event);

            this.ProcessOrderEvent(@event);
            this.ClearModifyBuffer(@event.OrderId);

            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderCancelled @event)
        {
            this.IncrementCounter(@event);

            this.ProcessOrderEvent(@event);
            this.ClearModifyBuffer(@event.OrderId);

            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderPartiallyFilled @event)
        {
            this.IncrementCounter(@event);

            this.ProcessOrderEvent(@event);
            this.HandleOrderFillEvent(@event);

            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderFilled @event)
        {
            this.IncrementCounter(@event);

            this.ProcessOrderEvent(@event);
            this.HandleOrderFillEvent(@event);
            this.ClearModifyBuffer(@event.OrderId);

            this.SendToEventPublisher(@event);
        }

        private void OnMessage(AccountStateEvent @event)
        {
            this.IncrementCounter(@event);

            var account = this.database.GetAccount(@event.AccountId);
            if (account is null)
            {
                account = new Account(@event);
                this.database.AddAccount(account);
            }
            else
            {
                account.Apply(@event);
                this.database.UpdateAccount(account);
            }

            this.SendToEventPublisher(@event);
        }

        private void IncrementCounter(Command command)
        {
            this.Log.Debug($"Received {command}.");
            this.CommandCount++;
        }

        private void IncrementCounter(Event @event)
        {
            this.Log.Debug($"Received {@event}.");
            this.EventCount++;
        }

        private Order? ProcessOrderEvent(OrderEvent @event)
        {
            var order = this.database.GetOrder(@event.OrderId);
            if (order is null)
            {
                // This should never happen
                this.Log.Error($"Cannot process event {@event} ({@event.OrderId} was not found in the cache).");
            }
            else
            {
                try
                {
                    order.Apply(@event);
                    this.database.UpdateOrder(order);
                }
                catch (Exception ex)
                {
                    this.Log.Error(ex.Message);
                    return null;
                }
            }

            return order;
        }

        private void HandleOrderFillEvent(OrderFillEvent @event)
        {
            var positionId = this.database.GetPositionId(@event.OrderId);
            if (positionId is null)
            {
                this.Log.Error($"Cannot process event {@event} (no PositionId found for {@event.OrderId}).");
                this.SendToEventPublisher(@event);
                return;
            }

            var position = this.database.GetPosition(positionId);
            if (position is null)
            {
                // Position does not exist - create new position
                position = new Position(positionId, @event);
                this.database.AddPosition(position).OnFailure(msg => this.Log.Error(msg));
            }
            else
            {
                position.Apply(@event);
                this.database.UpdatePosition(position);
            }
        }

        private void ProcessModifyBuffer(Order order)
        {
            if (this.bufferModify.TryGetValue(order.Id, out var modifyOrder))
            {
                if (!(order.Price is null) && order.Price != modifyOrder.ModifiedPrice)
                {
                    this.gateway.ModifyOrder(order, modifyOrder.ModifiedPrice);
                    this.Log.Debug($"Sent {modifyOrder} to TradingGateway.");
                }

                this.bufferModify.Remove(order.Id);
                this.Log.Debug($"Cleared {modifyOrder} from buffer.");
            }
        }

        private void ClearModifyBuffer(OrderId orderId)
        {
            if (this.bufferModify.ContainsKey(orderId))
            {
                this.bufferModify.Remove(orderId);
            }
        }

        private void SendToEventPublisher(OrderEvent @event)
        {
            var traderId = this.database.GetTraderId(@event.OrderId);
            if (traderId is null)
            {
                this.Log.Error($"Cannot send event {@event} to publisher (cannot find TraderId).");
                return;
            }

            this.SendToEventPublisher(new TradeEvent(traderId, @event));
        }

        private void SendToEventPublisher(Event @event)
        {
            this.eventPublisher.Send(@event);
            this.Log.Debug($"Sent {@event} to EventPublisher.");
        }
    }
}
