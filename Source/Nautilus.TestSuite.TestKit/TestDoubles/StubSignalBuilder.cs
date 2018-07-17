// -------------------------------------------------------------------------------------------------
// <copyright file="StubSignalBuilder.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Creates an example stub order with default but correct values.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubSignalBuilder
    {
        public static EntrySignal BuyEntrySignal()
        {
            var tradeProfile = StubTradeProfileFactory.Create(20);

            return new EntrySignal(
                new Symbol("AUDUSD", Venue.GLOBEX),
                new EntityId("TestSignal"),
                new Label("TestTrade"),
                tradeProfile,
                OrderSide.Buy,
                Price.Create(0.81000m, 0.00001m),
                Price.Create(0.80900m, 0.00001m),
                new SortedDictionary<int, Price> { { 1, Price.Create(0.81100m, 0.00001m) } },
                StubZonedDateTime.UnixEpoch());
        }

        public static EntrySignal SellEntrySignal()
        {
            var tradeProfile = StubTradeProfileFactory.Create(20);

            return new EntrySignal(
                new Symbol("EURUSD", Venue.FXCM),
                new EntityId("TestSignal"),
                new Label("TestTrade"),
                tradeProfile,
                OrderSide.Sell,
                Price.Create(1.20500m, 0.00001m),
                Price.Create(1.20550m, 0.00001m),
                new SortedDictionary<int, Price> { { 1, Price.Create(1.20450m, 0.00001m) } },
                StubZonedDateTime.UnixEpoch());
        }

        public static ExitSignal LongExitSignalForAllUnits(
            TradeType tradeType,
            Period signalTimeOffset)
        {
            return new ExitSignal(
                new Symbol("AUDUSD", Venue.FXCM),
                new EntityId("TestSignal"),
                new Label("TestTrade"),
                tradeType,
                MarketPosition.Long,
                new List<int> { 0 },
                StubZonedDateTime.UnixEpoch() + signalTimeOffset.ToDuration());
        }

        public static ExitSignal ShortExitSignalForAllUnits(
            TradeType tradeType,
            Period signalTimeOffset)
        {
            return new ExitSignal(
                new Symbol("AUDUSD", Venue.FXCM),
                new EntityId("TestSignal"),
                new Label("TestTrade"),
                tradeType,
                MarketPosition.Short,
                new List<int> { 0 },
                StubZonedDateTime.UnixEpoch() + signalTimeOffset.ToDuration());
        }

        public static ExitSignal LongExitSignal(
            TradeType tradeType,
            List<int> forUnits,
            Period signalTimeOffset)
        {
            return new ExitSignal(
                new Symbol("AUDUSD", Venue.FXCM),
                new EntityId("TestSignal"),
                new Label("TestTrade"),
                tradeType,
                MarketPosition.Long,
                forUnits,
                StubZonedDateTime.UnixEpoch() + signalTimeOffset.ToDuration());
        }

        public static ExitSignal ShortExitSignal(
            TradeType tradeType,
            List<int> forUnits,
            Period signalTimeOffset)
        {
            return new ExitSignal(
                new Symbol("AUDUSD", Venue.FXCM),
                new EntityId("TestSignal"),
                new Label("TestTrade"),
                tradeType,
                MarketPosition.Short,
                forUnits,
                StubZonedDateTime.UnixEpoch() + signalTimeOffset.ToDuration());
        }

        public static TrailingStopSignal LongTrailingStopSignal(
            TradeType tradeType,
            Dictionary<int, Price> forUnitStoplossPrices,
            Period signalTimeOffset)
        {
            return new TrailingStopSignal(
                new Symbol("AUDUSD", Venue.FXCM),
                new EntityId("TestSignal"),
                new Label("TestTrade"),
                tradeType,
                MarketPosition.Long,
                forUnitStoplossPrices,
                StubZonedDateTime.UnixEpoch() + signalTimeOffset.ToDuration());
        }

        public static TrailingStopSignal ShortTrailingStopSignal(
            TradeType tradeType,
            Dictionary<int, Price> forUnitStoplossPrices,
            Period signalTimeOffset)
        {
            return new TrailingStopSignal(
                new Symbol("AUDUSD", Venue.FXCM),
                new EntityId("TestSignal"),
                new Label("TestTrade"),
                tradeType,
                MarketPosition.Short,
                forUnitStoplossPrices,
                StubZonedDateTime.UnixEpoch() + signalTimeOffset.ToDuration());
        }
    }
}