//--------------------------------------------------------------------------------------------------
// <copyright file="TrailingStopSignalGeneratorTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.AlphaModelTests.SignalTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Moq;
    using Nautilus.Core;
    using Nautilus.BlackBox.AlphaModel.Signal;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class TrailingStopSignalGeneratorTests
    {
        private readonly ITestOutputHelper output;
        private readonly Instrument instrument;
        private readonly TradeProfile tradeProfile;

        public TrailingStopSignalGeneratorTests(ITestOutputHelper output)
        {
            this.output = output;

            this.instrument = StubInstrumentFactory.AUDUSD();
            this.tradeProfile = StubTradeProfileFactory.Create(20);
        }

        [Fact]
        internal void ProcessLong_IsSignalTrueValidParameters_ReturnsExpectedTrailingStopSignal()
        {
            // Arrange
            var mockTrailingStopAlgoSetup = new Mock<ITrailingStopAlgorithm>();

            mockTrailingStopAlgoSetup
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateLong())
               .Returns(new TrailingStopResponse(
                    new Label("TS0"),
                    MarketPosition.Long,
                    Price.Create(100m, 0.01m),
                    0,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo = mockTrailingStopAlgoSetup.Object;

            var trailingStopSignalGenerator = new TrailingStopSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<ITrailingStopAlgorithm> { mockTrailingStopAlgo });

            // Act
            var result = trailingStopSignalGenerator.ProcessLong();

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal("TS0", result.Value.SignalLabel.ToString());
            Assert.Equal(MarketPosition.Long, result.Value.ForMarketPosition);
            Assert.Equal(100m, result.Value.ForUnitStoplossPrices[0].Value);
        }

        [Fact]
        internal void ProcessShort_IsSignalTrueValidParameters_ReturnsExpectedTrailingStopSignal()
        {
            // Arrange
            var mockTrailingStopAlgoSetup = new Mock<ITrailingStopAlgorithm>();

            mockTrailingStopAlgoSetup
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateShort())
               .Returns(new TrailingStopResponse(
                    new Label("TS0"),
                    MarketPosition.Short,
                    Price.Create(110m, 0.01m),
                    0,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo = mockTrailingStopAlgoSetup.Object;

            var trailingStopSignalGenerator = new TrailingStopSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<ITrailingStopAlgorithm> { mockTrailingStopAlgo });

            // Act
            var result = trailingStopSignalGenerator.ProcessShort();

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal("TS0", result.Value.SignalLabel.ToString());
            Assert.Equal(MarketPosition.Short, result.Value.ForMarketPosition);
            Assert.Equal(110m, result.Value.ForUnitStoplossPrices[0].Value);
        }

        [Fact]
        internal void ProcessLong_IsSignalFalseValidParameters_ReturnsExpectedTrailingStopSignal()
        {
            // Arrange
            var mockTrailingStopAlgoSetup = new Mock<ITrailingStopAlgorithm>();

            mockTrailingStopAlgoSetup
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateLong())
               .Returns(Option<ITrailingStopResponse>.None());
            var mockTrailingStopAlgo = mockTrailingStopAlgoSetup.Object;

            var trailingStopSignalGenerator = new TrailingStopSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<ITrailingStopAlgorithm> { mockTrailingStopAlgo });

            // Act
            var result = trailingStopSignalGenerator.ProcessLong();

            // Assert
            Assert.False(result.HasValue);
        }

        [Fact]
        internal void ProcessShort_IsSignalFalseValidParameters_ReturnsExpectedTrailingStopSignal()
        {
            // Arrange
            var mockTrailingStopAlgoSetup = new Mock<ITrailingStopAlgorithm>();

            mockTrailingStopAlgoSetup
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateShort())
               .Returns(Option<ITrailingStopResponse>.None());
            var mockTrailingStopAlgo = mockTrailingStopAlgoSetup.Object;

            var trailingStopSignalGenerator = new TrailingStopSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<ITrailingStopAlgorithm> { mockTrailingStopAlgo });

            // Act
            var result = trailingStopSignalGenerator.ProcessShort();

            // Assert
            Assert.True(result.HasNoValue);
        }

        [Fact]
        internal void ProcessLong_MultipleTrailingStopAlgosWithAUnitZero_ReturnsExpectedTrailingStopSignalZeroOverride()
        {
            // Arrange
            var mockTrailingStopAlgoSetup1 = new Mock<ITrailingStopAlgorithm>();

            mockTrailingStopAlgoSetup1
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateLong())
               .Returns(new TrailingStopResponse(
                    new Label("TS1"),
                    MarketPosition.Long,
                    Price.Create(95m, 0.1m),
                    0,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo1 = mockTrailingStopAlgoSetup1.Object;

            var mockTrailingStopAlgoSetup2 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup2
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateLong())
               .Returns(new TrailingStopResponse(
                    new Label("TS2"),
                    MarketPosition.Long,
                    Price.Create(90m, 0.1m),
                    1,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo2 = mockTrailingStopAlgoSetup2.Object;

            var trailingStopSignalGenerator = new TrailingStopSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<ITrailingStopAlgorithm> { mockTrailingStopAlgo1, mockTrailingStopAlgo2 });

            // Act
            var result = trailingStopSignalGenerator.ProcessLong();
            var expectedForUnitStopPrices = new Dictionary<int, Price> { { 0, Price.Create(95m, 0.1m) } };

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal("TS1_TS2", result.Value.SignalLabel.ToString());
            Assert.Equal(MarketPosition.Long, result.Value.ForMarketPosition);
            Assert.Equal(expectedForUnitStopPrices, result.Value.ForUnitStoplossPrices);
        }

        [Fact]
        internal void ProcessShort_MultipleTrailingStopAlgosWithAUnitZero_ReturnsExpectedTrailingStopSignalZeroOverride()
        {
            // Arrange
            var mockTrailingStopAlgoSetup1 = new Mock<ITrailingStopAlgorithm>();

            mockTrailingStopAlgoSetup1
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateShort())
               .Returns(new TrailingStopResponse(
                    new Label("TS1"),
                    MarketPosition.Short,
                    Price.Create(0.80500m, 0.00001m),
                    0,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo1 = mockTrailingStopAlgoSetup1.Object;

            var mockTrailingStopAlgoSetup2 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup2
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateShort())
               .Returns(new TrailingStopResponse(
                    new Label("TS2"),
                    MarketPosition.Short,
                    Price.Create(0.80600m, 0.00001m),
                    1,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo2 = mockTrailingStopAlgoSetup2.Object;

            var trailingStopSignalGenerator = new TrailingStopSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<ITrailingStopAlgorithm> { mockTrailingStopAlgo1, mockTrailingStopAlgo2 });

            // Act
            var result = trailingStopSignalGenerator.ProcessShort();
            var expectedForUnitStopPrices = new Dictionary<int, Price> { { 0, Price.Create(0.80500m, 0.00001m) } };

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal("TS1_TS2", result.Value.SignalLabel.ToString());
            Assert.Equal(MarketPosition.Short, result.Value.ForMarketPosition);
            Assert.Equal(expectedForUnitStopPrices, result.Value.ForUnitStoplossPrices);
        }

        [Fact]
        internal void ProcessLong_MultipleTrailingStopAlgosWithAUnitZero_ReturnsExpectedTrailingStopSignalForSpecificUnits()
        {
            // Arrange
            var mockTrailingStopAlgoSetup1 = new Mock<ITrailingStopAlgorithm>();

            mockTrailingStopAlgoSetup1
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateLong())
               .Returns(new TrailingStopResponse(
                    new Label("TS1"),
                    MarketPosition.Long,
                    Price.Create(100m, 0.1m),
                    0,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo1 = mockTrailingStopAlgoSetup1.Object;

            var mockTrailingStopAlgoSetup2 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup2
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateLong())
               .Returns(new TrailingStopResponse(
                    new Label("TS2"),
                    MarketPosition.Long,
                    Price.Create(110m, 0.1m),
                    1,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo2 = mockTrailingStopAlgoSetup2.Object;

            var trailingStopSignalGenerator = new TrailingStopSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<ITrailingStopAlgorithm> { mockTrailingStopAlgo1, mockTrailingStopAlgo2 });

            // Act
            var result = trailingStopSignalGenerator.ProcessLong();
            var expectedForUnitStopPrices = new Dictionary<int, Price>
                                                {
                                                    { 0, Price.Create(100m, 0.1m) },
                                                    { 1, Price.Create(110m, 0.1m) }
                                                };

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal("TS1_TS2", result.Value.SignalLabel.ToString());
            Assert.Equal(MarketPosition.Long, result.Value.ForMarketPosition);
            Assert.Equal(expectedForUnitStopPrices, result.Value.ForUnitStoplossPrices);
        }

        [Fact]
        internal void ProcessShort_MultipleTrailingStopAlgosWithAUnitZero_ReturnsExpectedTrailingStopSignalForSpecificUnits()
        {
            // Arrange
            var mockTrailingStopAlgoSetup1 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup1.Setup(trailingStopAlgo => trailingStopAlgo.CalculateShort())
               .Returns(new TrailingStopResponse(
                    new Label("TS1"),
                    MarketPosition.Short,
                    Price.Create(0.80700m, 0.00001m),
                    0,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo1 = mockTrailingStopAlgoSetup1.Object;

            var mockTrailingStopAlgoSetup2 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup2.Setup(trailingStopAlgo => trailingStopAlgo.CalculateShort())
               .Returns(new TrailingStopResponse(
                    new Label("TS2"),
                    MarketPosition.Short,
                    Price.Create(0.80600m, 0.00001m),
                    1,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo2 = mockTrailingStopAlgoSetup2.Object;

            var trailingStopSignalGenerator = new TrailingStopSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<ITrailingStopAlgorithm> { mockTrailingStopAlgo1, mockTrailingStopAlgo2 });

            // Act
            var result = trailingStopSignalGenerator.ProcessShort();
            var expectedForUnitStopPrices = new Dictionary<int, Price>
                                                {
                                                    { 0, Price.Create(0.80700m, 0.00001m) },
                                                    { 1, Price.Create(0.80600m, 0.00001m) }
                                                };

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal("TS1_TS2", result.Value.SignalLabel.ToString());
            Assert.Equal(MarketPosition.Short, result.Value.ForMarketPosition);
            Assert.Equal(expectedForUnitStopPrices, result.Value.ForUnitStoplossPrices);
        }

        [Fact]
        internal void ProcessLong_MultipleTrailingStopAlgosForSpecificUnits_ReturnsExpectedTrailingStopSignalForSpecificUnits()
        {
            // Arrange
            var mockTrailingStopAlgoSetup1 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup1
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateLong())
               .Returns(new TrailingStopResponse(
                    new Label("TS1"),
                    MarketPosition.Long,
                    Price.Create(110m, 0.1m),
                    1,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo1 = mockTrailingStopAlgoSetup1.Object;

            var mockTrailingStopAlgoSetup2 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup2
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateLong())
               .Returns(new TrailingStopResponse(
                    new Label("TS2"),
                    MarketPosition.Long,
                    Price.Create(100m, 0.1m),
                    2,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo2 = mockTrailingStopAlgoSetup2.Object;

            var trailingStopSignalGenerator = new TrailingStopSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<ITrailingStopAlgorithm> { mockTrailingStopAlgo1, mockTrailingStopAlgo2 });

            // Act
            var result = trailingStopSignalGenerator.ProcessLong();
            var expectedForUnitStopPrices = new Dictionary<int, Price>
                                                {
                                                    { 1, Price.Create(110m, 0.1m) },
                                                    { 2, Price.Create(100m, 0.1m) }
                                                };

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal("TS1_TS2", result.Value.SignalLabel.ToString());
            Assert.Equal(MarketPosition.Long, result.Value.ForMarketPosition);
            Assert.Equal(expectedForUnitStopPrices, result.Value.ForUnitStoplossPrices);
        }

        [Fact]
        internal void ProcessShort_MultipleTrailingStopAlgosForSpecificUnits_ReturnsExpectedTrailingStopSignalForSpecificUnits()
        {
            // Arrange
            var mockTrailingStopAlgoSetup1 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup1
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateShort())
               .Returns(new TrailingStopResponse(
                    new Label("TS1"),
                    MarketPosition.Short,
                    Price.Create(0.80700m, 0.00001m),
                    1,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo1 = mockTrailingStopAlgoSetup1.Object;

            var mockTrailingStopAlgoSetup2 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup2
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateShort())
               .Returns(new TrailingStopResponse(
                    new Label("TS2"),
                    MarketPosition.Short,
                    Price.Create(0.80600m, 0.00001m),
                    2,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo2 = mockTrailingStopAlgoSetup2.Object;

            var trailingStopSignalGenerator = new TrailingStopSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<ITrailingStopAlgorithm> { mockTrailingStopAlgo1, mockTrailingStopAlgo2 });

            // Act
            var result = trailingStopSignalGenerator.ProcessShort();
            var expectedForUnitStopPrices = new Dictionary<int, Price>
                                                {
                                                    { 1, Price.Create(0.80700m, 0.00001m) },
                                                    { 2, Price.Create(0.80600m, 0.00001m) }
                                                };

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal(MarketPosition.Short, result.Value.ForMarketPosition);
            Assert.Equal(expectedForUnitStopPrices, result.Value.ForUnitStoplossPrices);
        }

        [Fact]
        internal void ProcessLong_MultipleTrailingStopAlgosForTheSameSpecificUnit_ReturnsTheGreaterStoplossForThatUnit()
        {
            // Arrange
            var mockTrailingStopAlgoSetup1 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup1
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateLong())
               .Returns(new TrailingStopResponse(
                    new Label("TS1"),
                    MarketPosition.Long,
                    Price.Create(110m, 0.1m),
                    1,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo1 = mockTrailingStopAlgoSetup1.Object;

            var mockTrailingStopAlgoSetup2 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup2
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateLong())
               .Returns(new TrailingStopResponse(
                    new Label("TS2"),
                    MarketPosition.Long,
                    Price.Create(100m, 0.1m),
                    1,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo2 = mockTrailingStopAlgoSetup2.Object;

            var trailingStopSignalGenerator = new TrailingStopSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<ITrailingStopAlgorithm> { mockTrailingStopAlgo1, mockTrailingStopAlgo2 });

            // Act
            var result = trailingStopSignalGenerator.ProcessLong();
            var expectedForUnitStopPrices = new Dictionary<int, Price> { { 1, Price.Create(110m, 0.1m) } };

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal("TS1_TS2", result.Value.SignalLabel.ToString());
            Assert.Equal(MarketPosition.Long, result.Value.ForMarketPosition);
            Assert.Equal(expectedForUnitStopPrices, result.Value.ForUnitStoplossPrices);
        }

        [Fact]
        internal void ProcessShort_MultipleTrailingStopAlgosForTheSameSpecificUnit_ReturnsTheLesserStoplossForThatUnit()
        {
            // Arrange
            var mockTrailingStopAlgoSetup1 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup1
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateShort())
               .Returns(new TrailingStopResponse(
                    new Label("TS1"),
                    MarketPosition.Short,
                    Price.Create(0.80700m, 0.00001m),
                    1,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo1 = mockTrailingStopAlgoSetup1.Object;

            var mockTrailingStopAlgoSetup2 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup2
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateShort())
               .Returns(new TrailingStopResponse(
                    new Label("TS2"),
                    MarketPosition.Short,
                    Price.Create(0.80600m, 0.00001m),
                    1,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo2 = mockTrailingStopAlgoSetup2.Object;

            var trailingStopSignalGenerator = new TrailingStopSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<ITrailingStopAlgorithm> { mockTrailingStopAlgo1, mockTrailingStopAlgo2 });

            // Act
            var result = trailingStopSignalGenerator.ProcessShort();
            var expectedForUnitStopPrices = new Dictionary<int, Price> { { 1, Price.Create(0.80600m, 0.00001m) } };

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal("TS1_TS2", result.Value.SignalLabel.ToString());
            Assert.Equal(MarketPosition.Short, result.Value.ForMarketPosition);
            Assert.Equal(expectedForUnitStopPrices, result.Value.ForUnitStoplossPrices);
        }

        [Fact]
        internal void ProcessLong_MultipleTrailingStopAlgosWithForUnitZeroAndOverrides_ReturnsTheExpectedForUnitStoploss()
        {
            // Arrange
            var mockTrailingStopAlgoSetup1 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup1
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateLong())
               .Returns(new TrailingStopResponse(
                    new Label("TS1"),
                    MarketPosition.Long,
                    Price.Create(100m, 0.1m),
                    0,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo1 = mockTrailingStopAlgoSetup1.Object;

            var mockTrailingStopAlgoSetup2 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup2
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateLong())
               .Returns(new TrailingStopResponse(
                    new Label("TS2"),
                    MarketPosition.Long,
                    Price.Create(120m, 0.1m),
                    1,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo2 = mockTrailingStopAlgoSetup2.Object;

            var mockTrailingStopAlgoSetup3 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup3
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateLong())
               .Returns(new TrailingStopResponse(
                    new Label("TS3"),
                    MarketPosition.Long,
                    Price.Create(130m, 0.1m),
                    1,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo3 = mockTrailingStopAlgoSetup3.Object;

            var trailingStopSignalGenerator = new TrailingStopSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<ITrailingStopAlgorithm> { mockTrailingStopAlgo1, mockTrailingStopAlgo2, mockTrailingStopAlgo3 });

            // Act
            var result = trailingStopSignalGenerator.ProcessLong();
            var expectedForUnitStopPrices = new Dictionary<int, Price>
                                                {
                                                    { 0, Price.Create(100m, 0.1m) },
                                                    { 1, Price.Create(130m, 0.1m) }
                                                };

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal("TS1_TS2_TS3", result.Value.SignalLabel.ToString());
            Assert.Equal(MarketPosition.Long, result.Value.ForMarketPosition);
            Assert.Equal(expectedForUnitStopPrices, result.Value.ForUnitStoplossPrices);
        }

        [Fact]
        internal void ProcessShort_MultipleTrailingStopAlgosWithForUnitZeroAndOverrides_ReturnsTheExpectedForUnitStoploss()
        {
            // Arrange
            var mockTrailingStopAlgoSetup1 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup1
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateShort())
               .Returns(new TrailingStopResponse(
                    new Label("TS1"),
                    MarketPosition.Short,
                    Price.Create(1.30000m, 0.00001m),
                    0,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo1 = mockTrailingStopAlgoSetup1.Object;

            var mockTrailingStopAlgoSetup2 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup2
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateShort())
               .Returns(new TrailingStopResponse(
                    new Label("TS2"),
                    MarketPosition.Short,
                    Price.Create(1.29000m, 0.00001m),
                    1,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo2 = mockTrailingStopAlgoSetup2.Object;

            var mockTrailingStopAlgoSetup3 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup3
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateShort())
               .Returns(new TrailingStopResponse(
                    new Label("TS3"),
                    MarketPosition.Short,
                    Price.Create(1.28000m, 0.00001m),
                    1,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo3 = mockTrailingStopAlgoSetup3.Object;

            var mockTrailingStopAlgoSetup4 = new Mock<ITrailingStopAlgorithm>();
            mockTrailingStopAlgoSetup4
               .Setup(trailingStopAlgo => trailingStopAlgo.CalculateShort())
               .Returns(new TrailingStopResponse(
                    new Label("TS4"),
                    MarketPosition.Short,
                    Price.Create(1.29500m, 0.00001m),
                    2,
                    StubDateTime.Now()));

            var mockTrailingStopAlgo4 = mockTrailingStopAlgoSetup4.Object;

            var trailingStopSignalGenerator = new TrailingStopSignalGenerator(
                this.instrument,
                this.tradeProfile,
                new List<ITrailingStopAlgorithm> { mockTrailingStopAlgo1, mockTrailingStopAlgo2, mockTrailingStopAlgo3, mockTrailingStopAlgo4 });

            // Act
            var result = trailingStopSignalGenerator.ProcessShort();
            var expectedForUnitStopPrices = new Dictionary<int, Price>
                                                {
                                                    { 0, Price.Create(1.30000m, 0.00001m) },
                                                    { 1, Price.Create(1.28000m, 0.00001m) },
                                                    { 2, Price.Create(1.29500m, 0.00001m) }
                                                };

            // Assert
            Assert.True(result.HasValue);
            Assert.Equal("TS1_TS2_TS3_TS4", result.Value.SignalLabel.ToString());
            Assert.Equal(MarketPosition.Short, result.Value.ForMarketPosition);
            Assert.Equal(expectedForUnitStopPrices, result.Value.ForUnitStoplossPrices);
        }
    }
}