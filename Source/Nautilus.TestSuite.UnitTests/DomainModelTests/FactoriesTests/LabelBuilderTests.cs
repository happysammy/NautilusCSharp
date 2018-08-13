//--------------------------------------------------------------------------------------------------
// <copyright file="LabelBuilderTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
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
        internal void ComponentLabel_WithComponentSymbolExchange_ReturnsExpectedLabel()
        {
            // Arrange

            // Act
            var result = LabelFactory.Component(
                "Portfolio",
                new Symbol("AUDUSD", Venue.LMAX));

            // Assert
            Assert.Equal("Portfolio-AUDUSD.LMAX", result.ToString());
        }

        [Fact]
        internal void ComponentLabel_WithComponentSymbolExchangeTradeType_ReturnsExpectedLabel()
        {
            // Arrange

            // Act
            var result = LabelFactory.Component(
                "Portfolio",
                new Symbol("AUDUSD", Venue.LMAX),
                new TradeType("TestScalps"));

            // Assert
            Assert.Equal("Portfolio-AUDUSD.LMAX-TestScalps", result.ToString());
        }

        [Fact]
        internal void ComponentLabel_WithModuleComponentSymbolExchange_ReturnsExpectedLabel()
        {
            // Arrange

            // Act
            var result = LabelFactory.Component(
                "TradeBook",
                new Symbol("AUDUSD", Venue.LMAX));

            // Assert
            Assert.Equal("TradeBook-AUDUSD.LMAX", result.ToString());
        }

        [Fact]
        internal void ComponentLabel_WithModuleComponentAndSymbol_ReturnsExpectedLabel()
        {
            // Arrange

            // Act
            var result = LabelFactory.Component(
                "TickBarAggregator",
                StubBarType.AUDUSD());

            // Assert
            Assert.Equal("TickBarAggregator-AUDUSD.Dukascopy-1-Minute[Ask]", result.ToString());
        }
    }
}
