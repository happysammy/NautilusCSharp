//--------------------------------------------------------------------------------------------------
// <copyright file="ITradingGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;
    using Venue = Nautilus.DomainModel.Identifiers.Venue;

    /// <summary>
    /// Provides a gateway to, and anti-corruption layer from a trading gateway.
    /// </summary>
    public interface ITradingGateway
    {
        /// <summary>
        /// Gets the gateways brokerage name.
        /// </summary>
        Brokerage Brokerage { get; }

        /// <summary>
        /// Gets the account identifier.
        /// </summary>
        AccountId AccountId { get; }

        /// <summary>
        /// Gets a value indicating whether the gateway is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Sends a account inquiry request message.
        /// </summary>
        void AccountInquiry();

        /// <summary>
        /// Sends a trading session status request message.
        /// </summary>
        void TradingSessionStatus();

        /// <summary>
        /// Sends a message to submit the given order.
        /// </summary>
        /// <param name="order">The new order.</param>
        void SubmitOrder(Order order);

        /// <summary>
        /// Sends a message to submit the given atomic order.
        /// </summary>
        /// <param name="atomicOrder">The new atomic order.</param>
        void SubmitOrder(AtomicOrder atomicOrder);

        /// <summary>
        /// Sends a message to modify the given order with the given price.
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
        /// Event handler for receiving FIX Position Reports.
        /// </summary>
        /// <param name="account">The account.</param>
        void OnPositionReport(string account);

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
        /// Creates an <see cref="AccountStateEvent"/> event and sends it to the registered event receivers.
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
        /// Creates an <see cref="OrderRejected"/> event and sends it to the registered event receivers.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="rejectReason">The order reject reason.</param>
        /// <param name="timestamp">The event timestamp.</param>
        void OnOrderRejected(
            string orderId,
            string rejectReason,
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates an <see cref="OrderCancelReject"/> event and sends it to the registered event receivers.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="cancelRejectResponseTo">The order cancel reject response to.</param>
        /// <param name="cancelRejectReason">The order cancel reject reason.</param>
        /// <param name="timestamp">The event timestamp.</param>
        void OnOrderCancelReject(
            string orderId,
            string cancelRejectResponseTo,
            string cancelRejectReason,
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates an <see cref="OrderCancelled"/> event and sends it to the registered event receivers.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderIdBroker">The order broker order identifier.</param>
        /// <param name="label">The order Label.</param>
        /// <param name="timestamp">The event timestamp.</param>
        void OnOrderCancelled(
            string orderId,
            string orderIdBroker,
            string label,
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates an <see cref="OrderModified"/> event and sends it to the registered event receivers.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderIdBroker">The order broker order identifier.</param>
        /// <param name="label">The order label.</param>
        /// <param name="price">The order price.</param>
        /// <param name="timestamp">The event timestamp.</param>
        void OnOrderModified(
            string orderId,
            string orderIdBroker,
            string label,
            decimal price,
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates an <see cref="OrderWorking"/> event and sends it to the registered event receivers.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderIdBroker">The order broker order identifier.</param>
        /// <param name="symbolCode">The order symbol code.</param>
        /// <param name="venue">The order venue.</param>
        /// <param name="label">The order label.</param>
        /// <param name="side">The order side.</param>
        /// <param name="type">The order type.</param>
        /// <param name="quantity">The order quantity.</param>
        /// <param name="price">The order price.</param>
        /// <param name="timeInForce">The order time in force.</param>
        /// <param name="expireTime">The order expire time.</param>
        /// <param name="timestamp">The event timestamp.</param>
        void OnOrderWorking(
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
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates an <see cref="OrderExpired"/> event and sends it to the registered event receivers.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderIdBroker">The order broker order identifier.</param>
        /// <param name="label">The order label.</param>
        /// <param name="timestamp">The event timestamp.</param>
        void OnOrderExpired(
            string orderId,
            string orderIdBroker,
            string label,
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates an <see cref="OrderFilled"/> event and sends it to the registered event receivers.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderIdBroker">The order broker order identifier.</param>
        /// <param name="executionId">The order execution identifier.</param>
        /// <param name="executionTicket">The order execution ticket.</param>
        /// <param name="symbolCode">The order symbol code.</param>
        /// <param name="venue">The order venue.</param>
        /// <param name="side">The order side.</param>
        /// <param name="filledQuantity">The order filled quantity.</param>
        /// <param name="averagePrice">The order average price.</param>
        /// <param name="timestamp">The event timestamp.</param>
        void OnOrderFilled(
            string orderId,
            string orderIdBroker,
            string executionId,
            string executionTicket,
            string symbolCode,
            Venue venue,
            OrderSide side,
            int filledQuantity,
            decimal averagePrice,
            ZonedDateTime timestamp);

        /// <summary>
        /// Creates an <see cref="OrderPartiallyFilled"/> event and sends it to the registered
        /// event receivers.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="orderIdBroker">The order broker order identifier.</param>
        /// <param name="executionId">The order execution identifier.</param>
        /// <param name="executionTicket">The order execution ticket.</param>
        /// <param name="symbolCode">The order symbol code.</param>
        /// <param name="venue">The order venue.</param>
        /// <param name="side">The order side.</param>
        /// <param name="filledQuantity">The order filled quantity.</param>
        /// <param name="leavesQuantity">The order leaves quantity.</param>
        /// <param name="averagePrice">The order average price.</param>
        /// <param name="timestamp">The event timestamp.</param>
        void OnOrderPartiallyFilled(
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
            ZonedDateTime timestamp);
    }
}
