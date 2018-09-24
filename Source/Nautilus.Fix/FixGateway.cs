//--------------------------------------------------------------------------------------------------
// <copyright file="FixGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Collections;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.Interfaces;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The system boundary for the execution implementation.
    /// </summary>
    [Stateless]
    [PerformanceOptimized]
    public sealed class FixGateway : ComponentBusConnectedBase, IFixGateway
    {
        private readonly IFixClient fixClient;
        private readonly IInstrumentRepository instrumentRepository;
        private readonly Dictionary<string, int> pricePrecisionIndex;
        private ReadOnlyList<IEndpoint> tickReceivers;
        private ReadOnlyList<IEndpoint> eventReceivers;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixGateway"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="instrumentRepository">The instrument repository.</param>
        /// <param name="fixClient">The trade client.</param>
        public FixGateway(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IInstrumentRepository instrumentRepository,
            IFixClient fixClient)
            : base(
                NautilusService.FIX,
                new Label(nameof(FixGateway)),
                container,
                messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(fixClient, nameof(fixClient));
            Validate.NotNull(instrumentRepository, nameof(instrumentRepository));

            this.fixClient = fixClient;
            this.instrumentRepository = instrumentRepository;
            this.pricePrecisionIndex = instrumentRepository.GetPricePrecisionIndex();
            this.tickReceivers = new ReadOnlyList<IEndpoint>(new List<IEndpoint>());
            this.eventReceivers = new ReadOnlyList<IEndpoint>(new List<IEndpoint>());
        }

        /// <summary>
        /// Gets the brokerage gateways broker name.
        /// </summary>
        public Broker Broker => this.fixClient.Broker;

        /// <summary>
        /// Gets the brokerage account currency.
        /// </summary>
        public CurrencyCode AccountCurrency => CurrencyCode.AUD;

        /// <summary>
        /// Gets a value indicating whether the brokerage gateways broker client is connected.
        /// </summary>
        public bool IsConnected => this.fixClient.IsConnected;

        /// <summary>
        /// Returns the current time of the system clock.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public ZonedDateTime GetTimeNow()
        {
            return this.TimeNow();
        }

        /// <summary>
        /// Registers the receiver to receive <see cref="Tick"/>s from the gateway.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        public void RegisterTickReceiver(IEndpoint receiver)
        {
            Validate.NotNull(receiver, nameof(receiver));
            Debug.DoesNotContain(receiver, nameof(receiver), this.tickReceivers);

            var receivers = this.tickReceivers.ToList();
            receivers.Add(receiver);

            this.tickReceivers = new ReadOnlyList<IEndpoint>(receivers);
        }

        /// <summary>
        /// Registers the receiver to receive <see cref="Event"/>s from the gateway.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        public void RegisterEventReceiver(IEndpoint receiver)
        {
            Validate.NotNull(receiver, nameof(receiver));
            Debug.DoesNotContain(receiver, nameof(receiver), this.eventReceivers);

            var receivers = this.eventReceivers.ToList();
            receivers.Add(receiver);

            this.eventReceivers = new ReadOnlyList<IEndpoint>(receivers);
        }

        /// <summary>
        /// Requests market data for the given symbol from the brokerage.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public void RequestMarketDataSubscribe(Symbol symbol)
        {
            Debug.NotNull(symbol, nameof(symbol));

            this.fixClient.RequestMarketDataSubscribe(symbol);
        }

        /// <summary>
        /// Requests market data for all symbols from the brokerage.
        /// </summary>
        public void RequestMarketDataSubscribeAll()
        {
            this.fixClient.RequestMarketDataSubscribeAll();
        }

        /// <summary>
        /// Request an update on the instrument corresponding to the given symbol from the brokerage,
        /// and subscribe to updates.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            Debug.NotNull(symbol, nameof(symbol));

            this.fixClient.UpdateInstrumentSubscribe(symbol);
        }

        /// <summary>
        /// Requests an update on all instruments from the brokerage.
        /// </summary>
        public void UpdateInstrumentsSubscribeAll()
        {
            this.fixClient.UpdateInstrumentsSubscribeAll();
        }

        /// <summary>
        /// Submits a collateral inquiry to the brokerage.
        /// </summary>
        public void CollateralInquiry()
        {
            this.fixClient.CollateralInquiry();
        }

        /// <summary>
        /// Submits a trading session status request to the brokerage.
        /// </summary>
        public void TradingSessionStatus()
        {
            this.fixClient.TradingSessionStatus();
        }

        /// <summary>
        /// Submits an entry order with a stop-loss and profit target to the brokerage.
        /// </summary>
        /// <param name="order">The order to submit.</param>
        public void SubmitOrder(IOrder order)
        {
            Debug.NotNull(order, nameof(order));

            this.fixClient.SubmitOrder(order);
        }

        /// <summary>
        /// Submits an entry order with a stop-loss to the brokerage.
        /// </summary>
        /// <param name="atomicOrder">The atomic order to submit.</param>
        public void SubmitOrder(IAtomicOrder atomicOrder)
        {
            Debug.NotNull(atomicOrder, nameof(atomicOrder));

            this.fixClient.SubmitOrder(atomicOrder);
        }

        /// <summary>
        /// Submits a request to modify the stop-loss of an existing order.
        /// </summary>
        /// <param name="order">The order to modify.</param>
        /// <param name="modifiedPrice">The modified order price.</param>
        public void ModifyOrder(IOrder order, Price modifiedPrice)
        {
            Debug.NotNull(order, nameof(order));

            this.fixClient.ModifyOrder(order, modifiedPrice);
        }

        /// <summary>
        /// Submits a request to cancel the given order.
        /// </summary>
        /// <param name="order">The order to cancel.</param>
        public void CancelOrder(IOrder order)
        {
            Debug.NotNull(order, nameof(order));

            this.fixClient.CancelOrder(order);
        }

        /// <summary>
        /// Submits a request to close the given position to the brokerage client.
        /// </summary>
        /// <param name="position">The position to close.</param>
        public void ClosePosition(IPosition position)
        {
            Debug.NotNull(position, nameof(position));

            this.fixClient.ClosePosition(position);
        }

        /// <summary>
        /// Creates a new <see cref="Tick"/> and sends it to the tick publisher and bar aggregation
        /// controller.
        /// </summary>
        /// <param name="symbol">The tick symbol.</param>
        /// <param name="venue">The tick exchange.</param>
        /// <param name="bid">The tick bid price.</param>
        /// <param name="ask">The tick ask price.</param>
        /// <param name="timestamp">The tick timestamp.</param>
        [SystemBoundary]
        public void OnTick(
            string symbol,
            Venue venue,
            decimal bid,
            decimal ask,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Validate.NotNull(symbol, nameof(symbol));
                Validate.PositiveDecimal(bid, nameof(bid));
                Validate.PositiveDecimal(ask, nameof(ask));

                if (!this.pricePrecisionIndex.ContainsKey(symbol))
                {
                    this.Log.Warning($"Cannot process tick (symbol {symbol} not contained in tick precision index).");
                    return;
                }

                var tick = new Tick(
                    new Symbol(symbol, venue),
                    Price.Create(bid, this.pricePrecisionIndex[symbol]),
                    Price.Create(ask, this.pricePrecisionIndex[symbol]),
                    timestamp);

                foreach (var receiver in this.tickReceivers)
                {
                    receiver.Send(tick);
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
                Validate.NotNull(account, nameof(account));

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
                Validate.NotNull(inquiryId, nameof(inquiryId));
                Validate.NotNull(accountNumber, nameof(accountNumber));

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
                Validate.NotNull(message, nameof(message));

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
            IReadOnlyCollection<Instrument> instruments,
            string responseId,
            string result)
        {
            this.Execute(() =>
            {
                Validate.NotNull(instruments, nameof(instruments));
                Validate.NotNull(responseId, nameof(responseId));
                Validate.NotNull(result, nameof(result));

                this.Log.Debug(
                    $"SecurityListReceived: " +
                    $"(SecurityResponseId={responseId}) result={result}");

                var commandResult = this.instrumentRepository.Add(instruments, this.TimeNow());

                this.Log.Result(commandResult);
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
                Validate.NotNull(accountNumber, nameof(accountNumber));
                Validate.NotNull(positionRequestId, nameof(positionRequestId));

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
                Validate.NotNull(inquiryId, nameof(inquiryId));
                Validate.NotNull(accountNumber, nameof(accountNumber));
                Validate.NotNegativeDecimal(cashBalance, nameof(cashBalance));
                Validate.NotNegativeDecimal(cashStartDay, nameof(cashStartDay));
                Validate.NotNegativeDecimal(cashDaily, nameof(cashDaily));
                Validate.NotNegativeDecimal(marginUsedMaintenance, nameof(marginUsedMaintenance));
                Validate.NotNegativeDecimal(marginUsedLiq, nameof(marginUsedLiq));
                Validate.NotNegativeDecimal(marginRatio, nameof(marginRatio));
                Validate.NotNull(marginCallStatus, nameof(marginCallStatus));
                Validate.NotDefault(timestamp, nameof(timestamp));

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
        /// <param name="symbol">The order symbol.</param>
        /// <param name="venue">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="rejectReason">The order reject reason.</param>
        /// <param name="timestamp">The event timestamp.</param>
        [SystemBoundary]
        public void OnOrderRejected(
            string symbol,
            Venue venue,
            string orderId,
            string rejectReason,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Validate.NotNull(symbol, nameof(symbol));
                Validate.NotNull(orderId, nameof(orderId));
                Validate.NotNull(rejectReason, nameof(rejectReason));
                Validate.NotDefault(timestamp, nameof(timestamp));

                var orderRejected = new OrderRejected(
                    new Symbol(symbol, venue),
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
        /// <param name="symbol">The order symbol.</param>
        /// <param name="venue">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="cancelRejectResponseTo">The order cancel reject response to.</param>
        /// <param name="cancelRejectReason">The order cancel reject reason.</param>
        /// <param name="timestamp">The event timestamp.</param>
        [SystemBoundary]
        public void OnOrderCancelReject(
            string symbol,
            Venue venue,
            string orderId,
            string cancelRejectResponseTo,
            string cancelRejectReason,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Validate.NotNull(symbol, nameof(symbol));
                Validate.NotNull(orderId, nameof(orderId));
                Validate.NotNull(cancelRejectResponseTo, nameof(cancelRejectResponseTo));
                Validate.NotNull(cancelRejectReason, nameof(cancelRejectReason));
                Validate.NotDefault(timestamp, nameof(timestamp));

                var orderCancelReject = new OrderCancelReject(
                    new Symbol(symbol, venue),
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
        /// <param name="symbol">The order symbol.</param>
        /// <param name="venue">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="orderLabel">The order Label.</param>
        /// <param name="timestamp">The event timestamp.</param>
        [SystemBoundary]
        public void OnOrderCancelled(
            string symbol,
            Venue venue,
            string orderId,
            string brokerOrderId,
            string orderLabel,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Validate.NotNull(symbol, nameof(symbol));
                Validate.NotNull(orderId, nameof(orderId));
                Validate.NotNull(brokerOrderId, nameof(brokerOrderId));
                Validate.NotNull(orderLabel, nameof(orderLabel));
                Validate.NotDefault(timestamp, nameof(timestamp));

                var orderCancelled = new OrderCancelled(
                    new Symbol(symbol, venue),
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
        /// <param name="symbol">The order symbol.</param>
        /// <param name="venue">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="price">The order price.</param>
        /// <param name="timestamp">The event timestamp.</param>
        [SystemBoundary]
        public void OnOrderModified(
            string symbol,
            Venue venue,
            string orderId,
            string brokerOrderId,
            string orderLabel,
            decimal price,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Validate.NotNull(symbol, nameof(symbol));
                Validate.NotNull(orderId, nameof(orderId));
                Validate.NotNull(brokerOrderId, nameof(brokerOrderId));
                Validate.NotNull(orderLabel, nameof(orderLabel));
                Validate.PositiveDecimal(price, nameof(price));
                Validate.NotDefault(timestamp, nameof(timestamp));

                var orderModified = new OrderModified(
                    new Symbol(symbol, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    new OrderId(brokerOrderId),
                    Price.Create(price, this.pricePrecisionIndex[symbol]),
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
        /// <param name="symbol">The order symbol.</param>
        /// <param name="venue">The order exchange.</param>
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
            string symbol,
            Venue venue,
            string orderId,
            string brokerOrderId,
            string orderLabel,
            OrderSide orderSide,
            OrderType orderType,
            int quantity,
            decimal price,
            TimeInForce timeInForce,
            Option<ZonedDateTime?> expireTime,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Validate.NotNull(symbol, nameof(symbol));
                Validate.NotNull(orderId, nameof(orderId));
                Validate.NotNull(brokerOrderId, nameof(brokerOrderId));
                Validate.NotNull(orderLabel, nameof(orderLabel));
                Validate.PositiveDecimal(price, nameof(price));
                Validate.NotNull(expireTime, nameof(expireTime));
                Validate.NotDefault(timestamp, nameof(timestamp));

                var orderWorking = new OrderWorking(
                    new Symbol(symbol, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    new OrderId(brokerOrderId),
                    new Label(orderLabel),
                    orderSide,
                    orderType,
                    Quantity.Create(quantity),
                    Price.Create(price, this.pricePrecisionIndex[symbol]),
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
        /// <param name="symbol">The order symbol.</param>
        /// <param name="venue">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="timestamp">The event timestamp.</param>
        [SystemBoundary]
        public void OnOrderExpired(
            string symbol,
            Venue venue,
            string orderId,
            string brokerOrderId,
            string orderLabel,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Validate.NotNull(symbol, nameof(symbol));
                Validate.NotNull(orderId, nameof(orderId));
                Validate.NotNull(brokerOrderId, nameof(brokerOrderId));
                Validate.NotNull(orderLabel, nameof(orderLabel));
                Validate.NotDefault(timestamp, nameof(timestamp));

                var orderExpired = new OrderExpired(
                    new Symbol(symbol, venue),
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
        /// <param name="symbol">The order symbol.</param>
        /// <param name="venue">The order exchange.</param>
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
            string symbol,
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
                Validate.NotNull(symbol, nameof(symbol));
                Validate.NotNull(orderId, nameof(orderId));
                Validate.NotNull(brokerOrderId, nameof(brokerOrderId));
                Validate.NotNull(executionId, nameof(executionId));
                Validate.NotNull(executionTicket, nameof(executionTicket));
                Validate.NotNull(orderLabel, nameof(orderLabel));
                Validate.PositiveInt32(filledQuantity, nameof(filledQuantity));
                Validate.PositiveDecimal(averagePrice, nameof(averagePrice));
                Validate.NotDefault(timestamp, nameof(timestamp));

                var orderFilled = new OrderFilled(
                    new Symbol(symbol, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    new ExecutionId(executionId),
                    new ExecutionId(executionTicket),
                    orderSide,
                    Quantity.Create(filledQuantity),
                    Price.Create(averagePrice, this.pricePrecisionIndex[symbol]),
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
        /// <param name="symbol">The order symbol.</param>
        /// <param name="venue">The order exchange.</param>
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
            string symbol,
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
                Validate.NotNull(symbol, nameof(symbol));
                Validate.NotNull(orderId, nameof(orderId));
                Validate.NotNull(brokerOrderId, nameof(brokerOrderId));
                Validate.NotNull(executionId, nameof(executionId));
                Validate.NotNull(executionTicket, nameof(executionTicket));
                Validate.NotNull(orderLabel, nameof(orderLabel));
                Validate.PositiveInt32(filledQuantity, nameof(filledQuantity));
                Validate.PositiveInt32(leavesQuantity, nameof(leavesQuantity));
                Validate.PositiveDecimal(averagePrice, nameof(averagePrice));
                Validate.NotDefault(timestamp, nameof(timestamp));

                var orderPartiallyFilled = new OrderPartiallyFilled(
                    new Symbol(symbol, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    new ExecutionId(executionId),
                    new ExecutionId(executionTicket),
                    orderSide,
                    Quantity.Create(filledQuantity),
                    Quantity.Create(leavesQuantity),
                    Price.Create(averagePrice, this.pricePrecisionIndex[symbol]),
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
    }
}
