// -------------------------------------------------------------------------------------------------
// <copyright file="MarketDataCollectorTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DatabaseTests.CoreTests.CollectorsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Database.Core.Collectors;
    using Nautilus.Database.Core.Messages.Commands;
    using Nautilus.Database.Core.Orchestration;
    using Nautilus.Database.Core.Readers;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using ServiceStack.Logging;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MarketDataCollectorTests : TestKit
    {
        private readonly ITestOutputHelper output;
        private readonly CsvBarDataReader dataReader;

        public MarketDataCollectorTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            this.dataReader = new CsvBarDataReader(
                StubSymbolBarData.AUDUSD(),
                new StubBarDataProvider());
        }

        [Fact]
        internal void CollectData_NotAtCollectionTime_ExpectNoMsg()
        {
            // Arrange
            var clock = new StubClock();
                clock.FreezeSetTime(StubZonedDateTime.UnixEpoch());

            var collectionSchedule = new DataCollectionSchedule(
                clock.TimeNow(),
                IsoDayOfWeek.Saturday,
                12,
                0,
                false,
                0);

            clock.FreezeSetTime(collectionSchedule.NextCollectionTime - Duration.FromHours(1));

            var marketDataCollector = this.Sys.ActorOf(
                Props.Create(() => new MarketDataCollector(
                    new ComponentryContainer(
                        clock,
                        new TestLogger()),
                    this.dataReader,
                    collectionSchedule)));

            // Act
            marketDataCollector.Tell(new CollectData(
                DataType.Bar,
                TestKitConstants.GetTestGuid,
                clock.TimeNow()));

            // Assert
            this.ExpectNoMsg();
        }

        [Fact]
        internal void ExecuteDataCollectors_PastCollectionTime_ReturnsTheNextBar()
        {
            // Arrange
            var clock = new StubClock();
            clock.FreezeSetTime(StubZonedDateTime.UnixEpoch());

            var collectionSchedule = new DataCollectionSchedule(
                clock.TimeNow(),
                IsoDayOfWeek.Saturday,
                12,
                0,
                false,
                0);

            clock.FreezeSetTime(collectionSchedule.NextCollectionTime + Duration.FromMinutes(1));

            var marketDataCollector = this.Sys.ActorOf(
                Props.Create(() => new MarketDataCollector(
                new ComponentryContainer(
                    clock,
                    new TestLogger()),
                this.dataReader,
                collectionSchedule)));

            //var result = this.ExpectMsg<MarketDataDelivery>(TimeSpan.FromSeconds(5));

            // Assert
            //Assert.Equal(7200, result.MarketData.Bars.Count);
        }
    }
}
