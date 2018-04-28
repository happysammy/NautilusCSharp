//--------------------------------------------------------------
// <copyright file="OrderFactoryTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.PortfolioTests.OrderTests
{
    using System.Diagnostics.CodeAnalysis;
    using NautechSystems.CSharp;
    using Nautilus.BlackBox.Portfolio.Orders;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class OrderFactoryTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLogger mockLogger;
        private readonly OrderPacketBuilder orderPacketBuilder;
        private readonly EntrySignal entrySignal;

        public OrderFactoryTests(ITestOutputHelper output)
        {
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLogger = setupFactory.Logger;

            this.orderPacketBuilder = new OrderPacketBuilder(setupContainer, StubInstrumentFactory.EURUSD());
            this.entrySignal = StubSignalBuilder.BuyEntrySignal();
        }

        [Fact]
        internal void Create_WithUnitSizeZero_ReturnsNullOrderPacket()
        {
            // Arrange
            // Act (ridiculous conversion rate).
            var orderPacket = this.orderPacketBuilder.Create(
                this.entrySignal,
                Money.Create(100000, CurrencyCode.USD),
                Percentage.Create(1),
                Option<Quantity>.None(),
                100000);

            // Assert
            Assert.True(orderPacket.HasNoValue);
        }

        [Fact]
        internal void Create_StubEntrySignalBuy_ReturnsCorrectlyConstructedOrderPacket()
        {
            // Arrange
            // Act
            var orderPacket = this.orderPacketBuilder.Create(
                this.entrySignal,
                Money.Create(100000, CurrencyCode.USD),
                Percentage.Create(1),
                Option<Quantity>.None(),
                1);

            // Assert
            Assert.Equal(2, orderPacket.Value.Orders.Count);
        }

        [Fact]
        internal void Create_StubEntrySignalBuy_ReturnsCorrectExpireTime()
        {
            // Arrange
            // Act
            var orderPacket = this.orderPacketBuilder.Create(
                this.entrySignal,
                Money.Create(100000, CurrencyCode.USD),
                Percentage.Create(1),
                Option<Quantity>.None(),
                1);

            // Assert
            Assert.Equal(this.entrySignal.ExpireTime, orderPacket.Value.Orders[0].EntryOrder.ExpireTime);
            this.output.WriteLine(orderPacket.Value.Orders[0].EntryOrder.ExpireTime.ToString());
        }

        [Fact]
        internal void Create_StubEntrySignalBuy_ReturnsCorrectEntryOrder()
        {
            // Arrange
            // Act
            var orderPacket = this.orderPacketBuilder.Create(
                this.entrySignal,
                Money.Create(100000, CurrencyCode.USD),
                Percentage.Create(1),
                Option<Quantity>.None(),
                1);

            // Assert
            Assert.Equal("TestTrade_U1", orderPacket.Value.Orders[0].EntryOrder.OrderLabel.ToString());
            Assert.Equal(500000, orderPacket.Value.Orders[0].EntryOrder.Quantity.Value);
            Assert.Equal(0.81000m, orderPacket.Value.Orders[0].EntryOrder.Price.Value);
        }

        [Fact]
        internal void Create_StubEntrySignalBuy_ReturnsCorrectStoplossOrder()
        {
            // Arrange
            // Act
            var orderPacket = this.orderPacketBuilder.Create(
                this.entrySignal,
                Money.Create(100000, CurrencyCode.USD),
                Percentage.Create(1),
                Option<Quantity>.None(),
                1);

            // Assert
            Assert.Equal("TestTrade_U1_SL", orderPacket.Value.Orders[0].StopLossOrder.OrderLabel.ToString());
            Assert.Equal(500000, orderPacket.Value.Orders[0].StopLossOrder.Quantity.Value);
            Assert.Equal(0.80900m, orderPacket.Value.Orders[0].StopLossOrder.Price.Value);
        }

        [Fact]
        internal void Create_StubEntrySignalBuy_ReturnsCorrectProfitTargetOrderUnit1()
        {
            // Arrange
            // Act
            var orderPacket = this.orderPacketBuilder.Create(
                this.entrySignal,
                Money.Create(100000, CurrencyCode.USD),
                Percentage.Create(1),
                Option<Quantity>.None(),
                1);

            // Assert
            Assert.Equal("TestTrade_U1_PT", orderPacket.Value.Orders[0].ProfitTargetOrder.Value.OrderLabel.ToString());
            Assert.Equal(500000, orderPacket.Value.Orders[0].ProfitTargetOrder.Value.Quantity.Value);
            Assert.Equal(0.81100m, orderPacket.Value.Orders[0].ProfitTargetOrder.Value.Price.Value);
        }

        [Fact]
        internal void Create_StubEntrySignalBuy_ReturnsNullProfitTargetOrderUnit2()
        {
            // Arrange
            // Act
            var orderPacket = this.orderPacketBuilder.Create(
                this.entrySignal,
                Money.Create(100000, CurrencyCode.USD),
                Percentage.Create(1),
                Option<Quantity>.None(),
                1);

            // Assert
            Assert.True(orderPacket.Value.Orders[1].ProfitTargetOrder.HasNoValue);
        }
    }
}
