// -------------------------------------------------------------------------------------------------
// <copyright file="MockTradingGateway.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Collections.Generic;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides a mock trading gateway for testing.
    /// </summary>
    public class MockTradingGateway : ITradingGateway
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockTradingGateway"/> class.
        /// </summary>
        public MockTradingGateway()
        {
            this.CalledMethods = new List<string>();
            this.ReceivedObjects = new List<object>();
        }

        /// <summary>
        /// Gets the objects received by the mock.
        /// </summary>
        public List<object> ReceivedObjects { get; }

        /// <summary>
        /// Gets the method names called on the mock.
        /// </summary>
        public List<string> CalledMethods { get; }

        /// <inheritdoc/>
        public Brokerage Brokerage => new Brokerage("NAUTILUS");

        /// <inheritdoc/>
        public AccountId AccountId => AccountId.FromString("NAUTILUS-000-SIMULATED");

        /// <inheritdoc/>
        public bool IsConnected => true;

        /// <inheritdoc/>
        public void AccountInquiry()
        {
            this.CalledMethods.Add(nameof(this.AccountInquiry));
        }

        /// <inheritdoc/>
        public void TradingSessionStatus()
        {
            this.CalledMethods.Add(nameof(this.TradingSessionStatus));
        }

        /// <inheritdoc/>
        public void SubmitOrder(Order order)
        {
            this.CalledMethods.Add(nameof(this.SubmitOrder));
            this.ReceivedObjects.Add(order);
        }

        /// <inheritdoc/>
        public void SubmitOrder(AtomicOrder atomicOrder)
        {
            this.CalledMethods.Add(nameof(this.SubmitOrder));
            this.ReceivedObjects.Add(atomicOrder);
        }

        /// <inheritdoc/>
        public void ModifyOrder(Order order, Price modifiedPrice)
        {
            this.CalledMethods.Add(nameof(this.ModifyOrder));
            this.ReceivedObjects.Add((order, modifiedPrice));
        }

        /// <inheritdoc/>
        public void CancelOrder(Order order)
        {
            this.CalledMethods.Add(nameof(this.CancelOrder));
            this.ReceivedObjects.Add(order);
        }

        /// <inheritdoc/>
        public void OnPositionReport(string account)
        {
            this.CalledMethods.Add(nameof(this.OnPositionReport));
        }

        /// <inheritdoc/>
        public void OnBusinessMessage(string message)
        {
            this.CalledMethods.Add(nameof(this.OnBusinessMessage));
        }

        /// <inheritdoc/>
        public void OnCollateralInquiryAck(string inquiryId, string accountNumber)
        {
            this.CalledMethods.Add(nameof(this.OnCollateralInquiryAck));
        }

        /// <inheritdoc/>
        public void OnRequestForPositionsAck(string accountNumber, string positionRequestId)
        {
            this.CalledMethods.Add(nameof(this.OnRequestForPositionsAck));
        }

        /// <inheritdoc/>
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
            this.CalledMethods.Add(nameof(this.OnAccountReport));
        }

        /// <inheritdoc/>
        public void OnOrderRejected(
            string orderId,
            string rejectReason,
            ZonedDateTime timestamp)
        {
            this.CalledMethods.Add(nameof(this.OnOrderRejected));
        }

        /// <inheritdoc/>
        public void OnOrderCancelReject(
            string orderId,
            string cancelRejectResponseTo,
            string cancelRejectReason,
            ZonedDateTime timestamp)
        {
            this.CalledMethods.Add(nameof(this.OnOrderCancelReject));
        }

        /// <inheritdoc/>
        public void OnOrderCancelled(
            string orderId,
            string orderIdBroker,
            string label,
            ZonedDateTime timestamp)
        {
            this.CalledMethods.Add(nameof(this.OnOrderCancelled));
        }

        /// <inheritdoc/>
        public void OnOrderModified(
            string orderId,
            string orderIdBroker,
            string label,
            decimal price,
            ZonedDateTime timestamp)
        {
            this.CalledMethods.Add(nameof(this.OnOrderModified));
        }

        /// <inheritdoc/>
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
            this.CalledMethods.Add(nameof(this.OnOrderWorking));
        }

        /// <inheritdoc/>
        public void OnOrderExpired(
            string orderId,
            string orderIdBroker,
            string label,
            ZonedDateTime timestamp)
        {
            this.CalledMethods.Add(nameof(this.OnOrderExpired));
        }

        /// <inheritdoc/>
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
            this.CalledMethods.Add(nameof(this.OnOrderFilled));
        }

        /// <inheritdoc/>
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
            this.CalledMethods.Add(nameof(this.OnOrderPartiallyFilled));
        }
    }
}
