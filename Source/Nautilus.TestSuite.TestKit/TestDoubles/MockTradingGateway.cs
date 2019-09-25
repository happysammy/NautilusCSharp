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
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a mock trading gateway for testing.
    /// </summary>
    public class MockTradingGateway : Component, ITradingGateway
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockTradingGateway"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        public MockTradingGateway(IComponentryContainer container)
            : base(container)
        {
            this.CalledMethods = new List<string>();
            this.ReceivedObjects = new List<object>();

            // Commands
            this.RegisterHandler<Connect>(this.OnMessage);
            this.RegisterHandler<Disconnect>(this.OnMessage);

            // Events
            this.RegisterHandler<OrderSubmitted>(this.OnMessage);
            this.RegisterHandler<OrderAccepted>(this.OnMessage);
            this.RegisterHandler<OrderRejected>(this.OnMessage);
            this.RegisterHandler<OrderWorking>(this.OnMessage);
            this.RegisterHandler<OrderModified>(this.OnMessage);
            this.RegisterHandler<OrderCancelReject>(this.OnMessage);
            this.RegisterHandler<OrderExpired>(this.OnMessage);
            this.RegisterHandler<OrderCancelled>(this.OnMessage);
            this.RegisterHandler<OrderPartiallyFilled>(this.OnMessage);
            this.RegisterHandler<OrderFilled>(this.OnMessage);
            this.RegisterHandler<AccountStateEvent>(this.OnMessage);
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
        public void AccountInquiry()
        {
            this.CalledMethods.Add(nameof(this.AccountInquiry));
        }

        /// <inheritdoc/>
        public void SubmitOrder(Order order, PositionIdBroker? positionIdBroker)
        {
            this.CalledMethods.Add(nameof(this.SubmitOrder));

            this.ReceivedObjects.Add(order);

            if (!(positionIdBroker is null))
            {
                this.ReceivedObjects.Add(positionIdBroker);
            }
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

        private void OnMessage(Connect message)
        {
            this.ReceivedObjects.Add(message);
        }

        private void OnMessage(Disconnect message)
        {
            this.ReceivedObjects.Add(message);
        }

        private void OnMessage(OrderSubmitted @event)
        {
            this.ReceivedObjects.Add(@event);
        }

        private void OnMessage(OrderAccepted @event)
        {
            this.ReceivedObjects.Add(@event);
        }

        private void OnMessage(OrderRejected @event)
        {
            this.ReceivedObjects.Add(@event);
        }

        private void OnMessage(OrderWorking @event)
        {
            this.ReceivedObjects.Add(@event);
        }

        private void OnMessage(OrderModified @event)
        {
            this.ReceivedObjects.Add(@event);
        }

        private void OnMessage(OrderCancelReject @event)
        {
            this.ReceivedObjects.Add(@event);
        }

        private void OnMessage(OrderExpired @event)
        {
            this.ReceivedObjects.Add(@event);
        }

        private void OnMessage(OrderCancelled @event)
        {
            this.ReceivedObjects.Add(@event);
        }

        private void OnMessage(OrderPartiallyFilled @event)
        {
            this.ReceivedObjects.Add(@event);
        }

        private void OnMessage(OrderFilled @event)
        {
            this.ReceivedObjects.Add(@event);
        }

        private void OnMessage(AccountStateEvent @event)
        {
            this.ReceivedObjects.Add(@event);
        }
    }
}
