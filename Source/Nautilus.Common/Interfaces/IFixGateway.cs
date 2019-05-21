//--------------------------------------------------------------------------------------------------
// <copyright file="IFixGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.Core;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Messaging;
    using Nautilus.Messaging.Interfaces;
    using NodaTime;

    /// <summary>
    /// Provides a gateway and anti-corruption layer into the system.
    /// </summary>
    public interface IFixGateway
    {
        /// <summary>
        /// Gets the gateways broker name.
        /// </summary>
        Brokerage Broker { get; }

        /// <summary>
        /// Gets a value indicating whether the brokerage gateways broker client is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Registers the receiver to receive <see cref="Tick"/>s from the gateway.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        void RegisterTickReceiver(IEndpoint receiver);

        /// <summary>
        /// Registers the receiver endpoint to receive connection events from the gateway.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        void RegisterConnectionEventReceiver(Address receiver);

        /// <summary>
        /// Registers the receiver to receive <see cref="Event"/>s from the gateway.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        void RegisterEventReceiver(Address receiver);

        /// <summary>
        /// Registers the receiver to receive <see cref="Instrument"/> updates from the gateway.
        /// </summary>
        /// <param name="receiver">The receiver.</param>
        void RegisterInstrumentReceiver(Address receiver);

        /// <summary>
        /// Submits a market data subscribe FIX message for the given symbol to the brokerage.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        void MarketDataSubscribe(Symbol symbol);

        /// <summary>
        /// Submits a market data subscribe all FIX message for all symbols to the brokerage.
        /// </summary>
        void MarketDataSubscribeAll();

        /// <summary>
        /// Submits an update on the instrument for the given symbol, and subscribe to updates FIX
        /// message, to the brokerage.
        /// </summary>
        /// <param name="symbol">The symbol of the instrument to update.</param>
        void UpdateInstrumentSubscribe(Symbol symbol);

        /// <summary>
        /// Submits an update all instruments FIX message to the brokerage.
        /// </summary>
        void UpdateInstrumentsSubscribeAll();

        /// <summary>
        /// Submits a collateral inquiry FIX message to the brokerage.
        /// </summary>
        void CollateralInquiry();

        /// <summary>
        /// Submits a trading session status FIX message to the brokerage.
        /// </summary>
        void TradingSessionStatus();

        /// <summary>
        /// Submits a new order FIX message to the brokerage.
        /// </summary>
        /// <param name="order">The new order.</param>
        void SubmitOrder(Order order);

        /// <summary>
        /// Submits a new atomic order FIX message to the brokerage.
        /// </summary>
        /// <param name="atomicOrder">The new atomic order.</param>
        void SubmitOrder(AtomicOrder atomicOrder);

        /// <summary>
        /// Submits an order cancel replace FIX message to modify the stop-loss of an existing order,
        /// to the brokerage.
        /// </summary>
        /// <param name="order">The order to modify.</param>
        /// <param name="modifiedPrice">The modified order price.</param>
        void ModifyOrder(Order order, Price modifiedPrice);

        /// <summary>
        /// Submits a cancel order FIX message to the brokerage.
        /// </summary>
        /// <param name="order">The order to cancel.</param>
        void CancelOrder(Order order);

        /// <summary>
        /// Creates a new <see cref="Tick"/> and sends it to the tick publisher and bar aggregation
        /// controller.
        /// </summary>
        /// <param name="symbolCode">The tick symbol.</param>
        /// <param name="venue">The tick exchange.</param>
        /// <param name="bid">The tick bid price.</param>
        /// <param name="ask">The tick ask price.</param>
        /// <param name="timestamp">The tick timestamp.</param>
        void OnTick(
            string symbolCode,
            Venue venue,
            decimal bid,
            decimal ask,
            ZonedDateTime timestamp);

        /// <summary>
        /// Event handler for receiving FIX Position Reports.
        /// </summary>
        /// <param name="account">The account.</param>
        void OnPositionReport(string account);

        /// <summary>
        /// Updates the given instruments in the instrument repository.
        /// </summary>
        /// <param name="instruments">The instruments collection.</param>
        /// <param name="responseId">The response identifier.</param>
        /// <param name="result">The result.</param>
        void OnInstrumentsUpdate(
            IEnumerable<Instrument> instruments,
            string responseId,
            string result);

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
        /// Creates an <see cref="AccountEvent"/> event, and sends it to the Risk Service via the
        /// Messaging system.
        /// </summary>
        /// <param name="inquiryId">The inquiry identifier.</param>
        /// <param name="accountNumber">The account.</param>
        /// <param name="cashBalance">The cash balance.</param>
        /// <param name="cashStartDay">The cash start day.</param>
        /// <param name="cashDaily">The cash daily.</param>
        /// <param name="marginUsedMaintenance">The margin used maintenance.</param>
        /// <param name="marginUsedLiq">The margin used liquidity.</param>
        /// <param name="marginRatio">The margin ratio.</param>
        /// <param name="marginCallStatus">The margin call status.</param>
        /// <param name="timestamp">The report timestamp.</param>
        void OnAccountReport(
            string inquiryId,
            string accountNumber,
            decimal cashBalance,
            decimal cashStartDay,
            decimal cashDaily,
            decimal marginUsedMaintenance,
            decimal marginUsedLiq,
            decimal marginRatio,
            string marginCallStatus,
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates an <see cref="OrderRejected"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbolCode">The order symbol.</param>
        /// <param name="venue">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="rejectReason">The order reject reason.</param>
        /// <param name="timestamp">The event timestamp.</param>
        void OnOrderRejected(
            string symbolCode,
            Venue venue,
            string orderId,
            string rejectReason,
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates an <see cref="OrderCancelReject"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbolCode">The order symbol.</param>
        /// <param name="venue">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="cancelRejectResponseTo">The order cancel reject response to.</param>
        /// <param name="cancelRejectReason">The order cancel reject reason.</param>
        /// <param name="timestamp">The event timestamp.</param>
        void OnOrderCancelReject(
            string symbolCode,
            Venue venue,
            string orderId,
            string cancelRejectResponseTo,
            string cancelRejectReason,
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates an <see cref="OrderCancelled"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbolCode">The order symbol.</param>
        /// <param name="venue">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="orderLabel">The order Label.</param>
        /// <param name="timestamp">The event timestamp.</param>
        void OnOrderCancelled(
            string symbolCode,
            Venue venue,
            string orderId,
            string brokerOrderId,
            string orderLabel,
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates an <see cref="OrderModified"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbolCode">The order symbol.</param>
        /// <param name="venue">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="price">The order price.</param>
        /// <param name="timestamp">The event timestamp.</param>
        void OnOrderModified(
            string symbolCode,
            Venue venue,
            string orderId,
            string brokerOrderId,
            string orderLabel,
            decimal price,
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates an <see cref="OrderWorking"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbolCode">The order symbol.</param>
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
        void OnOrderWorking(
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
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates an <see cref="OrderExpired"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbolCode">The order symbol.</param>
        /// <param name="venue">The order exchange.</param>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="brokerOrderId">The order broker order identifier.</param>
        /// <param name="orderLabel">The order label.</param>
        /// <param name="timestamp">The event timestamp.</param>
        void OnOrderExpired(
            string symbolCode,
            Venue venue,
            string orderId,
            string brokerOrderId,
            string orderLabel,
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates an <see cref="OrderFilled"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbolCode">The order symbol.</param>
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
        void OnOrderFilled(
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
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates an <see cref="OrderPartiallyFilled"/> event, and sends it to the Portfolio
        /// Service via the Messaging system.
        /// </summary>
        /// <param name="symbolCode">The order symbol.</param>
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
        void OnOrderPartiallyFilled(
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
            ZonedDateTime timestamp);
    }
}
