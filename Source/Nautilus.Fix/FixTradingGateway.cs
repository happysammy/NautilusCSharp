//--------------------------------------------------------------------------------------------------
// <copyright file="FixTradingGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides a gateway to, and anti-corruption layer from, the FIX module of the service.
    /// </summary>
    [PerformanceOptimized]
    public sealed class FixTradingGateway : MessageBusConnected, ITradingGateway
    {
        private readonly IFixClient fixClient;
        private readonly Currency accountCurrency = Currency.USD;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixTradingGateway"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messageBusAdapter">The messaging adapter.</param>
        /// <param name="fixClient">The FIX client.</param>
        public FixTradingGateway(
            IComponentryContainer container,
            IMessageBusAdapter messageBusAdapter,
            IFixClient fixClient)
            : base(container, messageBusAdapter)
        {
            this.fixClient = fixClient;

            this.RegisterHandler<ConnectFix>(this.OnMessage);
            this.RegisterHandler<DisconnectFix>(this.OnMessage);
        }

        /// <inheritdoc />
        public Brokerage Broker => this.fixClient.Broker;

        /// <inheritdoc />
        public bool IsConnected => this.fixClient.IsConnected;

        /// <inheritdoc />
        public void CollateralInquiry()
        {
            this.fixClient.CollateralInquiry();
        }

        /// <inheritdoc />
        public void TradingSessionStatus()
        {
            this.fixClient.TradingSessionStatus();
        }

        /// <inheritdoc />
        public void SubmitOrder(Order order)
        {
            this.fixClient.SubmitOrder(order);
        }

        /// <inheritdoc />
        public void SubmitOrder(AtomicOrder atomicOrder)
        {
            this.fixClient.SubmitOrder(atomicOrder);
        }

        /// <inheritdoc />
        public void ModifyOrder(Order order, Price modifiedPrice)
        {
            this.fixClient.ModifyOrder(order, modifiedPrice);
        }

        /// <inheritdoc />
        public void CancelOrder(Order order)
        {
            this.fixClient.CancelOrder(order);
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnPositionReport(string account)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(account, nameof(account));

                this.Log.Debug($"PositionReport: ({account})");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnCollateralInquiryAck(string inquiryId, string accountNumber)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(inquiryId, nameof(inquiryId));
                Condition.NotEmptyOrWhiteSpace(accountNumber, nameof(accountNumber));

                this.Log.Debug(
                    $"CollateralInquiryAck: ({this.Broker}-{accountNumber}, " +
                    $"InquiryId={inquiryId})");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnBusinessMessage(string message)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(message, nameof(message));

                this.Log.Debug($"BusinessMessageReject: {message}");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnRequestForPositionsAck(string accountNumber, string positionRequestId)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(accountNumber, nameof(accountNumber));
                Condition.NotEmptyOrWhiteSpace(positionRequestId, nameof(positionRequestId));

                this.Log.Debug(
                    $"RequestForPositionsAck: ({accountNumber}-{positionRequestId})");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnAccountReport(
            string inquiryId,
            string accountNumber,
            decimal cashBalance,
            decimal cashStartDay,
            decimal cashDaily,
            decimal marginUsedMaintenance,
            decimal marginUsedLiq,
            decimal marginRatio,
            string marginCallStatus,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(inquiryId, nameof(inquiryId));
                Condition.NotEmptyOrWhiteSpace(accountNumber, nameof(accountNumber));
                Condition.NotNegativeDecimal(cashBalance, nameof(cashBalance));
                Condition.NotNegativeDecimal(cashStartDay, nameof(cashStartDay));
                Condition.NotNegativeDecimal(cashDaily, nameof(cashDaily));
                Condition.NotNegativeDecimal(marginUsedMaintenance, nameof(marginUsedMaintenance));
                Condition.NotNegativeDecimal(marginUsedLiq, nameof(marginUsedLiq));
                Condition.NotNegativeDecimal(marginRatio, nameof(marginRatio));
                Condition.NotEmptyOrWhiteSpace(marginCallStatus, nameof(marginCallStatus));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var accountEvent = new AccountEvent(
                    this.fixClient.Broker,
                    accountNumber,
                    this.accountCurrency,
                    Money.Create(cashBalance, this.accountCurrency),
                    Money.Create(cashStartDay, this.accountCurrency),
                    Money.Create(cashDaily, this.accountCurrency),
                    Money.Create(marginUsedLiq, this.accountCurrency),
                    Money.Create(marginUsedMaintenance, this.accountCurrency),
                    marginRatio,
                    marginCallStatus,
                    this.NewGuid(),
                    timestamp);

                this.SendToBus(accountEvent);

                this.Log.Debug(
                    $"AccountEvent: " +
                    $"({this.fixClient.Broker}-{accountNumber}, " +
                    $"InquiryId={inquiryId})");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnOrderRejected(
            string orderId,
            string rejectReason,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Condition.NotEmptyOrWhiteSpace(rejectReason, nameof(rejectReason));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var orderRejected = new OrderRejected(
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    timestamp,
                    rejectReason,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendToBus(orderRejected);

                this.Log.Warning(
                    $"OrderRejected: " +
                    $"(OrderId={orderId}, " +
                    $"RejectReason={rejectReason}");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnOrderCancelReject(
            string orderId,
            string cancelRejectResponseTo,
            string cancelRejectReason,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Condition.NotEmptyOrWhiteSpace(cancelRejectResponseTo, nameof(cancelRejectResponseTo));
                Condition.NotEmptyOrWhiteSpace(cancelRejectReason, nameof(cancelRejectReason));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var orderCancelReject = new OrderCancelReject(
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    timestamp,
                    cancelRejectResponseTo,
                    cancelRejectReason,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendToBus(orderCancelReject);

                this.Log.Warning(
                    $"OrderCancelReject: " +
                    $"(OrderId={orderId}, " +
                    $"CxlRejResponseTo={cancelRejectResponseTo}, " +
                    $"Reason={cancelRejectReason}");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnOrderCancelled(
            string orderId,
            string orderIdBroker,
            string label,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Condition.NotEmptyOrWhiteSpace(orderIdBroker, nameof(orderIdBroker));
                Condition.NotEmptyOrWhiteSpace(label, nameof(label));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var orderCancelled = new OrderCancelled(
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendToBus(orderCancelled);

                this.Log.Information(
                    $"OrderCancelled: {label} " +
                    $"(OrderId={orderId}, " +
                    $"BrokerOrderId={orderIdBroker})");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnOrderModified(
            string orderId,
            string orderIdBroker,
            string label,
            decimal price,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Condition.NotEmptyOrWhiteSpace(orderIdBroker, nameof(orderIdBroker));
                Condition.NotEmptyOrWhiteSpace(label, nameof(label));
                Condition.PositiveDecimal(price, nameof(price));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var orderModified = new OrderModified(
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    new OrderId(orderIdBroker),
                    Price.Create(price, price.GetDecimalPlaces()),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendToBus(orderModified);

                this.Log.Information(
                    $"OrderModified: {label} " +
                    $"(OrderId={orderId}, " +
                    $"BrokerOrderId={orderIdBroker}, " +
                    $"Price={price})");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnOrderWorking(
            string orderId,
            string orderIdBroker,
            string symbolCode,
            Venue venue,
            string label,
            OrderSide side,
            OrderType type,
            int quantity,
            decimal price,
            TimeInForce timeInForce,
            ZonedDateTime? expireTime,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Condition.NotEmptyOrWhiteSpace(orderIdBroker, nameof(orderIdBroker));
                Condition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Condition.NotEmptyOrWhiteSpace(label, nameof(label));
                Condition.PositiveDecimal(price, nameof(price));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var orderWorking = new OrderWorking(
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    new OrderId(orderIdBroker),
                    new Symbol(symbolCode, venue),
                    new Label(label),
                    side,
                    type,
                    Quantity.Create(quantity),
                    Price.Create(price, price.GetDecimalPlaces()),
                    timeInForce,
                    expireTime,
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendToBus(orderWorking);

                var expireTimeString = string.Empty;

                if (expireTime.HasValue)
                {
                    expireTimeString = expireTime.Value.ToIsoString();
                }

                this.Log.Information(
                    $"OrderWorking: {label} " +
                    $"(OrderId={orderId}, " +
                    $"BrokerOrderId={orderIdBroker}, " +
                    $"Price={price}, " +
                    $"ExpireTime={expireTimeString})");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnOrderExpired(
            string orderId,
            string orderIdBroker,
            string label,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Condition.NotEmptyOrWhiteSpace(orderIdBroker, nameof(orderIdBroker));
                Condition.NotEmptyOrWhiteSpace(label, nameof(label));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var orderExpired = new OrderExpired(
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendToBus(orderExpired);

                this.Log.Information(
                    $"OrderExpired: {label} " +
                    $"(OrderId={orderId}, " +
                    $"BrokerOrderId={orderIdBroker})");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnOrderFilled(
            string orderId,
            string orderIdBroker,
            string executionId,
            string executionTicket,
            string symbolCode,
            Venue venue,
            OrderSide side,
            int filledQuantity,
            decimal averagePrice,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Condition.NotEmptyOrWhiteSpace(orderIdBroker, nameof(orderIdBroker));
                Condition.NotEmptyOrWhiteSpace(executionId, nameof(executionId));
                Condition.NotEmptyOrWhiteSpace(executionTicket, nameof(executionTicket));
                Condition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Condition.PositiveInt32(filledQuantity, nameof(filledQuantity));
                Condition.PositiveDecimal(averagePrice, nameof(averagePrice));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var orderFilled = new OrderFilled(
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    new ExecutionId(executionId),
                    new ExecutionTicket(executionTicket),
                    new Symbol(symbolCode, venue),
                    side,
                    Quantity.Create(filledQuantity),
                    Price.Create(averagePrice, averagePrice.GetDecimalPlaces()),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendToBus(orderFilled);

                this.Log.Information(
                    $"OrderFilled: " +
                    $"(OrderId={orderId}, " +
                    $"BrokerOrderId={orderIdBroker}, " +
                    $"ExecutionId={executionId}, " +
                    $"ExecutionTicket={executionTicket}, " +
                    $"FilledQty={filledQuantity} at {averagePrice})");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnOrderPartiallyFilled(
            string orderId,
            string orderIdBroker,
            string executionId,
            string executionTicket,
            string symbolCode,
            Venue venue,
            OrderSide side,
            int filledQuantity,
            int leavesQuantity,
            decimal averagePrice,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Condition.NotEmptyOrWhiteSpace(orderIdBroker, nameof(orderIdBroker));
                Condition.NotEmptyOrWhiteSpace(executionId, nameof(executionId));
                Condition.NotEmptyOrWhiteSpace(executionTicket, nameof(executionTicket));
                Condition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Condition.PositiveInt32(filledQuantity, nameof(filledQuantity));
                Condition.PositiveInt32(leavesQuantity, nameof(leavesQuantity));
                Condition.PositiveDecimal(averagePrice, nameof(averagePrice));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var orderPartiallyFilled = new OrderPartiallyFilled(
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    new ExecutionId(executionId),
                    new ExecutionTicket(executionTicket),
                    new Symbol(symbolCode, venue),
                    side,
                    Quantity.Create(filledQuantity),
                    Quantity.Create(leavesQuantity),
                    Price.Create(averagePrice, averagePrice.GetDecimalPlaces()),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendToBus(orderPartiallyFilled);

                this.Log.Information(
                    $"OrderPartiallyFilled: " +
                    $"(OrderId={orderId}, " +
                    $"BrokerOrderId={orderIdBroker}, " +
                    $"ExecutionId={executionId}, " +
                    $"ExecutionTicket={executionTicket}, " +
                    $"FilledQty={filledQuantity} at {averagePrice}, " +
                    $"LeavesQty={leavesQuantity})");
            });
        }

        /// <inheritdoc />
        protected override void OnStart(Start start)
        {
            this.fixClient.Connect();
        }

        /// <inheritdoc />
        protected override void OnStop(Stop stop)
        {
            this.fixClient.Disconnect();
        }

        private void OnMessage(ConnectFix message)
        {
            this.fixClient.Connect();
        }

        private void OnMessage(DisconnectFix message)
        {
            this.fixClient.Disconnect();
        }
    }
}
