//--------------------------------------------------------------
// <copyright file="StubOrderBuilder.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using NautechSystems.CSharp;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Orders;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class StubOrderBuilder
    {
        private Symbol Symbol { get; set; } = new Symbol("AUDUSD", Exchange.FXCM);

        private EntityId OrderId { get; set; } = new EntityId("StubOrderId");

        private Label OrderLabel { get; set; } = new Label("StubOrderLabel");

        private OrderSide OrderSide { get; set; } = OrderSide.Buy;

        private Quantity Quantity { get; set; } = Quantity.Create(1);

        private Price Price { get; set; } = Price.Create(1, 1);

        private TimeInForce TimeInForce { get; set; } = TimeInForce.DAY;

        private Option<ZonedDateTime?> ExpireTime { get; set; } = Option<ZonedDateTime?>.None();

        private ZonedDateTime Timestamp { get; set; } = StubDateTime.Now();

        public StubOrderBuilder WithSymbol(Symbol symbol)
        {
            this.Symbol = symbol;

            return this;
        }

        public StubOrderBuilder WithOrderId(string orderId)
        {
            this.OrderId = new EntityId(orderId);

            return this;
        }

        public StubOrderBuilder WithOrderLabel(string label)
        {
            this.OrderLabel = new Label(label);

            return this;
        }

        public StubOrderBuilder WithOrderSide(OrderSide orderSide)
        {
            this.OrderSide = orderSide;

            return this;
        }

        public StubOrderBuilder WithOrderQuantity(Quantity quantity)
        {
            this.Quantity = quantity;

            return this;
        }

        public StubOrderBuilder WithOrderPrice(Price price)
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
            this.Symbol = new Symbol("AUDUSD", Exchange.FXCM);
            this.OrderId = new EntityId(orderId);
            this.OrderSide = OrderSide.Buy;
            this.Quantity = Quantity.Create(100000);
            this.Price = Price.Create(0.80000m, 0.00001m);
            this.TimeInForce = TimeInForce.GTD;
            this.ExpireTime = StubDateTime.Now() + Period.FromMinutes(5).ToDuration();

            return this;
        }

        public StubOrderBuilder StoplossOrder(string orderId)
        {
            this.Symbol = new Symbol("AUDUSD", Exchange.FXCM);
            this.OrderId = new EntityId(orderId);
            this.OrderSide = OrderSide.Sell;
            this.Quantity = Quantity.Create(100000);
            this.Price = Price.Create(0.79900m, 0.00001m);
            this.TimeInForce = TimeInForce.GTC;

            return this;
        }

        public StubOrderBuilder ProfitTargetOrder(string orderId)
        {
            this.Symbol = new Symbol("AUDUSD", Exchange.FXCM);
            this.OrderId = new EntityId(orderId);
            this.OrderSide = OrderSide.Sell;
            this.Quantity = Quantity.Create(100000);
            this.Price = Price.Create(0.80100m, 0.00001m);
            this.TimeInForce = TimeInForce.GTC;

            return this;
        }

        public MarketOrder BuildMarket()
        {
            return new MarketOrder(
                this.Symbol,
                this.OrderId,
                this.OrderLabel,
                this.OrderSide,
                this.Quantity,
                this.Timestamp);
        }

        public StopMarketOrder BuildStopMarket()
        {
            return new StopMarketOrder(
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

        public StopLimitOrder BuildStopLimit()
        {
            return new StopLimitOrder(
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
