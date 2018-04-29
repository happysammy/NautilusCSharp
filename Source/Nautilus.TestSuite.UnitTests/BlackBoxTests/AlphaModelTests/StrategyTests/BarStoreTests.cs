//--------------------------------------------------------------------------------------------------
// <copyright file="BarStoreTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.AlphaModelTests.StrategyTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.BlackBox.AlphaModel.Strategy;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BarStoreTests
    {
        private readonly BarStore barStore;

        public BarStoreTests()
        {
            this.barStore = StubBarStoreFactory.Create();
        }

        [Fact]
        internal void Symbol_ReturnsTheSymbolOfTheBarStore()
        {
            // Arrange
            // Act
            var result = this.barStore.Symbol;

            // Assert
            Assert.Equal(new Symbol("AUDUSD", Exchange.FXCM), result);
        }

        [Fact]
        internal void Volume_ReturnsTheVolumeOfTheLastBar()
        {
            // Arrange
            // Act
            var result = this.barStore.Volume;

            // Assert
            Assert.Equal(Quantity.Create(1000), result);
            Assert.Equal(Quantity.Create(1000), this.barStore.GetBar(0).Volume);
        }

        [Fact]
        internal void Time_ReturnsTheTimeOfTheLastBar()
        {
            // Arrange
            // Act
            var result = this.barStore.Timestamp;

            // Assert
            Assert.Equal(StubDateTime.Now(), result);
            Assert.Equal(StubDateTime.Now(), this.barStore.GetTimestamp(0));
        }

        [Fact]
        internal void Reset_WhenBarsInStore_ReturnsTheCountOfBarsToZero()
        {
            // Arrange
            var result1 = this.barStore.Count;

            // Act
            this.barStore.Reset();
            var result2 = this.barStore.Count;

            // Assert
            Assert.Equal(10, result1);
            Assert.Equal(0, result2);
        }

        [Fact]
        internal void GetBar_WhenBarsInStore_ReturnsTheCorrectBar()
        {
            // Arrange
            // Act
            var bar1 = this.barStore.GetBar(StubDateTime.Now());
            var bar2 = this.barStore.GetBar(StubDateTime.Now() - Period.FromMinutes(10).ToDuration());

            // Assert
            Assert.Equal(StubBarBuilder.BuildList()[9], bar1);
            Assert.Equal(StubBarBuilder.BuildList()[7], bar2);
        }

        [Fact]
        internal void GetLargestRange_WithStubBarFactory_ReturnsExpectedLargestRange()
        {
            // Arrange
            // Act
            var result = this.barStore.GetLargestRange(5, 0);

            // Assert
            Assert.Equal(0.00025m, result);
        }

        [Fact]
        internal void GetSmallestRange_WithStubBarFactory_ReturnsExpectedSmallestRange()
        {
            // Arrange
            // Act
            var result = this.barStore.GetSmallestRange(5, 0);

            // Assert
            Assert.Equal(0.00010m, result);
        }

        [Fact]
        internal void GetMaxHigh_FromStubBarFactory_ReturnsExpectedMaxHigh()
        {
            // Arrange
            // Act
            var result = this.barStore.GetMaxHigh(5, 0);

            // Assert
            Assert.Equal(0.80015m, result.Value);
        }

        [Fact]
        internal void GetMinLow_FromStubBarFactory_ReturnsExpectedMinLow()
        {
            // Arrange
            // Act
            var result = this.barStore.GetMinLow(5, 0);

            // Assert
            Assert.Equal(0.79980m, result.Value);
        }

        [Fact]
        internal void GetBarRange_OfLastBarInBarStore_ReturnsExpectedRange()
        {
            // Arrange
            // Act
            var result = this.barStore.GetBarRange(0);

            // Assert
            Assert.Equal(0.00025m, result);
        }
    }
}
