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
    /// Provides a gateway to the FIX implementation of the system.
    /// </summary>
    [PerformanceOptimized]
    public sealed class FixGateway : ComponentBusConnectedBase, IFixGateway
    {
        private readonly IFixClient fixClient;
        private readonly List<IEndpoint> tickReceivers;
        private readonly List<IEndpoint> eventReceivers;
        private readonly List<Address> instrumentReceivers;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixGateway"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
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
            this.eventReceivers = new List<IEndpoint>();
            this.instrumentReceivers = new List<Address>();
        }

        /// <summary>
        /// Gets the brokerage gateways broker name.
        /// </summary>
        public Brokerage Broker => this.fixClient.Broker;

        /// <summary>
        /// Gets the brokerage account currency.
        /// </summary>
        public Currency AccountCurrency => Currency.AUD;  // TODO: Account currency

        /// <summary>
        /// Gets a value indicating whether the brokerage gateways broker client is connected.
        /// </summary>
        public bool IsConnected => this.fixClient.IsConnected;

        /// <summary>
        /// Connect to the FIX platform.
        /// </summary>
        public void Connect()
        {
            this.fixClient.Connect();
        }

        /// <summary>
        /// Disconnect from the the FIX platform.
        /// </summary>
        public void Disconnect()
        {
            this.fixClient.Disconnect();
        }

        /// <summary>
        /// Registers the receiver endpoint to receive connection events from the gateway.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        public void RegisterConnectionEventReceiver(IEndpoint receiver)
        {
            this.fixClient.RegisterConnectionEventReceiver(receiver);
        }

        /// <summary>
        /// Registers the receiver endpoint to receive <see cref="Tick"/>s from the gateway.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        public void RegisterTickReceiver(IEndpoint receiver)
        {
            Debug.NotIn(receiver, this.tickReceivers, nameof(receiver), nameof(this.tickReceivers));

            this.tickReceivers.Add(receiver);
        }

        /// <summary>
        /// Registers the receiver endpoint to receive <see cref="Event"/>s from the gateway.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        public void RegisterEventReceiver(IEndpoint receiver)
        {
            Debug.NotIn(receiver, this.tickReceivers, nameof(receiver), nameof(this.eventReceivers));

            this.eventReceivers.Add(receiver);
        }

        /// <summary>
        /// Registers the service to receive <see cref="Instrument"/> updates from the gateway.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        public void RegisterInstrumentReceiver(Address receiver)
        {
            Debug.NotIn(receiver, this.instrumentReceivers, nameof(receiver), nameof(this.instrumentReceivers));

            this.instrumentReceivers.Add(receiver);
        }

        /// <summary>
        /// Submits a market data subscribe FIX message for the given symbol to the brokerage.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public void MarketDataSubscribe(Symbol symbol)
        {
            this.fixClient.RequestMarketDataSubscribe(symbol);
        }

        /// <summary>
        /// Submits a market data subscribe all FIX message for all symbols to the brokerage.
        /// </summary>
        public void MarketDataSubscribeAll()
        {
            this.fixClient.RequestMarketDataSubscribeAll();
        }

        /// <summary>
        /// Submits an update on the instrument corresponding to the given symbol, and subscribe to
        /// updates FIX message, to the brokerage.
        /// </summary>
        /// <param name="symbol">The symbol of the instrument to update.</param>
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            this.fixClient.UpdateInstrumentSubscribe(symbol);
        }

        /// <summary>
        /// Submits an update all instruments FIX message to the brokerage.
        /// </summary>
        public void UpdateInstrumentsSubscribeAll()
        {
            this.fixClient.UpdateInstrumentsSubscribeAll();
        }

        /// <summary>
        /// Submits a collateral inquiry FIX message to the brokerage.
        /// </summary>
        public void CollateralInquiry()
        {
            this.fixClient.CollateralInquiry();
        }

        /// <summary>
        /// Submits a trading session status FIX message to the brokerage.
        /// </summary>
        public void TradingSessionStatus()
        {
            this.fixClient.TradingSessionStatus();
        }

        /// <summary>
        /// Submits a new order FIX message to the brokerage.
        /// </summary>
        /// <param name="order">The new order.</param>
        public void SubmitOrder(Order order)
        {
            this.fixClient.SubmitOrder(order);
        }

        /// <summary>
        /// Submits a new atomic order FIX message to the brokerage.
        /// </summary>
        /// <param name="atomicOrder">The new atomic order.</param>
        public void SubmitOrder(AtomicOrder atomicOrder)
        {
            this.fixClient.SubmitOrder(atomicOrder);
        }

        /// <summary>
        /// Submits an order cancel replace FIX message to modify the stop-loss of an existing order,
        /// to the brokerage.
        /// </summary>
        /// <param name="order">The order to modify.</param>
        /// <param name="modifiedPrice">The modified order price.</param>
        public void ModifyOrder(Order order, Price modifiedPrice)
        {
            this.fixClient.ModifyOrder(order, modifiedPrice);
        }

        /// <summary>
        /// Submits a cancel order FIX message to the brokerage.
        /// </summary>
        /// <param name="order">The order to cancel.</param>
        public void CancelOrder(Order order)
        {
            this.fixClient.CancelOrder(order);
        }

        /// <summary>
        /// Creates a new <see cref="Tick"/> and sends it directly to registered tick receivers.
        /// </summary>
        /// <param name="symbolCode">The tick symbol code.</param>
        /// <param name="venue">The tick venue.</param>
        /// <param name="bid">The tick best bid price.</param>
        /// <param name="ask">The tick best ask price.</param>
        /// <param name="timestamp">The tick timestamp.</param>
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
                Precondition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Precondition.PositiveDecimal(bid, nameof(bid));
                Precondition.PositiveDecimal(ask, nameof(ask));

                var tick = new Tick(
                    new Symbol(symbolCode, venue),
                    Price.Create(bid),
                    Price.Create(ask),
                    timestamp);

                for (var i = 0; i < this.tickReceivers.Count; i++)
                {
                    this.tickReceivers[i].Send(tick);
                }
            });
        }

        /// <summary>
        /// Event handler for receiving FIX Position Reports.
        /// </summary>
        /// <param name="account">The account.</param>
        [SystemBoundary]
        public void OnPositionReport(string account)
        {
            this.Execute(() =>
            {
                Precondition.NotEmptyOrWhiteSpace(account, nameof(account));

                this.Log.Debug($"PositionReport: ({account})");
            });
        }

        /// <summary>
        /// Event handler for receiving FIX Collateral Inquiry Acknowledgements.
        /// </summary>
        /// <param name="inquiryId">The inquiry identifier.</param>
        /// <param name="accountNumber">The account number.</param>
        [SystemBoundary]
        public void OnCollateralInquiryAck(string inquiryId, string accountNumber)
        {
            this.Execute(() =>
            {
                Precondition.NotEmptyOrWhiteSpace(inquiryId, nameof(inquiryId));
                Precondition.NotEmptyOrWhiteSpace(accountNumber, nameof(accountNumber));

                this.Log.Debug(
                    $"CollateralInquiryAck: ({this.Broker}-{accountNumber}, " +
                    $"InquiryId={inquiryId})");
            });
        }

        /// <summary>
        /// Event handler for receiving FIX business messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SystemBoundary]
        public void OnBusinessMessage(string message)
        {
            this.Execute(() =>
            {
                Precondition.NotEmptyOrWhiteSpace(message, nameof(message));

                this.Log.Debug($"BusinessMessageReject: {message}");
            });
        }

        /// <summary>
        /// Updates the given instruments in the <see cref="IInstrumentRepository"/>.
        /// </summary>
        /// <param name="instruments">The instruments collection.</param>
        /// <param name="responseId">The response identifier.</param>
        /// <param name="result">The result.</param>
        [SystemBoundary]
        public void OnInstrumentsUpdate(
            IEnumerable<Instrument> instruments,
            string responseId,
            string result)
        {
            this.Execute(() =>
            {
                Precondition.NotEmptyOrWhiteSpace(responseId, nameof(responseId));
                Precondition.NotEmptyOrWhiteSpace(result, nameof(result));

                this.Log.Debug(
                    $"SecurityListReceived: " +
                    $"(SecurityResponseId={responseId}) result={result}");

                var dataDelivery = new DataDelivery<IEnumerable<Instrument>>(
                    instruments,
                    this.NewGuid(),
                    this.TimeNow());

                foreach (var receiver in this.instrumentReceivers)
                {
                    this.Send(receiver, dataDelivery);
                }
            });
        }

        /// <summary>
        /// Event handler for acknowledgement of a request for positions.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="positionRequestId">The position request identifier.</param>
        [SystemBoundary]
        public void OnRequestForPositionsAck(string accountNumber, string positionRequestId)
        {
            this.Execute(() =>
            {
                Precondition.NotEmptyOrWhiteSpace(accountNumber, nameof(accountNumber));
                Precondition.NotEmptyOrWhiteSpace(positionRequestId, nameof(positionRequestId));

                this.Log.Debug(
                    $"RequestForPositionsAck: ({accountNumber}-{positionRequestId})");
            });
        }

        /// <summary>
        /// Creates an <see cref="AccountEvent"/> event, and sends it to the Risk Service via the
        /// Messaging system.
        /// </summary>
        /// <param name="inquiryId">The inquiry identifier.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="cashBalance">The cash balance.</param>
        /// <param name="cashStartDay">The cash start day.</param>
        /// <param name="cashDaily">The cash daily.</param>
        /// <param name="marginUsedMaintenance">The margin used maintenance.</param>
        /// <param name="marginUsedLiq">The margin used liquidity.</param>
        /// <param name="marginRatio">The margin ratio.</param>
        /// <param name="marginCallStatus">The margin call status.</param>
        /// <param name="timestamp">The report timestamp.</param>
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
                Precondition.NotEmptyOrWhiteSpace(inquiryId, nameof(inquiryId));
                Precondition.NotEmptyOrWhiteSpace(accountNumber, nameof(accountNumber));
                Precondition.NotNegativeDecimal(cashBalance, nameof(cashBalance));
                Precondition.NotNegativeDecimal(cashStartDay, nameof(cashStartDay));
                Precondition.NotNegativeDecimal(cashDaily, nameof(cashDaily));
                Precondition.NotNegativeDecimal(marginUsedMaintenance, nameof(marginUsedMaintenance));
                Precondition.NotNegativeDecimal(marginUsedLiq, nameof(marginUsedLiq));
                Precondition.NotNegativeDecimal(marginRatio, nameof(marginRatio));
                Precondition.NotEmptyOrWhiteSpace(marginCallStatus, nameof(marginCallStatus));
                Precondition.NotDefault(timestamp, nameof(timestamp));

                var accountEvent = new AccountEvent(
                    EntityIdFactory.Account(this.fixClient.Broker, accountNumber),
                    this.fixClient.Broker,
                    accountNumber,
                    this.AccountCurrency,
                    Money.Create(cashBalance, this.AccountCurrency),
                    Money.Create(cashStartDay, this.AccountCurrency),
                    Money.Create(cashDaily, this.AccountCurrency),
                    Money.Create(marginUsedLiq, this.AccountCurrency),
                    Money.Create(marginUsedMaintenance, this.AccountCurrency),
                    marginRatio,
                    marginCallStatus,
                    this.NewGuid(),
                    timestamp);

                foreach (var receiver in this.eventReceivers)
                {
                    receiver.Send(accountEvent);
                }

                this.Log.Debug(
                    $"AccountEvent: " +
                    $"({this.fixClient.Broker}-{accountNumber}, " +
                    $"InquiryId={inquiryId})");
            });
        }

        /// <summary>
        /// Creates an <see cref="OrderRejected"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbolCode">The order symbol code.</param>
        /// <param name="venue">The order venue.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="rejectReason">The order reject reason.</param>
        /// <param name="timestamp">The event timestamp.</param>
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
                Precondition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Precondition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Precondition.NotEmptyOrWhiteSpace(rejectReason, nameof(rejectReason));
                Precondition.NotDefault(timestamp, nameof(timestamp));

                var orderRejected = new OrderRejected(
                    new Symbol(symbolCode, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    timestamp,
                    rejectReason,
                    this.NewGuid(),
                    this.TimeNow());

                foreach (var receiver in this.eventReceivers)
                {
                    receiver.Send(orderRejected);
                }

                this.Log.Warning(
                    $"OrderRejected: " +
                    $"(OrderId={orderId}, " +
                    $"RejectReason={rejectReason}");
            });
        }

        /// <summary>
        /// Creates an <see cref="OrderCancelReject"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbolCode">The order symbol code.</param>
        /// <param name="venue">The order venue.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="cancelRejectResponseTo">The order cancel reject response to.</param>
        /// <param name="cancelRejectReason">The order cancel reject reason.</param>
        /// <param name="timestamp">The event timestamp.</param>
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
                Precondition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Precondition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Precondition.NotEmptyOrWhiteSpace(cancelRejectResponseTo, nameof(cancelRejectResponseTo));
                Precondition.NotEmptyOrWhiteSpace(cancelRejectReason, nameof(cancelRejectReason));
                Precondition.NotDefault(timestamp, nameof(timestamp));

                var orderCancelReject = new OrderCancelReject(
                    new Symbol(symbolCode, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    timestamp,
                    cancelRejectResponseTo,
                    cancelRejectReason,
                    this.NewGuid(),
                    this.TimeNow());

                foreach (var receiver in this.eventReceivers)
                {
                    receiver.Send(orderCancelReject);
                }

                this.Log.Warning(
                    $"OrderCancelReject: " +
                    $"(OrderId={orderId}, " +
                    $"CxlRejResponseTo={cancelRejectResponseTo}, " +
                    $"Reason={cancelRejectReason}");
            });
        }

        /// <summary>
        /// Creates an <see cref="OrderCancelled"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbolCode">The order symbol code.</param>
        /// <param name="venue">The order venue.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="orderLabel">The order Label.</param>
        /// <param name="timestamp">The event timestamp.</param>
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
                Precondition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Precondition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Precondition.NotEmptyOrWhiteSpace(brokerOrderId, nameof(brokerOrderId));
                Precondition.NotEmptyOrWhiteSpace(orderLabel, nameof(orderLabel));
                Precondition.NotDefault(timestamp, nameof(timestamp));

                var orderCancelled = new OrderCancelled(
                    new Symbol(symbolCode, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                foreach (var receiver in this.eventReceivers)
                {
                    receiver.Send(orderCancelled);
                }

                this.Log.Information(
                    $"OrderCancelled: {orderLabel} " +
                    $"(OrderId={orderId}, " +
                    $"BrokerOrderId={brokerOrderId})");
            });
        }

        /// <summary>
        /// Creates an <see cref="OrderModified"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbolCode">The order symbol code.</param>
        /// <param name="venue">The order venue.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="price">The order price.</param>
        /// <param name="timestamp">The event timestamp.</param>
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
                Precondition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Precondition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Precondition.NotEmptyOrWhiteSpace(brokerOrderId, nameof(brokerOrderId));
                Precondition.NotEmptyOrWhiteSpace(orderLabel, nameof(orderLabel));
                Precondition.PositiveDecimal(price, nameof(price));
                Precondition.NotDefault(timestamp, nameof(timestamp));

                var orderModified = new OrderModified(
                    new Symbol(symbolCode, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    new OrderId(brokerOrderId),
                    Price.Create(price, price.GetDecimalPlaces()),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                foreach (var receiver in this.eventReceivers)
                {
                    receiver.Send(orderModified);
                }

                this.Log.Information(
                    $"OrderModified: {orderLabel} " +
                    $"(OrderId={orderId}, " +
                    $"BrokerOrderId={brokerOrderId}, " +
                    $"Price={price})");
            });
        }

        /// <summary>
        /// Creates an <see cref="OrderWorking"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbolCode">The order symbol code.</param>
        /// <param name="venue">The order venue.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="orderType">The order type.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The order price.</param>
        /// <param name="timeInForce">The order time in force.</param>
        /// <param name="expireTime">The order expire time.</param>
        /// <param name="timestamp">The event timestamp.</param>
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
            OptionVal<ZonedDateTime> expireTime,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Precondition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Precondition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Precondition.NotEmptyOrWhiteSpace(brokerOrderId, nameof(brokerOrderId));
                Precondition.NotEmptyOrWhiteSpace(orderLabel, nameof(orderLabel));
                Precondition.PositiveDecimal(price, nameof(price));
                Precondition.NotDefault(timestamp, nameof(timestamp));

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

                foreach (var receiver in this.eventReceivers)
                {
                    receiver.Send(orderWorking);
                }

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

        /// <summary>
        /// Creates an <see cref="OrderExpired"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbolCode">The order symbol code.</param>
        /// <param name="venue">The order venue.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="timestamp">The event timestamp.</param>
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
                Precondition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Precondition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Precondition.NotEmptyOrWhiteSpace(brokerOrderId, nameof(brokerOrderId));
                Precondition.NotEmptyOrWhiteSpace(orderLabel, nameof(orderLabel));
                Precondition.NotDefault(timestamp, nameof(timestamp));

                var orderExpired = new OrderExpired(
                    new Symbol(symbolCode, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                foreach (var receiver in this.eventReceivers)
                {
                    receiver.Send(orderExpired);
                }

                this.Log.Information(
                    $"OrderExpired: {orderLabel} " +
                    $"(OrderId={orderId}, " +
                    $"BrokerOrderId={brokerOrderId})");
            });
        }

        /// <summary>
        /// Creates an <see cref="OrderFilled"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbolCode">The order symbol code.</param>
        /// <param name="venue">The order venue.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="executionId">The order execution identifier.</param>
        /// <param name="executionTicket">The order execution ticket.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="filledQuantity">The order filled quantity.</param>
        /// <param name="averagePrice">The order average price.</param>
        /// <param name="timestamp">The event timestamp.</param>
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
                Precondition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Precondition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Precondition.NotEmptyOrWhiteSpace(brokerOrderId, nameof(brokerOrderId));
                Precondition.NotEmptyOrWhiteSpace(executionId, nameof(executionId));
                Precondition.NotEmptyOrWhiteSpace(executionTicket, nameof(executionTicket));
                Precondition.NotEmptyOrWhiteSpace(orderLabel, nameof(orderLabel));
                Precondition.PositiveInt32(filledQuantity, nameof(filledQuantity));
                Precondition.PositiveDecimal(averagePrice, nameof(averagePrice));
                Precondition.NotDefault(timestamp, nameof(timestamp));

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

                foreach (var receiver in this.eventReceivers)
                {
                    receiver.Send(orderFilled);
                }

                this.Log.Information(
                    $"OrderFilled: {orderLabel} " +
                    $"(OrderId={orderId}, " +
                    $"BrokerOrderId={brokerOrderId}, " +
                    $"ExecutionId={executionId}, " +
                    $"ExecutionTicket={executionTicket}, " +
                    $"FilledQty={filledQuantity} at {averagePrice})");
            });
        }

        /// <summary>
        /// Creates an <see cref="OrderPartiallyFilled"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbolCode">The order symbol code.</param>
        /// <param name="venue">The order venue.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="executionId">The order execution identifier.</param>
        /// <param name="executionTicket">The order execution ticket.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="filledQuantity">The order filled quantity.</param>
        /// <param name="leavesQuantity">The order leaves quantity.</param>
        /// <param name="averagePrice">The order average price.</param>
        /// <param name="timestamp">The event timestamp.</param>
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
                Precondition.NotEmptyOrWhiteSpace(symbolCode, nameof(symbolCode));
                Precondition.NotEmptyOrWhiteSpace(orderId, nameof(orderId));
                Precondition.NotEmptyOrWhiteSpace(brokerOrderId, nameof(brokerOrderId));
                Precondition.NotEmptyOrWhiteSpace(executionId, nameof(executionId));
                Precondition.NotEmptyOrWhiteSpace(executionTicket, nameof(executionTicket));
                Precondition.NotEmptyOrWhiteSpace(orderLabel, nameof(orderLabel));
                Precondition.PositiveInt32(filledQuantity, nameof(filledQuantity));
                Precondition.PositiveInt32(leavesQuantity, nameof(leavesQuantity));
                Precondition.PositiveDecimal(averagePrice, nameof(averagePrice));
                Precondition.NotDefault(timestamp, nameof(timestamp));

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

                foreach (var receiver in this.eventReceivers)
                {
                    receiver.Send(orderPartiallyFilled);
                }

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
        protected override void Start(Start message)
        {
            this.Connect();
        }

        /// <inheritdoc />
        protected override void Stop(Stop message)
        {
            this.Disconnect();
        }
    }
}
