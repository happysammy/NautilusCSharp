// -------------------------------------------------------------------------------------------------
// <copyright file="LabelBuilderTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.FactoriesTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [Collection("UnitTests.Collections.Core")]
    public class LabelBuilderTests
    {
        [Fact]
        internal void ComponentLabel_WithComponentSymbolExchange_RetusnExpectedLabel()
        {
            // Arrange

            // Act
            var result = LabelFactory.Component(
                "Portfolio",
                new Symbol("AUDUSD", Exchange.LMAX));

            // Assert
            Assert.Equal("Portfolio-AUDUSD.LMAX", result.ToString());
        }

        [Fact]
        internal void ComponentLabel_WithComponentSymbolExchangeTradeType_RetusnExpectedLabel()
        {
            // Arrange

            // Act
            var result = LabelFactory.ComponentByTradeType(
                "Portfolio",
                new Symbol("AUDUSD", Exchange.LMAX),
                new TradeType("Scalp"));

            // Assert
            Assert.Equal("Portfolio-AUDUSD.LMAX(Scalp)", result.ToString());
        }

        [Fact]
        internal void ComponentLabel_WithModuleComponentSymbolExchange_RetusnExpectedLabel()
        {
            // Arrange

            // Act
            var result = LabelFactory.Component(
                "TradeBook",
                new Symbol("AUDUSD", Exchange.LMAX));

            // Assert
            Assert.Equal("TradeBook-AUDUSD.LMAX", result.ToString());
        }

        [Fact]
        internal void ComponentLabel_WithModuleComponentSymbolExchangeTradeType_RetusnExpectedLabel()
        {
            // Arrange

            // Act
            var result = LabelFactory.ComponentByTradeType(
                "TickBarAggregator",
                new Symbol("AUDUSD", Exchange.LMAX),
                new TradeType("Scalp"));

            // Assert
            Assert.Equal("TickBarAggregator-AUDUSD.LMAX(Scalp)", result.ToString());
        }
    }
}
