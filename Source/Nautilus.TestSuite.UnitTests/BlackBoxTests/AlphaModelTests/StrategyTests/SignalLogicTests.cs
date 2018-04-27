// -------------------------------------------------------------------------------------------------
// <copyright file="SignalLogicTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.AlphaModelTests.StrategyTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using NautechSystems.CSharp;
    using Nautilus.BlackBox.AlphaModel.Strategy;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class SignalLogicTests
    {
        [Fact]
        internal void IsValidBuySignal_WithNoEntrySignal_ReturnsFalse()
        {
            // Arrange
            var signalLogic = new SignalLogic(false, false);

            var entrySignalsBuy = new List<EntrySignal>();
            var entrySignalsSell = new List<EntrySignal>();
            var exitSignalLong = Option<ExitSignal>.None();

            // Act
            var result = signalLogic.IsValidBuySignal(
                entrySignalsBuy,
                entrySignalsSell,
                exitSignalLong);

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void IsValidBuySignal_WithBuyEntrySignalAndNoConditions_ReturnsTrue()
        {
            // Arrange
            var signalLogic = new SignalLogic(false, false);

            var entrySignalsBuy = new List<EntrySignal> { StubSignalBuilder.BuyEntrySignal() };
            var entrySignalsSell = new List<EntrySignal>();
            var exitSignalLong = Option<ExitSignal>.None();

            // Act
            var result = signalLogic.IsValidBuySignal(
                entrySignalsBuy,
                entrySignalsSell,
                exitSignalLong);

            // Assert
            Assert.True(result);
        }

        [Fact]
        internal void IsValidBuySignal_WithBuyEntryPlusExitSignalsConditionExitsBlockEntries_ReturnsFalse()
        {
            // Arrange
            // SameDirectionExitSignalBlocksEntires = true.
            var signalLogic = new SignalLogic(false, true);

            var entrySignalsBuy = new List<EntrySignal> { StubSignalBuilder.BuyEntrySignal() };
            var entrySignalsSell = new List<EntrySignal>();
            var exitSignalLong = StubSignalBuilder.LongExitSignal(new TradeType("TestTrade"), new List<int> { 0 }, Period.FromMinutes(1));

            // Act
            var result = signalLogic.IsValidBuySignal(
                entrySignalsBuy,
                entrySignalsSell,
                exitSignalLong);

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void IsValidBuySignal_WithBuyPlusSellEntrySignalsConditionOppositeDirectionSignalsBlockEntries_ReturnsFalse()
        {
            // Arrange
            // OppositeDirectionSignalBlocksEntires = true.
            var signalLogic = new SignalLogic(true, false);

            var entrySignalsBuy = new List<EntrySignal> { StubSignalBuilder.BuyEntrySignal() };
            var entrySignalsSell = new List<EntrySignal> { StubSignalBuilder.SellEntrySignal() };
            var exitSignalLong = Option<ExitSignal>.None();

            // Act
            var result = signalLogic.IsValidBuySignal(
                entrySignalsBuy,
                entrySignalsSell,
                exitSignalLong);

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void IsValidSellSignal_WithNoEntrySignals_ReturnsFalse()
        {
            // Arrange
            var signalLogic = new SignalLogic(false, false);

            var entrySignalsBuy = new List<EntrySignal>();
            var entrySignalsSell = new List<EntrySignal>();
            var exitSignalShort = Option<ExitSignal>.None();

            // Act
            var result = signalLogic.IsValidSellSignal(
                entrySignalsBuy,
                entrySignalsSell,
                exitSignalShort);

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void IsValidSellSignal_WithSellEntrySignalAndNoConditions_ReturnsTrue()
        {
            // Arrange
            var signalLogic = new SignalLogic(false, false);

            var entrySignalsBuy = new List<EntrySignal>();
            var entrySignalsSell = new List<EntrySignal> { StubSignalBuilder.SellEntrySignal() };
            var exitSignalShort = Option<ExitSignal>.None();

            // Act
            var result = signalLogic.IsValidSellSignal(
                entrySignalsBuy,
                entrySignalsSell,
                exitSignalShort);

            // Assert
            Assert.True(result);
        }

        [Fact]
        internal void IsValidSellSignal_WithSellEntryAndExitSignalsAndExitsBlockEntries_ReturnsFalse()
        {
            // Arrange
            // SameDirectionExitSignalBlocksEntires = true.
            var signalLogic = new SignalLogic(false, true);

            var entrySignalsBuy = new List<EntrySignal>();
            var entrySignalsSell = new List<EntrySignal> { StubSignalBuilder.SellEntrySignal() };
            var exitSignalShort = StubSignalBuilder.ShortExitSignal(new TradeType("TestTrade"), new List<int> { 0 }, Period.FromMinutes(1));

            // Act
            var result = signalLogic.IsValidSellSignal(
                entrySignalsBuy,
                entrySignalsSell,
                exitSignalShort);

            // Assert
            Assert.False(result);
        }

        [Fact]
        internal void IsValidSellSignal_WithBuyAndSellEntrySignalsAndOppositeDirectionSignalsBlockEntries_ReturnsFalse()
        {
            // Arrange
            // OppositeDirectionSignalBlocksEntires = true.
            var signalLogic = new SignalLogic(true, false);

            var entrySignalsBuy = new List<EntrySignal> { StubSignalBuilder.BuyEntrySignal() };
            var entrySignalsSell = new List<EntrySignal> { StubSignalBuilder.SellEntrySignal() };
            var exitSignalShort = Option<ExitSignal>.None();

            // Act
            var result = signalLogic.IsValidSellSignal(
                entrySignalsBuy,
                entrySignalsSell,
                exitSignalShort);

            // Assert
            Assert.False(result);
        }
    }
}