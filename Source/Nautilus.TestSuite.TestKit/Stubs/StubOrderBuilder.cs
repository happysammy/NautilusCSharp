//--------------------------------------------------------------------------------------------------
// <copyright file="StubOrderBuilder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Stubs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class StubOrderBuilder
    {
        private Symbol Symbol { get; set; } = new Symbol("AUDUSD", new Venue("FXCM"));

        private OrderId OrderId { get; set; } = new OrderId("O-123456");

        private Label OrderLabel { get; set; } = new Label("TEST_ORDER");

        private OrderSide OrderSide { get; set; } = OrderSide.Buy;

        private OrderPurpose OrderPurpose { get; set; } = OrderPurpose.None;

        private Quantity Quantity { get; set; } = Quantity.Create(100000);

        private Price Price { get; set; } = Price.Create(1, 0);

        private TimeInForce TimeInForce { get; set; } = TimeInForce.DAY;

        private ZonedDateTime? ExpireTime { get; set; }

        private ZonedDateTime Timestamp { get; set; } = StubZonedDateTime.UnixEpoch();

        public StubOrderBuilder WithSymbol(Symbol symbol)
        {
            this.Symbol = symbol;

            return this;
        }

        public StubOrderBuilder WithOrderId(string orderId)
        {
            this.OrderId = new OrderId(orderId);

            return this;
        }

        public StubOrderBuilder WithLabel(string label)
        {
            this.OrderLabel = new Label(label);

            return this;
        }

        public StubOrderBuilder WithOrderSide(OrderSide orderSide)
        {
            this.OrderSide = orderSide;

            return this;
        }

        public StubOrderBuilder WithQuantity(Quantity quantity)
        {
            this.Quantity = quantity;

            return this;
        }

        public StubOrderBuilder WithPrice(Price price)
        {
            this.Price = price;

            return this;
        }

        public StubOrderBuilder WithTimeInForce(TimeInForce timeInForce)
        {
            this.TimeInForce = timeInForce;

            return this;
        }

        public StubOrderBuilder WithExpireTime(ZonedDateTime expireTime)
        {
            this.ExpireTime = expireTime;

            return this;
        }

        public StubOrderBuilder WithTimestamp(ZonedDateTime timestamp)
        {
            this.Timestamp = timestamp;

            return this;
        }

        public StubOrderBuilder EntryOrder(string orderId)
        {
            this.Symbol = new Symbol("AUDUSD", new Venue("FXCM"));
            this.OrderId = new OrderId(orderId);
            this.OrderSide = OrderSide.Buy;
            this.OrderPurpose = OrderPurpose.Entry;
            this.Quantity = Quantity.Create(100000);
            this.Price = Price.Create(0.80000m, 5);
            this.TimeInForce = TimeInForce.GTD;
            this.ExpireTime = StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(5);

            return this;
        }

        public StubOrderBuilder StopLossOrder(string orderId)
        {
            this.Symbol = new Symbol("AUDUSD", "FXCM");
            this.OrderId = new OrderId(orderId);
            this.OrderSide = OrderSide.Sell;
            this.OrderPurpose = OrderPurpose.StopLoss;
            this.Quantity = Quantity.Create(100000);
            this.Price = Price.Create(0.79900m, 5);
            this.TimeInForce = TimeInForce.GTC;

            return this;
        }

        public StubOrderBuilder TakeProfitOrder(string orderId)
        {
            this.Symbol = new Symbol("AUDUSD", new Venue("FXCM"));
            this.OrderId = new OrderId(orderId);
            this.OrderSide = OrderSide.Sell;
            this.OrderPurpose = OrderPurpose.TakeProfit;
            this.Quantity = Quantity.Create(100000);
            this.Price = Price.Create(0.80100m, 5);
            this.TimeInForce = TimeInForce.GTC;

            return this;
        }

        public Order BuildMarketOrder()
        {
            return OrderFactory.Market(
                this.OrderId,
                this.Symbol,
                this.OrderLabel,
                this.OrderSide,
                this.OrderPurpose,
                this.Quantity,
                this.Timestamp,
                Guid.NewGuid());
        }

        public Order BuildLimitOrder()
        {
            return OrderFactory.Limit(
                this.OrderId,
                this.Symbol,
                this.OrderLabel,
                this.OrderSide,
                this.OrderPurpose,
                this.Quantity,
                this.Price,
                this.TimeInForce,
                this.ExpireTime,
                this.Timestamp,
                Guid.NewGuid());
        }

        public Order BuildStopMarketOrder()
        {
            return OrderFactory.StopMarket(
                this.OrderId,
                this.Symbol,
                this.OrderLabel,
                this.OrderSide,
                this.OrderPurpose,
                this.Quantity,
                this.Price,
                this.TimeInForce,
                this.ExpireTime,
                this.Timestamp,
                Guid.NewGuid());
        }

        public Order BuildStopLimitOrder()
        {
            return OrderFactory.StopLimit(
                this.OrderId,
                this.Symbol,
                this.OrderLabel,
                this.OrderSide,
                this.OrderPurpose,
                this.Quantity,
                this.Price,
                this.TimeInForce,
                this.ExpireTime,
                this.Timestamp,
                Guid.NewGuid());
        }
    }
}
