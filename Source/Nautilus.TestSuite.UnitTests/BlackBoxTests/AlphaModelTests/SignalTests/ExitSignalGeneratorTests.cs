// -------------------------------------------------------------------------------------------------
// <copyright file="ExitSignalGeneratorTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.AlphaModelTests.SignalTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Algorithms.Exit;
    using Nautilus.BlackBox.AlphaModel.Signal;
    using Nautilus.BlackBox.AlphaModel.Strategy;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class ExitSignalGeneratorTests
    {
        private readonly Instrument instrument;
        private readonly TradeProfile tradeProfile;
        private readonly IBarStore barStore;
        private readonly MarketDataProvider marketDataProvider;

        public ExitSignalGeneratorTests()
        {
            this.instrument = StubInstrumentFactory.AUDUSD();
            this.tradeProfile = StubTradeProfileFactory.Create(20);
            this.barStore = StubBarStoreFactory.Create();
            this.marketDataProvider = new MarketDataProvider(this.instrument.Symbol);
            this.marketDataProvider.Update(StubTickFactory.Create(this.instrument.Symbol), 0.00005m);
        }

        [Fact]
        internal void ProcessLong_IsSignalTrueValidParameters_ReturnsExpectedExitSignal()
        {
            // Arrange
            var exhaustionBarExit = new AlwaysExit(this.tradeProfile, this.instrument, 0);
            exhaustionBarExit.Initialize(this.barStore, this.marketDataProvider);

            var exitSignalGenerator = new ExitSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<IExitAlgorithm> { exhaustionBarExit });

            AlgorithmHydrator.Hydrate(exitSignalGenerator, this.barStore);

            // Act
            var result = exitSignalGenerator.ProcessLong();

            // Assert
            Assert.True(result.HasValue);
        }

        [Fact]

        internal void ProcessShort_IsSignalTrueParameters_ReturnsExpectedExitSignal()
        {
            // Arrange
            var exhaustionBarExit = new AlwaysExit(this.tradeProfile, this.instrument, 0);
            exhaustionBarExit.Initialize(this.barStore, this.marketDataProvider);

            var exitSignalGenerator = new ExitSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<IExitAlgorithm> { exhaustionBarExit });

            AlgorithmHydrator.Hydrate(exitSignalGenerator, this.barStore);

            // Act
            var result = exitSignalGenerator.ProcessShort();

            // Assert
            Assert.True(result.HasValue);
        }

        [Fact]
        internal void ProcessBuy_MultipleExitAlgos_ReturnsExpectedExitSignalForAllUnits()
        {
            // Arrange
            var alwaysExit1 = new AlwaysExit(this.tradeProfile, this.instrument, 0);
            alwaysExit1.Initialize(this.barStore, this.marketDataProvider);

            var alwaysExit2 = new AlwaysExit(this.tradeProfile, this.instrument, 0);
            alwaysExit2.Initialize(this.barStore, this.marketDataProvider);

            var exitSignalGenerator = new ExitSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<IExitAlgorithm> { alwaysExit1, alwaysExit2 });

            AlgorithmHydrator.Hydrate(exitSignalGenerator, this.barStore);

            // Act
            var result = exitSignalGenerator.ProcessLong();

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal(MarketPosition.Long, result.Value.ForMarketPosition);
            Assert.Equal(new List<int> { 0 }, result.Value.ForUnit);
        }

        [Fact]
        internal void ProcessBuy_MultipleExitAlgos_ReturnsExpectedExitSignalForSpecificUnits()
        {
            // Arrange
            var alwaysExit1 = new AlwaysExit(this.tradeProfile, this.instrument, 1);
            alwaysExit1.Initialize(this.barStore, this.marketDataProvider);

            var alwaysExit2 = new AlwaysExit(this.tradeProfile, this.instrument, 2);
            alwaysExit2.Initialize(this.barStore, this.marketDataProvider);

            var exitSignalGenerator = new ExitSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<IExitAlgorithm> { alwaysExit1, alwaysExit2 });

            AlgorithmHydrator.Hydrate(exitSignalGenerator, this.barStore);

            // Act
            var result = exitSignalGenerator.ProcessLong();

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal(MarketPosition.Long, result.Value.ForMarketPosition);
            Assert.Equal(new List<int> { 1, 2 }, result.Value.ForUnit);
        }
    }
}