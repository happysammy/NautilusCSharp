//--------------------------------------------------------------------------------------------------
// <copyright file="LabelBuilderTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.FactoriesTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class LabelBuilderTests
    {
        [Fact]
        internal void Create_WithComponentSymbolExchange_ReturnsExpectedLabel()
        {
            // Arrange

            // Act
            var result = LabelFactory.Create(
                "Portfolio",
                new Symbol("AUDUSD", Venue.LMAX));

            // Assert
            Assert.Equal("Portfolio-AUDUSD.LMAX", result.ToString());
        }

        [Fact]
        internal void Create_WithComponentSymbolExchangeTradeType_ReturnsExpectedLabel()
        {
            // Arrange

            // Act
            var result = LabelFactory.Create(
                "Portfolio",
                new Symbol("AUDUSD", Venue.LMAX));

            // Assert
            Assert.Equal("Portfolio-AUDUSD.LMAX", result.ToString());
        }

        [Fact]
        internal void Create_WithModuleComponentSymbolExchange_ReturnsExpectedLabel()
        {
            // Arrange

            // Act
            var result = LabelFactory.Create(
                "TradeBook",
                new Symbol("AUDUSD", Venue.LMAX));

            // Assert
            Assert.Equal("TradeBook-AUDUSD.LMAX", result.ToString());
        }

        [Fact]
        internal void Create_WithModuleComponentAndSymbol_ReturnsExpectedLabel()
        {
            // Arrange

            // Act
            var result = LabelFactory.Create(
                "TickBarAggregator",
                StubBarType.AUDUSD());

            // Assert
            Assert.Equal("TickBarAggregator-AUDUSD.DUKASCOPY-1-Minute[Ask]", result.ToString());
        }
    }
}
