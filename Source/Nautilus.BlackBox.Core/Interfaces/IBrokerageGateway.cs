//--------------------------------------------------------------------------------------------------
// <copyright file="IBrokerageGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using System.Collections.Generic;
    using Akka.Actor;
    using NautechSystems.CSharp;
    using Nautilus.Common.Componentry;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The <see cref="IBrokerageGateway"/> interface.
    /// </summary>
    public interface IBrokerageGateway
    {
        /// <summary>
        /// Gets the brokerage gateways broker name.
        /// </summary>
        Broker Broker { get; }

        /// <summary>
        /// Gets the brokerage gateways command handler.
        /// </summary>
        CommandHandler Command { get; }

        /// <summary>
        /// Gets a value indicating whether the brokerage gateways broker client is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Connects the brokerage client.
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnects the brokerage client.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Returns the current time of the <see cref="BlackBox"/> system clock.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        ZonedDateTime GetTimeNow();

        /// <summary>
        /// Registers the given market data port actor address with this brokerage gateway.
        /// </summary>
        /// <param name="actorRef">The actor address.</param>
        void RegisterMarketDataPort(IActorRef actorRef);

        /// <summary>
        /// Requests market data for the given symbol from the brokerage.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        void RequestMarketDataSubscribe(Symbol symbol);

        /// <summary>
        /// Request an update on the instrument corresponding to the given symbol from the brokerage,
        /// and subscribe to updates.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        void UpdateInstrumentSubscribe(Symbol symbol);

        /// <summary>
        /// Requests an update on all instruments from the brokerage.
        /// </summary>
        void UpdateInstrumentsSubscribeAll();

        /// <summary>
        /// Submits an entry order with a stop-loss and profit target to the brokerage.
        /// </summary>
        /// <param name="order">The atomic order.</param>
        void SubmitEntryLimitStopOrder(AtomicOrder order);

        /// <summary>
        /// Submits an entry order with a stop-loss to the brokerage.
        /// </summary>
        /// <param name="order">The atomic order.</param>
        void SubmitEntryStopOrder(AtomicOrder order);

        /// <summary>
        /// Submits a request to modify the stop-loss of an existing order.
        /// </summary>
        /// <param name="stoplossModification">The stop-loss modification.</param>
        void ModifyStoplossOrder(KeyValuePair<Order, Price> stoplossModification);

        /// <summary>
        /// Submits a request to cancel the given order.
        /// </summary>
        /// <param name="order">The order.</param>
        void CancelOrder(Order order);

        /// <summary>
        /// Submits a request to close the given position to the brokerage client.
        /// </summary>
        /// <param name="position">The position.</param>
        void ClosePosition(Position position);

        /// <summary>
        /// Updates the given instruments in the <see cref="IInstrumentRepository"/>.
        /// </summary>
        /// <param name="instruments">The instruments collection.</param>
        /// <param name="responseId">The response identifier.</param>
        /// <param name="result">The result.</param>
        void OnInstrumentsUpdate(IReadOnlyCollection<Instrument> instruments, string responseId, string result);

        /// <summary>
        /// Event handler for receiving FIX business messages.
        /// </summary>
        /// <param name="message">The message.</param>
        void OnBusinessMessage(string message);

        /// <summary>
        /// Event handler for receiving FIX Collateral Inquiry Acknowledgements.
        /// </summary>
        /// <param name="inquiryId">The inquiry identifier.</param>
        /// <param name="accountNumber">The account number.</param>
        void OnCollateralInquiryAck(string inquiryId, string accountNumber);

        /// <summary>
        /// Event handler for acknowledgement of a request for positions.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="positionRequestId">The position request identifier.</param>
        void OnRequestForPositionsAck(string accountNumber, string positionRequestId);

        /// <summary>
        /// Event handler for receiving FIX Position Reports.
        /// </summary>
        /// <param name="account">The account.</param>
        void OnPositionReport(string account);

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
        void OnAccountReport(
            string inquiryId,
            string account,
            decimal cashBalance,
            decimal cashStartDay,
            decimal cashDaily,
            decimal marginUsedMaint,
            decimal marginUsedLiq,
            decimal marginRatio,
            string marginCallStatus,
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates a new <see cref="Tick"/> and sends it to the <see cref="IQuoteProvider"/> and
        /// the MarketDataPort.
        /// </summary>
        /// <param name="symbol">The quote symbol.</param>
        /// <param name="exchange">The quote exchange.</param>
        /// <param name="bid">The quote bid.</param>
        /// <param name="ask">The quote ask.</param>
        /// <param name="timestamp">The quote timestamp.</param>
        void OnQuote(
            string symbol,
            Exchange exchange,
            decimal bid,
            decimal ask,
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates an <see cref="OrderRejected"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbol">The order symbol.</param>
        /// <param name="exchange">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="rejectReason">The order reject reason.</param>
        /// <param name="timestamp">The event timestamp.</param>
        void OnOrderRejected(
            string symbol,
            Exchange exchange,
            string orderId,
            string rejectReason,
            ZonedDateTime timestamp);

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
        void OnOrderCancelReject(
            string symbol,
            Exchange exchange,
            string orderId,
            string brokerOrderId,
            string cancelRejectResponseTo,
            string cancelRejectReason,
            ZonedDateTime timestamp);

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
        void OnOrderCancelled(
            string symbol,
            Exchange exchange,
            string orderId,
            string brokerOrderId,
            string orderLabel,
            ZonedDateTime timestamp);

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
        void OnOrderModified(
            string symbol,
            Exchange exchange,
            string orderId,
            string brokerOrderId,
            string orderLabel,
            decimal price,
            ZonedDateTime timestamp);

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
        void OnOrderWorking(
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
            ZonedDateTime timestamp);

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
        void OnOrderExpired(
            string symbol,
            Exchange exchange,
            string orderId,
            string brokerOrderId,
            string orderLabel,
            ZonedDateTime timestamp);

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
        void OnOrderFilled(
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
            ZonedDateTime timestamp);

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
        void OnOrderPartiallyFilled(
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
            ZonedDateTime timestamp);
    }
}
