//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
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
    /// The system boundary for the trading implementation.
    /// </summary>
    public sealed class ExecutionGateway : ComponentBusConnectedBase, IExecutionGateway
    {
        private readonly IInstrumentRepository instrumentRepository;
        private readonly IFixClient fixClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionGateway"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="fixClient">The trade client.</param>
        /// <param name="instrumentRepository">The instrument repository.</param>
        public ExecutionGateway(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IFixClient fixClient,
            IInstrumentRepository instrumentRepository)
            : base(
                NautilusService.Execution,
                new Label(nameof(ExecutionGateway)),
                container,
                messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(fixClient, nameof(fixClient));
            Validate.NotNull(instrumentRepository, nameof(instrumentRepository));

            this.fixClient = fixClient;
            this.instrumentRepository = instrumentRepository;
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
        /// Connects the brokerage client.
        /// </summary>
        public void Connect()
        {
            this.fixClient.Connect();
        }

        /// <summary>
        /// Disconnects the brokerage client.
        /// </summary>
        public void Disconnect()
        {
            this.fixClient.Disconnect();
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
        /// Event handler for receiving FIX Position Reports.
        /// </summary>
        /// <param name="account">The account.</param>
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
                Validate.DecimalNotOutOfRange(cashBalance, nameof(cashBalance), decimal.Zero, decimal.MaxValue);
                Validate.DecimalNotOutOfRange(cashStartDay, nameof(cashStartDay), decimal.Zero, decimal.MaxValue);
                Validate.DecimalNotOutOfRange(cashDaily, nameof(cashDaily), decimal.Zero, decimal.MaxValue);
                Validate.DecimalNotOutOfRange(marginUsedMaintenance, nameof(marginUsedMaintenance), decimal.Zero, decimal.MaxValue);
                Validate.DecimalNotOutOfRange(marginUsedLiq, nameof(marginUsedLiq), decimal.Zero, decimal.MaxValue);
                Validate.DecimalNotOutOfRange(marginRatio, nameof(marginRatio), decimal.Zero, decimal.MaxValue);
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

                this.Send(NautilusService.Portfolio, accountEvent);

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

                this.Send(NautilusService.Portfolio, orderRejected);

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
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="cancelRejectResponseTo">The order cancel reject response to.</param>
        /// <param name="cancelRejectReason">The order cancel reject reason.</param>
        /// <param name="timestamp">The event timestamp.</param>
        public void OnOrderCancelReject(
            string symbol,
            Venue venue,
            string orderId,
            string brokerOrderId,
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

                this.Send(NautilusService.Portfolio, orderCancelReject);

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

                this.Send(NautilusService.Portfolio, orderCancelled);

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
        /// <param name="decimals">The price decimal precision.</param>
        /// <param name="timestamp">The event timestamp.</param>
        public void OnOrderModified(
            string symbol,
            Venue venue,
            string orderId,
            string brokerOrderId,
            string orderLabel,
            decimal price,
            int decimals,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Validate.NotNull(symbol, nameof(symbol));
                Validate.NotNull(orderId, nameof(orderId));
                Validate.NotNull(brokerOrderId, nameof(brokerOrderId));
                Validate.NotNull(orderLabel, nameof(orderLabel));
                Validate.DecimalNotOutOfRange(price, nameof(price), decimal.Zero, decimal.MaxValue);
                Validate.NotDefault(timestamp, nameof(timestamp));

                var orderModified = new OrderModified(
                    new Symbol(symbol, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    new OrderId(brokerOrderId),
                    Price.Create(price, decimals),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(NautilusService.Portfolio, orderModified);

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
        /// <param name="decimals">The price decimal precision.</param>
        /// <param name="timeInForce">The order time in force.</param>
        /// <param name="expireTime">The order expire time.</param>
        /// <param name="timestamp">The event timestamp.</param>
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
            int decimals,
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
                Validate.DecimalNotOutOfRange(price, nameof(price), decimal.Zero, decimal.MaxValue);
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
                    Price.Create(price, decimals),
                    timeInForce,
                    expireTime,
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(NautilusService.Portfolio, orderWorking);

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

                this.Send(NautilusService.Portfolio, orderExpired);

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
        /// <param name="decimals">The decimal precision for the price.</param>
        /// <param name="timestamp">The event timestamp.</param>
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
            int decimals,
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
                Validate.Int32NotOutOfRange(filledQuantity, nameof(filledQuantity), 0, int.MaxValue);
                Validate.DecimalNotOutOfRange(averagePrice, nameof(averagePrice), decimal.Zero, decimal.MaxValue);
                Validate.NotDefault(timestamp, nameof(timestamp));

                var orderFilled = new OrderFilled(
                    new Symbol(symbol, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    new ExecutionId(executionId),
                    new ExecutionId(executionTicket),
                    orderSide,
                    Quantity.Create(filledQuantity),
                    Price.Create(averagePrice, decimals),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(NautilusService.Portfolio, orderFilled);

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
        /// <param name="decimals">The decimal precision of the price.</param>
        /// <param name="timestamp">The event timestamp.</param>
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
            int decimals,
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
                Validate.Int32NotOutOfRange(filledQuantity, nameof(filledQuantity), 0, int.MaxValue, RangeEndPoints.Exclusive);
                Validate.Int32NotOutOfRange(leavesQuantity, nameof(leavesQuantity), 0, int.MaxValue, RangeEndPoints.Exclusive);
                Validate.DecimalNotOutOfRange(averagePrice, nameof(averagePrice), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);
                Validate.NotDefault(timestamp, nameof(timestamp));

                var orderPartiallyFilled = new OrderPartiallyFilled(
                    new Symbol(symbol, venue),
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    new ExecutionId(executionId),
                    new ExecutionId(executionTicket),
                    orderSide,
                    Quantity.Create(filledQuantity),
                    Quantity.Create(leavesQuantity),
                    Price.Create(averagePrice, decimals),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(NautilusService.Portfolio, orderPartiallyFilled);

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
