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
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Commands;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Events.Base;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Execution.Interfaces;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Scheduler;
    using NodaTime;

    /// <summary>
    /// Provides a generic execution engine utilizing an abstract execution database.
    /// </summary>
    public class ExecutionEngine : MessageBusConnected
    {
        private const string SENT = "-->";
        private const string RECV = "<--";
        private const string CMD = "[CMD]";
        private const string EVT = "[EVT]";

        private readonly IScheduler scheduler;
        private readonly IExecutionDatabase database;
        private readonly ITradingGateway gateway;
        private readonly IEndpoint eventPublisher;

        private readonly Dictionary<OrderId, ModifyOrder> bufferModify;
        private readonly Dictionary<OrderId, ICancelable> gtdExpiryBackupTokens;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionEngine"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="messagingAdapter">The message bus adapter.</param>
        /// <param name="database">The execution database.</param>
        /// <param name="gateway">The trading gateway.</param>
        /// <param name="eventPublisher">The event publisher endpoint.</param>
        /// <param name="gtdExpiryBackups">The option flag for GTD order expiry cancel backups.</param>
        public ExecutionEngine(
            IComponentryContainer container,
            IScheduler scheduler,
            IMessageBusAdapter messagingAdapter,
            IExecutionDatabase database,
            ITradingGateway gateway,
            IEndpoint eventPublisher,
            bool gtdExpiryBackups = true)
            : base(container, messagingAdapter)
        {
            this.database = database;
            this.scheduler = scheduler;
            this.gateway = gateway;
            this.eventPublisher = eventPublisher;

            this.bufferModify = new Dictionary<OrderId, ModifyOrder>();
            this.gtdExpiryBackupTokens = new Dictionary<OrderId, ICancelable>();

            this.OptionGtdExpiryBackups = gtdExpiryBackups;

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

            this.Log.Information($"{nameof(this.OptionGtdExpiryBackups)} is {this.OptionGtdExpiryBackups}");
        }

        /// <summary>
        /// Gets a value indicating whether GTD order expiry cancel backups are turned on.
        /// </summary>
        public bool OptionGtdExpiryBackups { get; }

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
            this.CommandCount++;
            this.Log.Information($"{RECV}{CMD} {command}.");

            var result = this.database.AddOrder(
                command.Order,
                command.TraderId,
                command.AccountId,
                command.StrategyId,
                command.PositionId);

            if (result.IsSuccess)
            {
                var positionIdBroker = this.database.GetPositionIdBroker(command.PositionId);
                this.gateway.SubmitOrder(command.Order, positionIdBroker);

                var submitted = new OrderSubmitted(
                    command.AccountId,
                    command.Order.Id,
                    this.TimeNow(),
                    this.NewGuid(),
                    this.TimeNow());

                command.Order.Apply(submitted);
                this.database.UpdateOrder(command.Order);

                this.SendToEventPublisher(submitted);
            }
            else
            {
                this.Log.Error($"Cannot execute command {command} ({result.Message}).");
            }
        }

        private void OnMessage(SubmitAtomicOrder command)
        {
            this.CommandCount++;
            this.Log.Information($"{RECV}{CMD} {command}.");

            var result = this.database.AddAtomicOrder(
                command.AtomicOrder,
                command.TraderId,
                command.AccountId,
                command.StrategyId,
                command.PositionId);

            if (result.IsSuccess)
            {
                this.gateway.SubmitOrder(command.AtomicOrder);

                var submitted1 = new OrderSubmitted(
                    command.AccountId,
                    command.AtomicOrder.Entry.Id,
                    this.TimeNow(),
                    this.NewGuid(),
                    this.TimeNow());

                var submitted2 = new OrderSubmitted(
                    command.AccountId,
                    command.AtomicOrder.StopLoss.Id,
                    this.TimeNow(),
                    this.NewGuid(),
                    this.TimeNow());

                command.AtomicOrder.Entry.Apply(submitted1);
                command.AtomicOrder.StopLoss.Apply(submitted2);
                this.database.UpdateOrder(command.AtomicOrder.Entry);
                this.database.UpdateOrder(command.AtomicOrder.StopLoss);

                this.SendToEventPublisher(submitted1);
                this.SendToEventPublisher(submitted2);

                if (command.AtomicOrder.TakeProfit != null)
                {
                    var submitted3 = new OrderSubmitted(
                        command.AccountId,
                        command.AtomicOrder.TakeProfit.Id,
                        this.TimeNow(),
                        this.NewGuid(),
                        this.TimeNow());

                    command.AtomicOrder.TakeProfit.Apply(submitted3);
                    this.database.UpdateOrder(command.AtomicOrder.TakeProfit);

                    this.SendToEventPublisher(submitted3);
                }
            }
            else
            {
                this.Log.Error($"Cannot execute command {command} {result.Message}");
            }
        }

        private void OnMessage(CancelOrder command)
        {
            this.CommandCount++;
            this.Log.Information($"{RECV}{CMD} {command}.");

            var order = this.database.GetOrder(command.OrderId);
            if (order is null)
            {
                this.Log.Error($"Cannot execute command {command} (order was not found in the memory cache).");
                return;
            }

            if (order.IsCompleted)
            {
                this.Log.Warning($"{RECV}{CMD} {command} and (order is already completed).");
            }

            this.gateway.CancelOrder(order);
        }

        private void OnMessage(ModifyOrder command)
        {
            this.CommandCount++;
            this.Log.Information($"{RECV}{CMD} {command}.");

            var order = this.database.GetOrder(command.OrderId);
            if (order is null)
            {
                this.Log.Error($"Cannot execute command {command} (order was not found in the memory cache).");
                return;
            }

            if (!order.IsWorking)
            {
                this.bufferModify[command.OrderId] = command;
                this.Log.Warning($"Buffering command {command} (order not yet working).");
                return;
            }

            if (this.bufferModify.ContainsKey(command.OrderId))
            {
                this.bufferModify[command.OrderId] = command;
                this.Log.Debug($"Buffering command {command} (order already being modified).");
                return;
            }

            // Buffer the command to check in later processing
            this.bufferModify[command.OrderId] = command;
            this.gateway.ModifyOrder(order, command.ModifiedPrice);
        }

        private void OnMessage(AccountInquiry command)
        {
            this.CommandCount++;
            this.Log.Information($"{RECV}{CMD} {command}.");

            this.gateway.AccountInquiry();
        }

        //-- EVENTS --------------------------------------------------------------------------------------------------//
        private void OnMessage(OrderSubmitted @event)
        {
            this.EventCount++;
            this.Log.Information($"{RECV}{EVT} {@event}.");

            this.ProcessOrderEvent(@event);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderAccepted @event)
        {
            this.EventCount++;
            this.Log.Information($"{RECV}{EVT} {@event}.");

            this.ProcessOrderEvent(@event);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderRejected @event)
        {
            this.EventCount++;
            this.Log.Warning($"{RECV}{EVT} {@event}.");

            this.ProcessOrderEvent(@event);
            this.ClearModifyBuffer(@event.OrderId);
            this.ClearExpiryBackup(@event.OrderId);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderWorking @event)
        {
            this.EventCount++;
            this.Log.Information($"{RECV}{EVT} {@event}.");

            var order = this.ProcessOrderEvent(@event);
            if (order != null)
            {
                this.ProcessModifyBuffer(order);

                if (this.OptionGtdExpiryBackups && this.IsExpiredGtdOrder(order))
                {
                    // Creates a scheduled CancelOrder backup command for the expiry time
                    this.CreateGtdCancelBackup(order);
                }
            }

            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderModified @event)
        {
            this.EventCount++;
            this.Log.Information($"{RECV}{EVT} {@event}.");

            var order = this.ProcessOrderEvent(@event);
            if (order != null)
            {
                this.ProcessModifyBuffer(order);
            }

            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderCancelReject @event)
        {
            this.EventCount++;
            this.Log.Warning($"{RECV}{EVT} {@event}.");

            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderExpired @event)
        {
            this.EventCount++;
            this.Log.Information($"{RECV}{EVT} {@event}.");

            this.ProcessOrderEvent(@event);
            this.ClearModifyBuffer(@event.OrderId);
            this.ClearExpiryBackup(@event.OrderId);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderCancelled @event)
        {
            this.EventCount++;
            this.Log.Information($"{RECV}{EVT} {@event}.");

            this.ProcessOrderEvent(@event);
            this.ClearModifyBuffer(@event.OrderId);
            this.ClearExpiryBackup(@event.OrderId);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderPartiallyFilled @event)
        {
            this.EventCount++;
            this.Log.Warning($"{RECV}{EVT} {@event}.");

            this.ProcessOrderEvent(@event);
            this.HandleOrderFillEvent(@event);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderFilled @event)
        {
            this.EventCount++;
            this.Log.Information($"{RECV}{EVT} {@event}.");

            this.ProcessOrderEvent(@event);
            this.HandleOrderFillEvent(@event);
            this.ClearModifyBuffer(@event.OrderId);
            this.ClearExpiryBackup(@event.OrderId);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(AccountStateEvent @event)
        {
            this.EventCount++;
            this.Log.Information($"{RECV}{EVT} {@event}.");

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

        private Order? ProcessOrderEvent(OrderEvent @event)
        {
            var order = this.database.GetOrder(@event.OrderId);
            if (order is null)
            {
                // This should never happen
                this.Log.Warning($"Cannot apply event {@event} to any order ({@event.OrderId} not found in the cache.");
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
            var positionId = this.database.GetPositionId(@event.OrderId) ?? this.database.GetPositionId(@event.AccountId, @event.PositionIdBroker);
            if (positionId is null)
            {
                this.Log.Error($"Cannot process event {@event} (no PositionId found for event).");
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
                // If the commands modified price or quantity is different then send command
                if ((!(order.Price is null) && order.Price != modifyOrder.ModifiedPrice)
                    || order.Quantity != modifyOrder.ModifiedQuantity)
                {
                    this.gateway.ModifyOrder(order, modifyOrder.ModifiedPrice);
                    this.Log.Debug($"{CMD}{SENT} {modifyOrder} to TradingGateway.");
                }

                this.bufferModify.Remove(order.Id);
                this.Log.Debug($"Cleared command {modifyOrder} from buffer.");
            }
        }

        private void ClearModifyBuffer(OrderId orderId)
        {
            if (this.bufferModify.ContainsKey(orderId))
            {
                this.bufferModify.Remove(orderId);
                this.Log.Debug($"Cleared ModifyOrder buffer for {orderId}.");
            }
        }

        private bool IsExpiredGtdOrder(Order order)
        {
            return order.TimeInForce == TimeInForce.GTD &&
                   order.ExpireTime != null &&
                   this.TimeNow().IsLessThan((ZonedDateTime)order.ExpireTime);
        }

        private void CreateGtdCancelBackup(Order order)
        {
            if (order.ExpireTime.HasValue)
            {
                var traderId = this.database.GetTraderId(order.Id);
                if (traderId is null)
                {
                    // This should never happen
                    this.Log.Error($"Cannot schedule backup CancelOrder command (cannot find TraderId for {order.Id}).");
                    return;
                }

                var accountId = this.database.GetAccountId(order.Id);
                if (accountId is null)
                {
                    // This should never happen
                    this.Log.Error($"Cannot schedule backup CancelOrder command (cannot find AccountId for {order.Id}).");
                    return;
                }

                var cancelOrder = new CancelOrder(
                    traderId,
                    accountId,
                    order.Id,
                    "GTD_EXPIRY_BACKUP",
                    this.NewGuid(),
                    order.ExpireTime.Value);

                var delay = TimingProvider.GetDurationToNextUtc(order.ExpireTime.Value, this.InstantNow());
                var token = this.scheduler.ScheduleSendOnceCancelable(delay, this.Endpoint, cancelOrder, this.Endpoint);
                this.gtdExpiryBackupTokens[order.Id] = token;

                this.Log.Debug($"Scheduled GTD CancelOrder backup for {order.ExpireTime.Value.ToIsoString()}.");
            }
            else
            {
                // This should never happen
                this.Log.Error($"Cannot schedule backup CancelOrder (no expire time set for GTD order {order.Id}).");
            }
        }

        private void ClearExpiryBackup(OrderId orderId)
        {
            if (this.gtdExpiryBackupTokens.TryGetValue(orderId, out var token))
            {
                token.Cancel();
                this.gtdExpiryBackupTokens.Remove(orderId);
                this.Log.Debug($"Cleared GtdExpiryBackup for {orderId}.");
            }
        }

        private void SendToEventPublisher(OrderEvent @event)
        {
            var traderId = this.database.GetTraderId(@event.OrderId);
            if (traderId is null)
            {
                this.Log.Error($"Cannot send event {@event} to publisher (no TraderId found for order).");
                return;
            }

            this.SendToEventPublisher(new TradeEvent(traderId, @event));
        }

        private void SendToEventPublisher(Event @event)
        {
            this.eventPublisher.Send(@event);
            this.Log.Debug($"{EVT}{SENT} {@event} to EventPublisher.");
        }
    }
}
