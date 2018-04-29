//--------------------------------------------------------------------------------------------------
// <copyright file="EntrySignalGeneratorTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.AlphaModelTests.SignalTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Nautilus.Algorithms.Entry;
    using Nautilus.Algorithms.EntryStop;
    using Nautilus.Algorithms.ProfitTarget;
    using Nautilus.Algorithms.StopLoss;
    using Nautilus.BlackBox.AlphaModel.Signal;
    using Nautilus.BlackBox.AlphaModel.Strategy;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class EntrySignalGeneratorTests
    {
        private readonly BarStore barStore;
        private readonly EntrySignalGenerator entrySignalGenerator;

        public EntrySignalGeneratorTests()
        {
            var tradePeriod = 20;
            var tradeProfile = StubTradeProfileFactory.Create(tradePeriod);

            var instrument = StubInstrumentFactory.AUDUSD();
            this.barStore = StubBarStoreFactory.Create();

            var marketDataProvider = new MarketDataProvider(instrument.Symbol);
            marketDataProvider.Update(StubTickFactory.Create(instrument.Symbol), 0.00005m);

            var entryStopAlgorithm = new BarStretchStop(tradeProfile, instrument);
            entryStopAlgorithm.Initialize(this.barStore, marketDataProvider);

            var alwaysEnterTest = new CloseDirectionEntry(tradeProfile, instrument, entryStopAlgorithm);
            alwaysEnterTest.Initialize(this.barStore, marketDataProvider);

            var barsBackWithBufferStop = new BarsBackWithBufferStop(tradeProfile, instrument, 3, 2.0m);
            barsBackWithBufferStop.Initialize(this.barStore, marketDataProvider);

            var riskMultiplesTargets = new RiskMultiplesTarget(tradeProfile, instrument, 1, 1.0m);
            riskMultiplesTargets.Initialize(this.barStore, marketDataProvider);

            this.entrySignalGenerator = new EntrySignalGenerator(
                instrument,
                StubTradeProfileFactory.Create(tradePeriod),
                new List<IEntryAlgorithm> { alwaysEnterTest },
                barsBackWithBufferStop,
                riskMultiplesTargets);

            AlgorithmHydrator.Hydrate(this.entrySignalGenerator, this.barStore);
        }

        [Fact]
        internal void ProcessBuy_ValidParameters_ReturnsExpectedEntrySignal()
        {
            // Arrange
            // Act
            var result = this.entrySignalGenerator.ProcessBuy();

            // Assert
            Assert.Equal("19700101000001000|AUDUSD.FXCM|TestTrade-Buy-CloseDirectionEntry-1", result.ToList()[0].SignalId.Value);
            Assert.Equal(1, result.Count);
            Assert.Equal(0.80021m, result.ToList()[0].EntryPrice.Value);
            Assert.Equal(0.79985m, result.ToList()[0].StopLossPrice.Value);
            Assert.Single(result.ToList()[0].ProfitTargets);
            Assert.Equal(OrderSide.Buy, result.ToList()[0].OrderSide);
            Assert.Equal(1, result.ToList()[0].TradeProfile.BarsValid);
            Assert.Equal(StubDateTime.Now() + Period.FromMinutes(5).ToDuration(), result.ToList()[0].ExpireTime);
        }

        [Fact]
        internal void ProcessSell_ValidParameters_ReturnsExpectedEntrySignal()
        {
            // Arrange
            var lastBar = new Bar(
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80010m, 0.00001m),
                Price.Create(0.79980m, 0.00001m),
                Price.Create(0.79990m, 0.00001m),
                Quantity.Create(1000),
                StubDateTime.Now() + Period.FromMinutes(5).ToDuration());

            this.barStore.Update(lastBar);

            // Act
            var result = this.entrySignalGenerator.ProcessSell();

            // Assert
            Assert.Equal("19700101000501000|AUDUSD.FXCM|TestTrade-Sell-CloseDirectionEntry-1", result.ToList()[0].SignalId.Value);
            Assert.Equal(1, result.Count);
            Assert.Equal(0.79979m, result.ToList()[0].EntryPrice.Value);
            Assert.Equal(0.80023m, result.ToList()[0].StopLossPrice.Value);
            Assert.Single(result.ToList()[0].ProfitTargets);
            Assert.Equal(OrderSide.Sell, result.ToList()[0].OrderSide);
            Assert.Equal(1, result.ToList()[0].TradeProfile.BarsValid);
            Assert.Equal(StubDateTime.Now() + Period.FromMinutes(10).ToDuration(), result.ToList()[0].ExpireTime);
        }
    }
}