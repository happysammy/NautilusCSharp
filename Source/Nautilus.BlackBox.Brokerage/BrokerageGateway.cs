//--------------------------------------------------------------------------------------------------
// <copyright file="BrokerageGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Brokerage
{
    using System.Collections.Generic;
    using Akka.Actor;
    using Nautilus.Core;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The <see cref="BlackBox"/> boundary for the brokerage implementation.
    /// </summary>
    public sealed class BrokerageGateway : ComponentBusConnectedBase, IBrokerageGateway
    {
        private readonly IInstrumentRepository instrumentRepository;
        private readonly IQuoteProvider quoteProvider;
        private readonly IReadOnlyBrokerageAccount brokerageAccount;
        private readonly IBrokerageClient brokerageClient;

        private IActorRef marketDataPortRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerageGateway"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="brokerageClient">The brokerage client.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public BrokerageGateway(
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter,
            IBrokerageClient brokerageClient)
            : base(
                BlackBoxService.Brokerage,
                new Label(nameof(BrokerageGateway)),
                container,
                messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(brokerageClient, nameof(brokerageClient));

            this.instrumentRepository = container.InstrumentRepository;
            this.quoteProvider = container.QuoteProvider;
            this.brokerageAccount = container.Account;
            this.brokerageClient = brokerageClient;
        }

        /// <summary>
        /// Gets the brokerage gateways broker name.
        /// </summary>
        public Broker Broker => this.brokerageClient.Broker;

        /// <summary>
        /// Gets a value indicating whether the brokerage gateways broker client is connected.
        /// </summary>
        public bool IsConnected => this.brokerageClient.IsConnected;

        /// <summary>
        /// Initializes the brokerage session.
        /// </summary>
        public void InitializeSession()
        {
            this.brokerageClient.InitializeSession();
        }

        /// <summary>
        /// Returns the current time of the <see cref="BlackBox"/> system clock.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public ZonedDateTime GetTimeNow()
        {
            return this.TimeNow();
        }

        /// <summary>
        /// Registers the given market data port actor address with this brokerage gateway.
        /// </summary>
        /// <param name="actorRef">The actor address.</param>
        public void RegisterMarketDataPort(IActorRef actorRef)
        {
            this.Execute(() =>
            {
                Validate.NotNull(actorRef, nameof(actorRef));

                this.marketDataPortRef = actorRef;

                this.Log.Information($"MarketDataPort registered at {this.marketDataPortRef}");
            });
        }

        /// <summary>
        /// Connects the brokerage client.
        /// </summary>
        public void Connect()
        {
            this.brokerageClient.Connect();
        }

        /// <summary>
        /// Disconnects the brokerage client.
        /// </summary>
        public void Disconnect()
        {
            this.brokerageClient.Disconnect();
        }

        /// <summary>
        /// Requests market data for the given symbol from the brokerage.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <exception cref="ValidationException">Throws if the symbol is null.</exception>
        public void RequestMarketDataSubscribe(Symbol symbol)
        {
            Validate.NotNull(symbol, nameof(symbol));

            this.brokerageClient.RequestMarketDataSubscribe(symbol);
        }

        /// <summary>
        /// Request an update on the instrument corresponding to the given symbol from the brokerage,
        /// and subscribe to updates.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <exception cref="ValidationException">Throws if the symbol is null.</exception>
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            Validate.NotNull(symbol, nameof(symbol));

            this.brokerageClient.UpdateInstrumentSubscribe(symbol);
        }

        /// <summary>
        /// Requests an update on all instruments from the brokerage.
        /// </summary>
        public void UpdateInstrumentsSubscribeAll()
        {
            this.brokerageClient.UpdateInstrumentsSubscribeAll();
        }

        /// <summary>
        /// Submits an entry order with a stop-loss and profit target to the brokerage.
        /// </summary>
        /// <param name="order">The atomic order.</param>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public void SubmitEntryLimitStopOrder(AtomicOrder order)
        {
            Validate.NotNull(order, nameof(order));

            this.brokerageClient.SubmitEntryLimitStopOrder(order);
        }

        /// <summary>
        /// Submits an entry order with a stop-loss to the brokerage.
        /// </summary>
        /// <param name="order">The atomic order.</param>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public void SubmitEntryStopOrder(AtomicOrder order)
        {
            Validate.NotNull(order, nameof(order));

            this.brokerageClient.SubmitEntryStopOrder(order);
        }

        /// <summary>
        /// Submits a request to modify the stop-loss of an existing order.
        /// </summary>
        /// <param name="stoplossModification">The stop-loss modification.</param>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public void ModifyStoplossOrder(KeyValuePair<Order, Price> stoplossModification)
        {
            Validate.NotNull(stoplossModification, nameof(stoplossModification));

            this.brokerageClient.ModifyStoplossOrder(stoplossModification);
        }

        /// <summary>
        /// Submits a request to cancel the given order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public void CancelOrder(Order order)
        {
            Validate.NotNull(order, nameof(order));

            this.brokerageClient.CancelOrder(order);
        }

        /// <summary>
        /// Submits a request to close the given position to the brokerage client.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public void ClosePosition(Position position)
        {
            Validate.NotNull(position, nameof(position));

            this.brokerageClient.ClosePosition(position);
        }

        /// <summary>
        /// Event handler for receiving FIX Position Reports.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public void OnPositionReport(string account)
        {
            Validate.NotNull(account, nameof(account));

            this.Log.Debug($"PositionReport: ({account})");
        }

        /// <summary>
        /// Event handler for receiving FIX Collateral Inquiry Acknowledgements.
        /// </summary>
        /// <param name="inquiryId">The inquiry identifier.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <exception cref="ValidationException">Throws if the either argument is null.</exception>
        public void OnCollateralInquiryAck(string inquiryId, string accountNumber)
        {
            Validate.NotNull(inquiryId, nameof(inquiryId));
            Validate.NotNull(accountNumber, nameof(accountNumber));

            this.Log.Debug($"CollateralInquiryAck: ({this.Broker}-{accountNumber}, InquiryId={inquiryId})");
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

                this.instrumentRepository.UpdateInstruments(instruments);

                this.Log.Debug($"SecurityListReceived: (SecurityResponseId={responseId}) result={result}");
            });
        }

        /// <summary>
        /// Event handler for acknowledgement of a request for positions.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="positionRequestId">The position request identifier.</param>
        public void OnRequestForPositionsAck(string accountNumber, string positionRequestId)
        {
            Validate.NotNull(accountNumber, nameof(accountNumber));
            Validate.NotNull(positionRequestId, nameof(positionRequestId));

            this.Log.Debug($"RequestForPositionsAck Received ({accountNumber}-{positionRequestId})");
        }

        /// <summary>
        /// Creates an <see cref="AccountEvent"/> event, and sends it to the Risk Service via the
        /// Messaging system.
        /// </summary>
        /// <param name="inquiryId">The inquiry identifier.</param>
        /// <param name="account">The account.</param>
        /// <param name="cashBalance">The cash balance.</param>
        /// <param name="cashStartDay">The cash start day.</param>
        /// <param name="cashDaily">The cash daily.</param>
        /// <param name="marginUsedMaint">The margin used maintenance.</param>
        /// <param name="marginUsedLiq">The margin used liquidity.</param>
        /// <param name="marginRatio">The margin ratio.</param>
        /// <param name="marginCallStatus">The margin call status.</param>
        /// <param name="timestamp">The report timestamp.</param>
        public void OnAccountReport(
            string inquiryId,
            string account,
            decimal cashBalance,
            decimal cashStartDay,
            decimal cashDaily,
            decimal marginUsedMaint,
            decimal marginUsedLiq,
            decimal marginRatio,
            string marginCallStatus,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Validate.NotNull(inquiryId, nameof(inquiryId));
                Validate.NotNull(account, nameof(account));
                Validate.DecimalNotOutOfRange(cashBalance, nameof(cashBalance), decimal.Zero, decimal.MaxValue);
                Validate.DecimalNotOutOfRange(cashStartDay, nameof(cashStartDay), decimal.Zero, decimal.MaxValue);
                Validate.DecimalNotOutOfRange(cashDaily, nameof(cashDaily), decimal.Zero, decimal.MaxValue);
                Validate.DecimalNotOutOfRange(marginUsedMaint, nameof(marginUsedMaint), decimal.Zero, decimal.MaxValue);
                Validate.DecimalNotOutOfRange(marginUsedLiq, nameof(marginUsedLiq), decimal.Zero, decimal.MaxValue);
                Validate.DecimalNotOutOfRange(marginRatio, nameof(marginRatio), decimal.Zero, decimal.MaxValue);
                Validate.NotNull(marginCallStatus, nameof(marginCallStatus));
                Validate.NotDefault(timestamp, nameof(timestamp));

                var accountEvent = new AccountEvent(
                    this.brokerageAccount.Broker,
                    account,
                    this.GetMoneyType(cashBalance),
                    this.GetMoneyType(cashStartDay),
                    this.GetMoneyType(cashDaily),
                    this.GetMoneyType(marginUsedLiq),
                    this.GetMoneyType(marginUsedMaint),
                    marginRatio,
                    marginCallStatus,
                    this.NewGuid(),
                    timestamp);

                var eventMessage = new EventMessage(
                    accountEvent,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(BlackBoxService.Risk, eventMessage);

                this.Log.Debug($"AccountReport: ({Broker.FXCM}-{account}, InquiryId={inquiryId})");
            });
        }

        /// <summary>
        /// Creates a new <see cref="Tick"/> and sends it to the <see cref="IQuoteProvider"/> and
        /// the MarketDataPort.
        /// </summary>
        /// <param name="symbol">The quote symbol.</param>
        /// <param name="exchange">The quote exchange.</param>
        /// <param name="bid">The quote bid.</param>
        /// <param name="ask">The quote ask.</param>
        /// <param name="timestamp">The quote timestamp.</param>
        public void OnQuote(
            string symbol,
            Exchange exchange,
            decimal bid,
            decimal ask,
            ZonedDateTime timestamp)
        {
            this.Execute(() =>
            {
                Validate.NotNull(symbol, nameof(symbol));
                Validate.DecimalNotOutOfRange(bid, nameof(bid), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);
                Validate.DecimalNotOutOfRange(ask, nameof(ask), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);

                var securitySymbol = new Symbol(symbol, exchange);
                var tickSize = this.instrumentRepository.GetTickSize(securitySymbol).Value;

            var quote = new Tick(
                    securitySymbol,
                    Price.Create(bid, tickSize),
                    Price.Create(ask, tickSize),
                    timestamp);

                this.marketDataPortRef.Tell(quote);
                this.quoteProvider.OnQuote(quote);
            });
        }

        /// <summary>
        /// Creates an <see cref="OrderRejected"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="exchange">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="rejectReason">The order reject reason.</param>
        /// <param name="timestamp">The event timestamp.</param>
        public void OnOrderRejected(
            string symbol,
            Exchange exchange,
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

                var orderEvent = new OrderRejected(
                    new Symbol(symbol, exchange),
                    new EntityId(OrderIdPostfixRemover.Remove(orderId)),
                    timestamp,
                    rejectReason,
                    this.NewGuid(),
                    this.TimeNow());

                var eventMessage = new EventMessage(
                    orderEvent,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(BlackBoxService.Portfolio, eventMessage);

                this.Log.Warning($"OrderRejected: (OrderId={orderId}, RejectReason={rejectReason}");
            });
        }

        /// <summary>
        /// Creates an <see cref="OrderCancelReject"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="exchange">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="cancelRejectResponseTo">The order cancel reject response to.</param>
        /// <param name="cancelRejectReason">The order cancel reject reason.</param>
        /// <param name="timestamp">The event timestamp.</param>
        public void OnOrderCancelReject(
            string symbol,
            Exchange exchange,
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

                    var orderEvent = new OrderCancelReject(
                        ConvertStringToSymbol(symbol, exchange),
                        new EntityId(OrderIdPostfixRemover.Remove(orderId)),
                        timestamp,
                        cancelRejectResponseTo,
                        cancelRejectReason,
                        this.NewGuid(),
                        this.TimeNow());

                    var eventMessage = new EventMessage(
                        orderEvent,
                        this.NewGuid(),
                        this.TimeNow());

                    this.Send(BlackBoxService.Portfolio, eventMessage);

                    this.Log.Warning($"OrderCancelReject: (OrderId={orderId}, CxlRejResponseTo={cancelRejectResponseTo}, Reason={cancelRejectReason}");
                });
        }

        /// <summary>
        /// Creates an <see cref="OrderCancelled"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="exchange">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="orderLabel">The order Label.</param>
        /// <param name="timestamp">The event timestamp.</param>
        public void OnOrderCancelled(
            string symbol,
            Exchange exchange,
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

                var orderEvent = new OrderCancelled(
                    ConvertStringToSymbol(symbol, exchange),
                    new EntityId(OrderIdPostfixRemover.Remove(orderId)),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                var eventMessage = new EventMessage(
                    orderEvent,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(BlackBoxService.Portfolio, eventMessage);

                this.Log.Information($"OrderCancelled: {orderLabel} (OrderId={orderId}, BrokerOrderId={brokerOrderId})");
            });
        }

        /// <summary>
        /// Creates an <see cref="OrderModified"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="exchange">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="price">The order price.</param>
        /// <param name="timestamp">The event timestamp.</param>
        public void OnOrderModified(
            string symbol,
            Exchange exchange,
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
                Validate.DecimalNotOutOfRange(price, nameof(price), decimal.Zero, decimal.MaxValue);
                Validate.NotDefault(timestamp, nameof(timestamp));

                var symbolType = ConvertStringToSymbol(symbol, exchange);

                var orderEvent = new OrderModified(
                    symbolType,
                    new EntityId(OrderIdPostfixRemover.Remove(orderId)),
                    new EntityId(brokerOrderId),
                    Price.Create(price, this.instrumentRepository.GetTickSize(symbolType).Value),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                var eventMessage = new EventMessage(
                    orderEvent,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(BlackBoxService.Portfolio, eventMessage);

                this.Log.Information($"OrderModified: {orderLabel} (OrderId={orderId}, BrokerOrderId={brokerOrderId}, Price={price})");
            });
        }

        /// <summary>
        /// Creates an <see cref="OrderWorking"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="exchange">The order exchange.</param>
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
        public void OnOrderWorking(
            string symbol,
            Exchange exchange,
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
                Validate.DecimalNotOutOfRange(price, nameof(price), decimal.Zero, decimal.MaxValue);
                Validate.NotNull(expireTime, nameof(expireTime));
                Validate.NotDefault(timestamp, nameof(timestamp));

                var symbolType = ConvertStringToSymbol(symbol, exchange);

                var orderEvent = new OrderWorking(
                    symbolType,
                    new EntityId(OrderIdPostfixRemover.Remove(orderId)),
                    new EntityId(brokerOrderId),
                    new Label(orderLabel),
                    orderSide,
                    orderType,
                    Quantity.Create(quantity),
                    Price.Create(price, this.instrumentRepository.GetTickSize(symbolType).Value),
                    timeInForce,
                    expireTime,
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                var eventMessage = new EventMessage(
                    orderEvent,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(BlackBoxService.Portfolio, eventMessage);

                var expireTimeString = string.Empty;

                if (expireTime.HasValue)
                {
                    expireTimeString = expireTime.Value.ToIsoString();
                }

                this.Log.Information($"OrderWorking: {orderLabel} (OrderId={orderId}, BrokerOrderId={brokerOrderId}, Price={price}, ExpireTime={expireTimeString})");
            });
        }

        /// <summary>
        /// Creates an <see cref="OrderExpired"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="exchange">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="timestamp">The event timestamp.</param>
        public void OnOrderExpired(
            string symbol,
            Exchange exchange,
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

                var orderEvent = new OrderExpired(
                    ConvertStringToSymbol(symbol, exchange),
                    new EntityId(OrderIdPostfixRemover.Remove(orderId)),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                var eventMessage = new EventMessage(
                    orderEvent,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(BlackBoxService.Portfolio, eventMessage);

                this.Log.Information($"OrderExpired: {orderLabel} (OrderId={orderId}, BrokerOrderId={brokerOrderId})");
            });
        }

        /// <summary>
        /// Creates an <see cref="OrderFilled"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="exchange">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="executionId">The order execution identifier.</param>
        /// <param name="executionTicket">The order execution ticket.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="orderSide">The order side.</param>
        /// <param name="filledQuantity">The order filled quantity.</param>
        /// <param name="averagePrice">The order average price.</param>
        /// <param name="timestamp">The event timestamp.</param>
        public void OnOrderFilled(
            string symbol,
            Exchange exchange,
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
                Validate.Int32NotOutOfRange(filledQuantity, nameof(filledQuantity), 0, int.MaxValue);
                Validate.DecimalNotOutOfRange(averagePrice, nameof(averagePrice), decimal.Zero, decimal.MaxValue);
                Validate.NotDefault(timestamp, nameof(timestamp));

                var symbolType = ConvertStringToSymbol(symbol, exchange);

                var orderEvent = new OrderFilled(
                    symbolType,
                    new EntityId(OrderIdPostfixRemover.Remove(orderId)),
                    new EntityId(executionId),
                    new EntityId(executionTicket),
                    orderSide,
                    Quantity.Create(filledQuantity),
                    Price.Create(averagePrice, this.instrumentRepository.GetTickSize(symbolType).Value),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                var eventMessage = new EventMessage(
                    orderEvent,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(BlackBoxService.Portfolio, eventMessage);

                this.Log.Information($"OrderFilled: {orderLabel} (OrderId={orderId}, BrokerOrderId={brokerOrderId}, ExecutionId={executionId}, ExecutionTicket={executionTicket}, FilledQty={filledQuantity} at {averagePrice})");
            });
        }

        /// <summary>
        /// Creates an <see cref="OrderPartiallyFilled"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="exchange">The order exchange.</param>
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
        public void OnOrderPartiallyFilled(
            string symbol,
            Exchange exchange,
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
                Validate.Int32NotOutOfRange(filledQuantity, nameof(filledQuantity), 0, int.MaxValue, RangeEndPoints.Exclusive);
                Validate.Int32NotOutOfRange(leavesQuantity, nameof(leavesQuantity), 0, int.MaxValue, RangeEndPoints.Exclusive);
                Validate.DecimalNotOutOfRange(averagePrice, nameof(averagePrice), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);
                Validate.NotDefault(timestamp, nameof(timestamp));

                var symbolType = ConvertStringToSymbol(symbol, exchange);

                var orderEvent = new OrderPartiallyFilled(
                    symbolType,
                    new EntityId(OrderIdPostfixRemover.Remove(orderId)),
                    new EntityId(executionId),
                    new EntityId(executionTicket),
                    orderSide,
                    Quantity.Create(filledQuantity),
                    Quantity.Create(leavesQuantity),
                    Price.Create(averagePrice, this.instrumentRepository.GetTickSize(symbolType).Value),
                    timestamp,
                    this.NewGuid(),
                    this.TimeNow());

                var eventMessage = new EventMessage(
                    orderEvent,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(BlackBoxService.Portfolio, eventMessage);

                this.Log.Information($"OrderPartiallyFilled: {orderLabel} (OrderId={orderId}, BrokerOrderId={brokerOrderId}, ExecutionId={executionId}, ExecutionTicket={executionTicket}, FilledQty={filledQuantity} at {averagePrice}, LeavesQty={leavesQuantity})");
            });
        }

        private static Symbol ConvertStringToSymbol(string symbolString, Exchange exchange)
        {
            Debug.NotNull(symbolString, nameof(symbolString));

            return symbolString != "NONE"
                       ? new Symbol(symbolString, exchange)
                       : new Symbol("AUDUSD", Exchange.FXCM);
        }

        private Money GetMoneyType(decimal amount)
        {
            Debug.DecimalNotOutOfRange(amount, nameof(amount), decimal.Zero, decimal.MaxValue);

            return amount > 0
                ? Money.Create(amount, this.brokerageAccount.Currency)
                : Money.Zero(this.brokerageAccount.Currency);
        }
    }
}
