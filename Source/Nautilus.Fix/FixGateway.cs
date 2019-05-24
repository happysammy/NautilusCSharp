//--------------------------------------------------------------------------------------------------
// <copyright file="FixGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Common.Messages.Documents;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using NodaTime;

    /// <summary>
    /// Provides a gateway to, and anti-corruption layer from, the FIX module of the service.
    /// </summary>
    [PerformanceOptimized]
    public sealed class FixGateway : ComponentBusConnectedBase, IFixGateway
    {
        private readonly IFixClient fixClient;
        private readonly List<IEndpoint> tickReceivers;
        private readonly List<Address> eventReceivers;
        private readonly List<Address> instrumentReceivers;
        private readonly Currency accountCurrency = Currency.AUD;  // TODO

        /// <summary>
        /// Initializes a new instance of the <see cref="FixGateway"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="fixClient">The FIX client.</param>
        public FixGateway(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IFixClient fixClient)
            : base(
                NautilusService.FIX,
                container,
                messagingAdapter)
        {
            this.fixClient = fixClient;
            this.tickReceivers = new List<IEndpoint>();
            this.eventReceivers = new List<Address>();
            this.instrumentReceivers = new List<Address>();

            this.RegisterHandler<ConnectFix>(this.OnMessage);
            this.RegisterHandler<DisconnectFix>(this.OnMessage);
        }

        /// <inheritdoc />
        public Brokerage Broker => this.fixClient.Broker;

        /// <inheritdoc />
        public bool IsConnected => this.fixClient.IsConnected;

        /// <inheritdoc />
        public void RegisterTickReceiver(IEndpoint receiver)
        {
            Debug.NotIn(receiver, this.tickReceivers, nameof(receiver), nameof(this.tickReceivers));

            this.tickReceivers.Add(receiver);
        }

        /// <inheritdoc />
        public void RegisterConnectionEventReceiver(Address receiver)
        {
            this.fixClient.RegisterConnectionEventReceiver(receiver);
        }

        /// <inheritdoc />
        public void RegisterEventReceiver(Address receiver)
        {
            Debug.NotIn(receiver, this.eventReceivers, nameof(receiver), nameof(this.eventReceivers));

            this.eventReceivers.Add(receiver);
        }

        /// <inheritdoc />
        public void RegisterInstrumentReceiver(Address receiver)
        {
            Debug.NotIn(receiver, this.instrumentReceivers, nameof(receiver), nameof(this.instrumentReceivers));

            this.instrumentReceivers.Add(receiver);
        }

        /// <inheritdoc />
        public void MarketDataSubscribe(Symbol symbol)
        {
            this.fixClient.RequestMarketDataSubscribe(symbol);
        }

        /// <inheritdoc />
        public void MarketDataSubscribeAll()
        {
            this.fixClient.RequestMarketDataSubscribeAll();
        }

        /// <inheritdoc />
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            this.fixClient.UpdateInstrumentSubscribe(symbol);
        }

        /// <inheritdoc />
        public void UpdateInstrumentsSubscribeAll()
        {
            this.fixClient.UpdateInstrumentsSubscribeAll();
        }

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
        [PerformanceOptimized]
        public void OnTick(
            string symbolCode,
            Venue venue,
            decimal bid,
            decimal ask,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Condition.PositiveDecimal(bid, nameof(bid));
                Condition.PositiveDecimal(ask, nameof(ask));

                var tick = new Tick(
                    new Symbol(symbolCode, venue),
                    Price.Create(bid),
                    Price.Create(ask),
                    timestamp);

                this.SendToTickReceivers(tick);
            });
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
        public void OnInstrumentsUpdate(
            IEnumerable<Instrument> instruments,
            string responseId,
            string result)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(responseId, nameof(responseId));
                Condition.NotEmptyOrWhiteSpace(result, nameof(result));

                this.Log.Debug(
                    $"SecurityListReceived: " +
                    $"(SecurityResponseId={responseId}) result={result}");

                foreach (var instrument in instruments)
                {
                    var dataDelivery = new DataDelivery<Instrument>(
                        instrument,
                        this.NewGuid(),
                        this.TimeNow());

                    this.SendToInstrumentReceivers(dataDelivery);
                }
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
                    EntityIdFactory.Account(this.fixClient.Broker, accountNumber),
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

                this.SendToEventReceivers(accountEvent);

                this.Log.Debug(
                    $"AccountEvent: " +
                    $"({this.fixClient.Broker}-{accountNumber}, " +
                    $"InquiryId={inquiryId})");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnOrderRejected(
            string symbolCode,
            Venue venue,
            string orderId,
            string rejectReason,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Condition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Condition.NotEmptyOrWhiteSpace(rejectReason, nameof(rejectReason));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var orderRejected = new OrderRejected(
                    new Symbol(symbolCode, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    timestamp,
                    rejectReason,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendToEventReceivers(orderRejected);

                this.Log.Warning(
                    $"OrderRejected: " +
                    $"(OrderId={orderId}, " +
                    $"RejectReason={rejectReason}");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnOrderCancelReject(
            string symbolCode,
            Venue venue,
            string orderId,
            string cancelRejectResponseTo,
            string cancelRejectReason,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Condition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Condition.NotEmptyOrWhiteSpace(cancelRejectResponseTo, nameof(cancelRejectResponseTo));
                Condition.NotEmptyOrWhiteSpace(cancelRejectReason, nameof(cancelRejectReason));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var orderCancelReject = new OrderCancelReject(
                    new Symbol(symbolCode, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    timestamp,
                    cancelRejectResponseTo,
                    cancelRejectReason,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendToEventReceivers(orderCancelReject);

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
            string symbolCode,
            Venue venue,
            string orderId,
            string brokerOrderId,
            string orderLabel,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Condition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Condition.NotEmptyOrWhiteSpace(brokerOrderId, nameof(brokerOrderId));
                Condition.NotEmptyOrWhiteSpace(orderLabel, nameof(orderLabel));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var orderCancelled = new OrderCancelled(
                    new Symbol(symbolCode, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendToEventReceivers(orderCancelled);

                this.Log.Information(
                    $"OrderCancelled: {orderLabel} " +
                    $"(OrderId={orderId}, " +
                    $"BrokerOrderId={brokerOrderId})");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnOrderModified(
            string symbolCode,
            Venue venue,
            string orderId,
            string brokerOrderId,
            string orderLabel,
            decimal price,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Condition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Condition.NotEmptyOrWhiteSpace(brokerOrderId, nameof(brokerOrderId));
                Condition.NotEmptyOrWhiteSpace(orderLabel, nameof(orderLabel));
                Condition.PositiveDecimal(price, nameof(price));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var orderModified = new OrderModified(
                    new Symbol(symbolCode, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    new OrderId(brokerOrderId),
                    Price.Create(price, price.GetDecimalPlaces()),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendToEventReceivers(orderModified);

                this.Log.Information(
                    $"OrderModified: {orderLabel} " +
                    $"(OrderId={orderId}, " +
                    $"BrokerOrderId={brokerOrderId}, " +
                    $"Price={price})");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnOrderWorking(
            string symbolCode,
            Venue venue,
            string orderId,
            string brokerOrderId,
            string orderLabel,
            OrderSide orderSide,
            OrderType orderType,
            int quantity,
            decimal price,
            TimeInForce timeInForce,
            ZonedDateTime? expireTime,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Condition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Condition.NotEmptyOrWhiteSpace(brokerOrderId, nameof(brokerOrderId));
                Condition.NotEmptyOrWhiteSpace(orderLabel, nameof(orderLabel));
                Condition.PositiveDecimal(price, nameof(price));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var orderWorking = new OrderWorking(
                    new Symbol(symbolCode, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    new OrderId(brokerOrderId),
                    new Label(orderLabel),
                    orderSide,
                    orderType,
                    Quantity.Create(quantity),
                    Price.Create(price, price.GetDecimalPlaces()),
                    timeInForce,
                    expireTime,
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendToEventReceivers(orderWorking);

                var expireTimeString = string.Empty;

                if (expireTime.HasValue)
                {
                    expireTimeString = expireTime.Value.ToIsoString();
                }

                this.Log.Information(
                    $"OrderWorking: {orderLabel} " +
                    $"(OrderId={orderId}, " +
                    $"BrokerOrderId={brokerOrderId}, " +
                    $"Price={price}, " +
                    $"ExpireTime={expireTimeString})");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnOrderExpired(
            string symbolCode,
            Venue venue,
            string orderId,
            string brokerOrderId,
            string orderLabel,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Condition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Condition.NotEmptyOrWhiteSpace(brokerOrderId, nameof(brokerOrderId));
                Condition.NotEmptyOrWhiteSpace(orderLabel, nameof(orderLabel));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var orderExpired = new OrderExpired(
                    new Symbol(symbolCode, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendToEventReceivers(orderExpired);

                this.Log.Information(
                    $"OrderExpired: {orderLabel} " +
                    $"(OrderId={orderId}, " +
                    $"BrokerOrderId={brokerOrderId})");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnOrderFilled(
            string symbolCode,
            Venue venue,
            string orderId,
            string brokerOrderId,
            string executionId,
            string executionTicket,
            string orderLabel,
            OrderSide orderSide,
            int filledQuantity,
            decimal averagePrice,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Condition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Condition.NotEmptyOrWhiteSpace(brokerOrderId, nameof(brokerOrderId));
                Condition.NotEmptyOrWhiteSpace(executionId, nameof(executionId));
                Condition.NotEmptyOrWhiteSpace(executionTicket, nameof(executionTicket));
                Condition.NotEmptyOrWhiteSpace(orderLabel, nameof(orderLabel));
                Condition.PositiveInt32(filledQuantity, nameof(filledQuantity));
                Condition.PositiveDecimal(averagePrice, nameof(averagePrice));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var orderFilled = new OrderFilled(
                    new Symbol(symbolCode, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    new ExecutionId(executionId),
                    new ExecutionTicket(executionTicket),
                    orderSide,
                    Quantity.Create(filledQuantity),
                    Price.Create(averagePrice, averagePrice.GetDecimalPlaces()),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendToEventReceivers(orderFilled);

                this.Log.Information(
                    $"OrderFilled: {orderLabel} " +
                    $"(OrderId={orderId}, " +
                    $"BrokerOrderId={brokerOrderId}, " +
                    $"ExecutionId={executionId}, " +
                    $"ExecutionTicket={executionTicket}, " +
                    $"FilledQty={filledQuantity} at {averagePrice})");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnOrderPartiallyFilled(
            string symbolCode,
            Venue venue,
            string orderId,
            string brokerOrderId,
            string executionId,
            string executionTicket,
            string orderLabel,
            OrderSide orderSide,
            int filledQuantity,
            int leavesQuantity,
            decimal averagePrice,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Condition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Condition.NotEmptyOrWhiteSpace(brokerOrderId, nameof(brokerOrderId));
                Condition.NotEmptyOrWhiteSpace(executionId, nameof(executionId));
                Condition.NotEmptyOrWhiteSpace(executionTicket, nameof(executionTicket));
                Condition.NotEmptyOrWhiteSpace(orderLabel, nameof(orderLabel));
                Condition.PositiveInt32(filledQuantity, nameof(filledQuantity));
                Condition.PositiveInt32(leavesQuantity, nameof(leavesQuantity));
                Condition.PositiveDecimal(averagePrice, nameof(averagePrice));
                Condition.NotDefault(timestamp, nameof(timestamp));

                var orderPartiallyFilled = new OrderPartiallyFilled(
                    new Symbol(symbolCode, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    new ExecutionId(executionId),
                    new ExecutionTicket(executionTicket),
                    orderSide,
                    Quantity.Create(filledQuantity),
                    Quantity.Create(leavesQuantity),
                    Price.Create(averagePrice, averagePrice.GetDecimalPlaces()),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                this.SendToEventReceivers(orderPartiallyFilled);

                this.Log.Information(
                    $"OrderPartiallyFilled: {orderLabel} " +
                    $"(OrderId={orderId}, " +
                    $"BrokerOrderId={brokerOrderId}, " +
                    $"ExecutionId={executionId}, " +
                    $"ExecutionTicket={executionTicket}, " +
                    $"FilledQty={filledQuantity} at {averagePrice}, " +
                    $"LeavesQty={leavesQuantity})");
            });
        }

        /// <inheritdoc />
        protected override void OnStart(Start message)
        {
            this.Log.Information($"Starting from {message}...");

            this.fixClient.Connect();
        }

        /// <inheritdoc />
        protected override void OnStop(Stop message)
        {
            this.Log.Information($"Stopping from {message}...");

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

        private void SendToTickReceivers(Tick tick)
        {
            for (var i = 0; i < this.tickReceivers.Count; i++)
            {
                this.tickReceivers[i].Send(tick);
            }
        }

        private void SendToEventReceivers(Event message)
        {
            for (var i = 0; i < this.eventReceivers.Count; i++)
            {
                this.Send(this.eventReceivers[i], message);
            }
        }

        private void SendToInstrumentReceivers(DataDelivery<Instrument> message)
        {
            for (var i = 0; i < this.instrumentReceivers.Count; i++)
            {
                this.Send(this.instrumentReceivers[i], message);
            }
        }
    }
}
