//--------------------------------------------------------------
// <copyright file="AlphaStrategyTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.AlphaModelTests.StrategyTests
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Moq;
    using Nautilus.Algorithms.Entry;
    using Nautilus.BlackBox.AlphaModel.Strategy;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Logging;
    using Nautilus.DomainModel.Entities;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class AlphaStrategyTests
    {
        private readonly Instrument instrument;
        private readonly TradeProfile tradeProfile;
        private readonly SignalLogic signalLogic;
        private readonly IEntryStopAlgorithm entryStopAlgorithm;
        private readonly IStopLossAlgorithm stopLossAlgorithm;
        private readonly IProfitTargetAlgorithm profitTargetAlgorithm;
        private readonly IReadOnlyCollection<IEntryAlgorithm> entryAlgorithms;
        private readonly IReadOnlyCollection<ITrailingStopAlgorithm> trailingStopAlgorithms;
        private readonly IReadOnlyCollection<IExitAlgorithm> exitAlgorithms;

        public AlphaStrategyTests()
        {
            var tradePeriod = 20;
            this.instrument = StubInstrumentFactory.AUDUSD();
            this.tradeProfile = StubTradeProfileFactory.Create(tradePeriod);
            this.signalLogic = new SignalLogic(false, false);
            this.entryStopAlgorithm = new Mock<IEntryStopAlgorithm>().Object;
            this.stopLossAlgorithm = new Mock<IStopLossAlgorithm>().Object;
            this.profitTargetAlgorithm = new Mock<IProfitTargetAlgorithm>().Object;
            this.entryAlgorithms = new List<IEntryAlgorithm> { new CloseDirectionEntry(this.tradeProfile, this.instrument, new Mock<IEntryStopAlgorithm>().Object) };
            this.trailingStopAlgorithms = new Mock<IReadOnlyCollection<ITrailingStopAlgorithm>>().Object;
            this.exitAlgorithms = new Mock<IReadOnlyCollection<IExitAlgorithm>>().Object;
        }

        [Fact]
        internal void ToString_ReturnsExpectedString()
        {
            // Arrange
            var alphaStrategy = new AlphaStrategy(
                this.instrument,
                this.tradeProfile,
                this.signalLogic,
                this.entryStopAlgorithm,
                this.stopLossAlgorithm,
                this.profitTargetAlgorithm,
                this.entryAlgorithms.ToImmutableList(),
                this.trailingStopAlgorithms.ToImmutableList(),
                this.exitAlgorithms.ToImmutableList());

            // Act
            var toString = LogFormatter.ToOutput(alphaStrategy);

            // Assert
            Assert.Equal("Symbol=AUDUSD.FXCM, TradeProfile=TradeProfile(TestTrade), BarSpecification=Minute(5), TradePeriod=20, EntryAlgorithms=1, TrailingStoplossAlgorithms=0, ExitAlgorithms=0", toString);
        }
    }
}