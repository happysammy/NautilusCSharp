// -------------------------------------------------------------------------------------------------
// <copyright file="ExecutionEngine.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Logging;
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
using Nautilus.Scheduling.Messages;
using NodaTime;
using Quartz;

namespace Nautilus.Execution.Engine
{
    /// <summary>
    /// Provides a generic execution engine utilizing an abstract execution database.
    /// </summary>
    public sealed class ExecutionEngine : MessageBusConnected
    {
        private const string Sent = "-->";
        private const string Received = "<--";
        private const string Command = "[CMD]";
        private const string Event = "[EVT]";

        private readonly IExecutionDatabase database;
        private readonly ITradingGateway gateway;
        private readonly IEndpoint eventPublisher;

        private readonly Dictionary<OrderId, ModifyOrder> bufferModify;
        private readonly Dictionary<OrderId, JobKey> cancelJobs;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionEngine"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The message bus adapter.</param>
        /// <param name="database">The execution database.</param>
        /// <param name="gateway">The trading gateway.</param>
        /// <param name="eventPublisher">The event publisher endpoint.</param>
        /// <param name="gtdExpiryBackups">The option flag for GTD order expiry cancel backups.</param>
        public ExecutionEngine(
            IComponentryContainer container,
            IMessageBusAdapter messagingAdapter,
            IExecutionDatabase database,
            ITradingGateway gateway,
            IEndpoint eventPublisher,
            bool gtdExpiryBackups = true)
            : base(container, messagingAdapter)
        {
            this.database = database;
            this.gateway = gateway;
            this.eventPublisher = eventPublisher;

            this.bufferModify = new Dictionary<OrderId, ModifyOrder>();
            this.cancelJobs = new Dictionary<OrderId, JobKey>();

            this.GtdExpiryBackups = gtdExpiryBackups;

            // Commands
            this.RegisterHandler<SubmitOrder>(this.OnMessage);
            this.RegisterHandler<SubmitBracketOrder>(this.OnMessage);
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

            this.Logger.LogInformation(LogId.Component, $"{nameof(this.GtdExpiryBackups)} is {this.GtdExpiryBackups}");
        }

        /// <summary>
        /// Gets a value indicating whether GTD order expiry cancel backups are turned on.
        /// </summary>
        public bool GtdExpiryBackups { get; }

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
            this.Logger.LogInformation(LogId.Trading, $"{Received}{Command} {command}.");

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
                this.Logger.LogError(LogId.Database, $"Cannot execute command {command} ({result.Message}).");
            }
        }

        private void OnMessage(SubmitBracketOrder command)
        {
            this.CommandCount++;
            this.Logger.LogInformation(LogId.Trading, $"{Received}{Command} {command}.");

            var result = this.database.AddBracketOrder(
                command.BracketOrder,
                command.TraderId,
                command.AccountId,
                command.StrategyId,
                command.PositionId);

            if (result.IsSuccess)
            {
                this.gateway.SubmitOrder(command.BracketOrder);

                var submitted1 = new OrderSubmitted(
                    command.AccountId,
                    command.BracketOrder.Entry.Id,
                    this.TimeNow(),
                    this.NewGuid(),
                    this.TimeNow());

                var submitted2 = new OrderSubmitted(
                    command.AccountId,
                    command.BracketOrder.StopLoss.Id,
                    this.TimeNow(),
                    this.NewGuid(),
                    this.TimeNow());

                command.BracketOrder.Entry.Apply(submitted1);
                command.BracketOrder.StopLoss.Apply(submitted2);
                this.database.UpdateOrder(command.BracketOrder.Entry);
                this.database.UpdateOrder(command.BracketOrder.StopLoss);

                this.SendToEventPublisher(submitted1);
                this.SendToEventPublisher(submitted2);

                if (command.BracketOrder.TakeProfit != null)
                {
                    var submitted3 = new OrderSubmitted(
                        command.AccountId,
                        command.BracketOrder.TakeProfit.Id,
                        this.TimeNow(),
                        this.NewGuid(),
                        this.TimeNow());

                    command.BracketOrder.TakeProfit.Apply(submitted3);
                    this.database.UpdateOrder(command.BracketOrder.TakeProfit);

                    this.SendToEventPublisher(submitted3);
                }
            }
            else
            {
                this.Logger.LogError(LogId.Database, $"Cannot execute command {command} {result.Message}");
            }
        }

        private void OnMessage(CancelOrder command)
        {
            this.CommandCount++;
            this.Logger.LogInformation(LogId.Trading, $"{Received}{Command} {command}.");

            var order = this.database.GetOrder(command.OrderId);
            if (order is null)
            {
                this.Logger.LogError(LogId.Database, $"Cannot execute command {command} (order was not found in the memory cache).");
                return;
            }

            if (order.IsCompleted)
            {
                this.Logger.LogWarning(LogId.Trading, $"{Received}{Command} {command} and (order is already completed).");
                return;
            }

            this.gateway.CancelOrder(order);
        }

        private void OnMessage(ModifyOrder command)
        {
            this.CommandCount++;
            this.Logger.LogInformation(LogId.Trading, $"{Received}{Command} {command}.");

            var order = this.database.GetOrder(command.OrderId);
            if (order is null)
            {
                this.Logger.LogError(LogId.Database, $"Cannot execute command {command} (order was not found in the memory cache).");
                return;
            }

            if (!order.IsWorking)
            {
                this.bufferModify[command.OrderId] = command;
                this.Logger.LogWarning(LogId.Trading, $"Buffering command {command} (order not yet working).");
                return;
            }

            if (this.bufferModify.ContainsKey(command.OrderId))
            {
                this.bufferModify[command.OrderId] = command;
                this.Logger.LogDebug(LogId.Trading, $"Buffering command {command} (order already being modified).");
                return;
            }

            // Buffer the command to check in later processing
            this.bufferModify[command.OrderId] = command;
            this.gateway.ModifyOrder(
                order,
                command.ModifiedQuantity,
                command.ModifiedPrice);
        }

        private void OnMessage(AccountInquiry command)
        {
            this.CommandCount++;
            this.Logger.LogInformation(LogId.Trading, $"{Received}{Command} {command}.");

            this.gateway.AccountInquiry();
        }

        //-- EVENTS --------------------------------------------------------------------------------------------------//
        private void OnMessage(OrderSubmitted @event)
        {
            this.EventCount++;
            this.Logger.LogInformation(LogId.Trading, $"{Received}{Event} {@event}.");

            this.ProcessOrderEvent(@event);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderAccepted @event)
        {
            this.EventCount++;
            this.Logger.LogInformation(LogId.Trading, $"{Received}{Event} {@event}.");

            this.ProcessOrderEvent(@event);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderRejected @event)
        {
            this.EventCount++;
            this.Logger.LogWarning(LogId.Trading, $"{Received}{Event} {@event}.");

            this.ProcessOrderEvent(@event);
            this.ClearModifyBuffer(@event.OrderId);
            this.CancelExpiryBackup(@event.OrderId);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderWorking @event)
        {
            this.EventCount++;
            this.Logger.LogInformation(LogId.Trading, $"{Received}{Event} {@event}.");

            var order = this.ProcessOrderEvent(@event);
            if (order != null)
            {
                this.ProcessModifyBuffer(order);

                if (this.GtdExpiryBackups && this.IsExpiredGtdOrder(order))
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
            this.Logger.LogInformation(LogId.Trading, $"{Received}{Event} {@event}.");

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
            this.Logger.LogWarning(LogId.Trading, $"{Received}{Event} {@event}.");

            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderExpired @event)
        {
            this.EventCount++;
            this.Logger.LogInformation(LogId.Trading, $"{Received}{Event} {@event}.");

            this.ProcessOrderEvent(@event);
            this.ClearModifyBuffer(@event.OrderId);
            this.CancelExpiryBackup(@event.OrderId);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderCancelled @event)
        {
            this.EventCount++;
            this.Logger.LogInformation(LogId.Trading, $"{Received}{Event} {@event}.");

            this.ProcessOrderEvent(@event);
            this.ClearModifyBuffer(@event.OrderId);
            this.CancelExpiryBackup(@event.OrderId);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderPartiallyFilled @event)
        {
            this.EventCount++;
            this.Logger.LogWarning(LogId.Trading, $"{Received}{Event} {@event}.");

            this.ProcessOrderEvent(@event);
            this.HandleOrderFillEvent(@event);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(OrderFilled @event)
        {
            this.EventCount++;
            this.Logger.LogInformation(LogId.Trading, $"{Received}{Event} {@event}.");

            this.ProcessOrderEvent(@event);
            this.HandleOrderFillEvent(@event);
            this.ClearModifyBuffer(@event.OrderId);
            this.CancelExpiryBackup(@event.OrderId);
            this.SendToEventPublisher(@event);
        }

        private void OnMessage(AccountStateEvent @event)
        {
            this.EventCount++;
            this.Logger.LogInformation(LogId.Trading, $"{Received}{Event} {@event}.");

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
                this.Logger.LogWarning(
                    LogId.Database,
                    $"Cannot apply event {@event} to any order ({@event.OrderId} not found in the cache.");
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
                    this.Logger.LogError(ex.Message);
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
                this.Logger.LogError(
                    LogId.Database,
                    $"Cannot process event {@event} (no PositionId found for event).");
                return;
            }

            var position = this.database.GetPosition(positionId);
            if (position is null)
            {
                // Position does not exist - create new position
                position = new Position(positionId, @event);
                this.database.AddPosition(position);
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
                    this.gateway.ModifyOrder(
                        order,
                        modifyOrder.ModifiedQuantity,
                        modifyOrder.ModifiedPrice);
                    this.Logger.LogDebug(LogId.Trading, $"{Command}{Sent} {modifyOrder} to TradingGateway.");
                }

                this.bufferModify.Remove(order.Id);
                this.Logger.LogDebug(LogId.Trading, $"Cleared command {modifyOrder} from buffer.");
            }
        }

        private void ClearModifyBuffer(OrderId orderId)
        {
            if (this.bufferModify.ContainsKey(orderId))
            {
                this.bufferModify.Remove(orderId);
                this.Logger.LogDebug(LogId.Trading, $"Cleared ModifyOrder buffer for {orderId}.");
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
            if (!order.ExpireTime.HasValue)
            {
                // This should "never happen"
                this.Logger.LogError(
                    LogId.Trading,
                    $"Cannot schedule backup CancelOrder (no expire time set for GTD order {order.Id}).");

                return;
            }

            var traderId = this.database.GetTraderId(order.Id);
            if (traderId is null)
            {
                // This should never happen
                this.Logger.LogError(
                    LogId.Database,
                    $"Cannot schedule backup CancelOrder command (cannot find TraderId for {order.Id}).");
                return;
            }

            var accountId = this.database.GetAccountId(order.Id);
            if (accountId is null)
            {
                // This should "never happen"
                this.Logger.LogError(
                    LogId.Database,
                    $"Cannot schedule backup CancelOrder command (cannot find AccountId for {order.Id}).");
                return;
            }

            var cancelOrder = new CancelOrder(
                traderId,
                accountId,
                order.Id,
                "GTD_EXPIRY_BACKUP",
                this.NewGuid(),
                order.ExpireTime.Value);

            var jobKey = new JobKey($"{nameof(CancelOrder)}-{order.Id.Value}", "OMS");
            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(jobKey.Name, jobKey.Group)
                .StartAt(order.ExpireTime.Value.ToDateTimeUtc())
                .Build();

            var createJob = new CreateJob(
                this.Endpoint,
                cancelOrder,
                jobKey,
                trigger,
                this.NewGuid(),
                this.TimeNow());

            this.cancelJobs.Add(order.Id, jobKey);

            this.Send(createJob, ComponentAddress.Scheduler);
        }

        private void CancelExpiryBackup(OrderId orderId)
        {
            if (this.cancelJobs.TryGetValue(orderId, out var jobKey))
            {
                var removeJob = new RemoveJob(
                    jobKey,
                    true,
                    this.NewGuid(),
                    this.TimeNow());

                this.cancelJobs.Remove(orderId);
                this.Send(removeJob, ComponentAddress.Scheduler);
            }
        }

        private void SendToEventPublisher(OrderEvent @event)
        {
            var traderId = this.database.GetTraderId(@event.OrderId);
            if (traderId is null)
            {
                this.Logger.LogError(
                    LogId.Component,
                    $"Cannot send event {@event} to publisher (no TraderId found for order).");
                return;
            }

            this.SendToEventPublisher(new TradeEvent(traderId, @event));
        }

        private void SendToEventPublisher(Event @event)
        {
            this.eventPublisher.Send(@event);
            this.Logger.LogDebug(LogId.Component, $"{Event}{Sent} {@event} to EventPublisher.");
        }
    }
}
