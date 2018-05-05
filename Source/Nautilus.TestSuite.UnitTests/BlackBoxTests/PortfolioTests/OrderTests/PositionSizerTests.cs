//--------------------------------------------------------------------------------------------------
// <copyright file="PositionSizerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.PortfolioTests.OrderTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using NautechSystems.CSharp;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Portfolio.Orders;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class PositionSizerTests
    {
        private readonly ITestOutputHelper output;
        private readonly ComponentryContainer container;
        private readonly MockLoggingAdatper mockLoggingAdatper;

        public PositionSizerTests(ITestOutputHelper output)
        {
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            this.container = setupFactory.Create();
            this.mockLoggingAdatper = setupFactory.LoggingAdatper;
        }

        [Fact]
        internal void Calculate_WithPositionSizeBeyondLimit_ReturnsZeroQuantityAndExpectedWarning()
        {
            // Arrange
            var instrument = StubInstrumentFactory.AUDUSD();
            var tradeProfile = StubTradeProfileFactory.Create(20);

            var positionSizer = new PositionSizer(this.container, instrument);

            var signal = new EntrySignal(
                instrument.Symbol,
                new EntityId("NONE"),
                new Label("TestSignal"),
                tradeProfile,
                OrderSide.Buy,
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.70000m, 0.00001m),
                new SortedDictionary<int, Price>(),
                StubDateTime.Now());

            // Act
            var result = positionSizer.Calculate(
                signal,
                Money.Create(100000, CurrencyCode.USD),
                Percentage.Create(0.1m),
                Option<Quantity>.None(),
                1);

            // Assert
            Assert.Equal(Quantity.Zero(), result);
        }

        [Fact]
        internal void Calculate_ForexUsdQuoteCurrency_BuyCase()
        {
            // Arrange
            var instrument = StubInstrumentFactory.AUDUSD();
            var tradeProfile = StubTradeProfileFactory.Create(20);

            var positionSizer = new PositionSizer(this.container, instrument);

            var signal = new EntrySignal(
                instrument.Symbol,
                new EntityId("NONE"),
                new Label("TestSignal"),
                tradeProfile,
                OrderSide.Buy,
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.79900m, 0.00001m),
                new SortedDictionary<int, Price>(),
                StubDateTime.Now());

            // Act
            var result = positionSizer.Calculate(
                signal,
                Money.Create(100000, CurrencyCode.USD),
                Percentage.Create(1),
                Option<Quantity>.None(),
                1);

            // Assert
            Assert.Equal(Quantity.Create(500000), result);
        }

        [Fact]
        internal void Calculate_ForexUsdQuoteCurrency_SellCase()
        {
            // Arrange
            var instrument = StubInstrumentFactory.AUDUSD();
            var tradeProfile = StubTradeProfileFactory.Create(20);

            var positionSizer = new PositionSizer(this.container, instrument);

            var signal = new EntrySignal(
                instrument.Symbol,
                new EntityId("NONE"),
                new Label("TestSignal"),
                tradeProfile,
                OrderSide.Sell,
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80100m, 0.00001m),
                new SortedDictionary<int, Price>(),
                StubDateTime.Now());

            // Act
            var result = positionSizer.Calculate(
                signal,
                Money.Create(100000, CurrencyCode.USD),
                Percentage.Create(1),
                Option<Quantity>.None(),
                1);

            // Assert
            Assert.Equal(Quantity.Create(500000), result);
        }

        [Fact]
        internal void Calculate_CanImposeHardLimit_Forex()
        {
            // Arrange
            var instrument = StubInstrumentFactory.AUDUSD();
            var tradeProfile = StubTradeProfileFactory.Create(20);

            var positionSizer = new PositionSizer(this.container, instrument);

            var signal = new EntrySignal(
                instrument.Symbol,
                new EntityId("NONE"),
                new Label("TestSignal"),
                tradeProfile,
                OrderSide.Sell,
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80001m, 0.00001m),
                new SortedDictionary<int, Price>(),
                StubDateTime.Now());

            // Act
            var result = positionSizer.Calculate(
                signal,
                Money.Create(100000, CurrencyCode.USD),
                Percentage.Create(2),
                Option<Quantity>.Some(Quantity.Create(1000000)),
                1);

            var totalSize = result.MultiplyBy(tradeProfile.Units.Value);

            // Assert
            Assert.Equal(Quantity.Create(1000000), totalSize);
        }

        [Fact]
        internal void Calculate_ForexJpyQuoteCurrency_BuyCase()
        {
            // Arrange
            var conversionRate = 100;

            var instrument = StubInstrumentFactory.USDJPY();
            var tradeProfile = StubTradeProfileFactory.Create(20);

            var positionSizer = new PositionSizer(this.container, instrument);

            var signal = new EntrySignal(
                instrument.Symbol,
                new EntityId("NONE"),
                new Label("TestSignal"),
                tradeProfile,
                OrderSide.Buy,
                Price.Create(113.800m, 0.001m),
                Price.Create(113.600m, 0.001m),
                new SortedDictionary<int, Price>(),
                StubDateTime.Now());

            // Act
            var result = positionSizer.Calculate(
                signal,
                Money.Create(100000, CurrencyCode.USD),
                Percentage.Create(1),
                Option<Quantity>.None(),
                instrument.TickValue / conversionRate);

            // Assert
            Assert.Equal(Quantity.Create(2500000), result);
        }

        [Fact]
        internal void Calculate_ForexJpyQuoteCurrency_SellCase()
        {
            // Arrange
            var conversionRate = 100;

            var instrument = StubInstrumentFactory.USDJPY();
            var tradeProfile = StubTradeProfileFactory.Create(20);

            var positionSizer = new PositionSizer(this.container, instrument);

            var signal = new EntrySignal(
                instrument.Symbol,
                new EntityId("NONE"),
                new Label("TestSignal"),
                tradeProfile,
                OrderSide.Sell,
                Price.Create(113.800m, 0.001m),
                Price.Create(114.000m, 0.001m),
                new SortedDictionary<int, Price>(),
                StubDateTime.Now());

            // Act
            var result = positionSizer.Calculate(
                signal,
                Money.Create(100000, CurrencyCode.USD),
                Percentage.Create(1),
                Option<Quantity>.None(),
                instrument.TickValue / conversionRate);

            // Assert
            Assert.Equal(Quantity.Create(2500000), result);
        }
    }
}
