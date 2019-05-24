//--------------------------------------------------------------------------------------------------
// <copyright file="StubOrderBuilder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class StubOrderBuilder
    {
        private Symbol Symbol { get; set; } = new Symbol("AUDUSD", Venue.FXCM);

        private OrderId OrderId { get; set; } = new OrderId("O-123456");

        private Label OrderLabel { get; set; } = new Label("TEST_ORDER");

        private OrderSide OrderSide { get; set; } = OrderSide.BUY;

        private Quantity Quantity { get; set; } = Quantity.Create(1);

        private Price Price { get; set; } = Price.Create(1, 1);

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
            this.Symbol = new Symbol("AUDUSD", Venue.FXCM);
            this.OrderId = new OrderId(orderId);
            this.OrderSide = OrderSide.BUY;
            this.Quantity = Quantity.Create(100000);
            this.Price = Price.Create(0.80000m, 5);
            this.TimeInForce = TimeInForce.GTD;
            this.ExpireTime = StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(5);

            return this;
        }

        public StubOrderBuilder StopLossOrder(string orderId)
        {
            this.Symbol = new Symbol("AUDUSD", Venue.FXCM);
            this.OrderId = new OrderId(orderId);
            this.OrderSide = OrderSide.SELL;
            this.Quantity = Quantity.Create(100000);
            this.Price = Price.Create(0.79900m, 5);
            this.TimeInForce = TimeInForce.GTC;

            return this;
        }

        public StubOrderBuilder TakeProfitOrder(string orderId)
        {
            this.Symbol = new Symbol("AUDUSD", Venue.FXCM);
            this.OrderId = new OrderId(orderId);
            this.OrderSide = OrderSide.SELL;
            this.Quantity = Quantity.Create(100000);
            this.Price = Price.Create(0.80100m, 5);
            this.TimeInForce = TimeInForce.GTC;

            return this;
        }

        public Order BuildMarketOrder()
        {
            return OrderFactory.Market(
                this.Symbol,
                this.OrderId,
                this.OrderLabel,
                this.OrderSide,
                this.Quantity,
                this.Timestamp);
        }

        public Order BuildLimitOrder()
        {
            return OrderFactory.Limit(
                this.Symbol,
                this.OrderId,
                this.OrderLabel,
                this.OrderSide,
                this.Quantity,
                this.Price,
                this.TimeInForce,
                this.ExpireTime,
                this.Timestamp);
        }

        public Order BuildStopMarketOrder()
        {
            return OrderFactory.StopMarket(
                this.Symbol,
                this.OrderId,
                this.OrderLabel,
                this.OrderSide,
                this.Quantity,
                this.Price,
                this.TimeInForce,
                this.ExpireTime,
                this.Timestamp);
        }

        public Order BuildStopLimitOrder()
        {
            return OrderFactory.StopLimit(
                this.Symbol,
                this.OrderId,
                this.OrderLabel,
                this.OrderSide,
                this.Quantity,
                this.Price,
                this.TimeInForce,
                this.ExpireTime,
                this.Timestamp);
        }
    }
}
