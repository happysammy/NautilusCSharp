//--------------------------------------------------------------------------------------------------
// <copyright file="TimeBarAggregatorTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests
{
    using System.Diagnostics.CodeAnalysis;
    using Akka.Actor;
    using Akka.TestKit.Xunit2;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Data;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BarAggregatorTests : TestKit
    {
        private readonly ITestOutputHelper output;
        private readonly BlackBoxContainer container;
        private readonly Symbol symbol;

        public BarAggregatorTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            this.container = setupFactory.Create();

            this.symbol = new Symbol("AUDUSD", Exchange.LMAX);
        }

        [Fact]
        internal void OnFirstTick_DoesNothing()
        {
            // Arrange
            var symbolBarSpec = new SymbolBarSpec(
                this.symbol,
                new BarSpecification(BarQuoteType.Bid, BarResolution.Tick, 5));

            var props = Props.Create(() => new BarAggregator(
                this.container,
                BlackBoxService.Data,
                symbolBarSpec,
                0.00001m));

            var barAggregatorRef = this.ActorOfAsTestActorRef<BarAggregator>(props, TestActor);

            var quote1 = new Tick(
                this.symbol,
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80005m, 0.00001m),
                StubDateTime.Now() + Duration.FromMinutes(1));

            // Act
            barAggregatorRef.Tell(quote1);

            // Assert
            ExpectNoMsg();
        }

        [Fact]
        internal void GivenTicks_WhenSecondBar_ReturnsValidBar()
        {
            // Arrange
            var symbolBarSpec = new SymbolBarSpec(
                this.symbol,
                new BarSpecification(BarQuoteType.Bid, BarResolution.Second, 30));

            var props = Props.Create(() => new BarAggregator(
                this.container,
                BlackBoxService.Data,
                symbolBarSpec,
                0.00001m));

            var barAggregatorRef = this.ActorOfAsTestActorRef<BarAggregator>(props, TestActor);

            var quote1 = new Tick(
                this.symbol,
                Price.Create(1.00010m, 5),
                Price.Create(1.00020m, 5),
                StubDateTime.Now() + Duration.FromSeconds(10));

            var quote2 = new Tick(
                this.symbol,
                Price.Create(1.00015m, 5),
                Price.Create(1.00025m, 5),
                StubDateTime.Now() + Duration.FromSeconds(20));

            var quote3 = new Tick(
                this.symbol,
                Price.Create(1.00005m, 5),
                Price.Create(1.00010m, 5),
                StubDateTime.Now() + Duration.FromSeconds(30));

            // Act
            barAggregatorRef.Tell(quote1);
            barAggregatorRef.Tell(quote2);
            barAggregatorRef.Tell(quote3);

            // Assert
            var result = ExpectMsg<BarDataEvent>();
        }

        [Fact]
        internal void GivenTicks_WhenMinuteBar_ReturnsValidBar()
        {
            // Arrange
            var symbolBarSpec = new SymbolBarSpec(
                this.symbol,
                new BarSpecification(BarQuoteType.Bid, BarResolution.Minute, 5));

            var props = Props.Create(() => new BarAggregator(
                this.container,
                BlackBoxService.Data,
                symbolBarSpec,
                0.00001m));

            var barAggregatorRef = this.ActorOfAsTestActorRef<BarAggregator>(props, TestActor);

            var quote1 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(1));

            var quote2 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(2));

            var quote3 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(3));

            var quote4 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(4));

            var quote5 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(5));

            // Act
            barAggregatorRef.Tell(quote1);
            barAggregatorRef.Tell(quote2);
            barAggregatorRef.Tell(quote3);
            barAggregatorRef.Tell(quote4);
            barAggregatorRef.Tell(quote5);

            // Assert
            var result = ExpectMsg<BarDataEvent>();
        }

        [Fact]
        internal void GivenTicks_WhenHourBar_ReturnsValidBar()
        {
            // Arrange
            var symbolBarSpec = new SymbolBarSpec(
                this.symbol,
                new BarSpecification(BarQuoteType.Bid, BarResolution.Hour, 1));

            var props = Props.Create(() => new BarAggregator(
                this.container,
                BlackBoxService.Data,
                symbolBarSpec,
                0.00001m));

            var barAggregatorRef = this.ActorOfAsTestActorRef<BarAggregator>(props, TestActor);

            var quote1 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(10));

            var quote2 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(20));

            var quote3 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(30));

            var quote4 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(40));

            var quote5 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMinutes(60));

            // Act
            barAggregatorRef.Tell(quote1);
            barAggregatorRef.Tell(quote2);
            barAggregatorRef.Tell(quote3);
            barAggregatorRef.Tell(quote4);
            barAggregatorRef.Tell(quote5);

            // Assert
            var result = ExpectMsg<BarDataEvent>();
        }

        [Fact]
        internal void GivenTicks_WhenTickBarAndReachedTicks_ReturnsValidBar()
        {
            // Arrange
            var symbolBarSpec = new SymbolBarSpec(
                this.symbol,
                new BarSpecification(BarQuoteType.Bid, BarResolution.Tick, 5));

            var props = Props.Create(() => new BarAggregator(
                this.container,
                BlackBoxService.Data,
                symbolBarSpec,
                0.00001m));

            var barAggregatorRef = this.ActorOfAsTestActorRef<BarAggregator>(props, TestActor);

            var quote1 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromSeconds(1));

            var quote2 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromSeconds(2));

            var quote3 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromSeconds(3));

            var quote4 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromSeconds(4));

            var quote5 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromSeconds(5));

            // Act
            barAggregatorRef.Tell(quote1);
            barAggregatorRef.Tell(quote2);
            barAggregatorRef.Tell(quote3);
            barAggregatorRef.Tell(quote4);
            barAggregatorRef.Tell(quote5);

            // Assert
            // Assert
            var result = ExpectMsg<BarDataEvent>();
        }

        [Fact]
        internal void GivenTicks_WhenSecondBarAndSecondBarsMissed1_ReturnsValidBars()
        {
            // Arrange
            var symbolBarSpec = new SymbolBarSpec(
                this.symbol,
                new BarSpecification(BarQuoteType.Bid, BarResolution.Second, 1));

            var props = Props.Create(() => new BarAggregator(
                this.container,
                BlackBoxService.Data,
                symbolBarSpec,
                0.00001m));

            var barAggregatorRef = this.ActorOfAsTestActorRef<BarAggregator>(props, TestActor);

            var quote1 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMilliseconds(1));

            var quote2 = new Tick(
                this.symbol,
                Price.Create(2, 1),
                Price.Create(2, 1),
                StubDateTime.Now() + Duration.FromSeconds(1));

            var quote3 = new Tick(
                this.symbol,
                Price.Create(3, 1),
                Price.Create(3, 1),
                StubDateTime.Now() + Duration.FromSeconds(3));

            var quote4 = new Tick(
                this.symbol,
                Price.Create(0.5m, 1),
                Price.Create(1m, 1),
                StubDateTime.Now() + Duration.FromSeconds(4));

            // Act
            barAggregatorRef.Tell(quote1);
            barAggregatorRef.Tell(quote2);
            barAggregatorRef.Tell(quote3);
            barAggregatorRef.Tell(quote4);

            var result1 = ExpectMsg<BarDataEvent>();
            var result2 = ExpectMsg<BarDataEvent>();
            var result3 = ExpectMsg<BarDataEvent>();
            var result4 = ExpectMsg<BarDataEvent>();

            // Assert
            Assert.Equal(1, result1.Bar.Open.Value);
            Assert.Equal(2, result1.Bar.High.Value);
            Assert.Equal(StubDateTime.Now() + Duration.FromSeconds(1), result1.Bar.Timestamp);

            Assert.Equal(3, result2.Bar.Open.Value);
            Assert.Equal(3, result2.Bar.High.Value);
            Assert.Equal(3, result2.Bar.Low.Value);
            Assert.Equal(3, result2.Bar.Close.Value);
            Assert.Equal(StubDateTime.Now() + Duration.FromSeconds(2), result2.Bar.Timestamp);

            Assert.Equal(3, result3.Bar.Open.Value);
            Assert.Equal(3, result3.Bar.High.Value);
            Assert.Equal(3, result3.Bar.Low.Value);
            Assert.Equal(3, result3.Bar.Close.Value);
            Assert.Equal(StubDateTime.Now() + Duration.FromSeconds(3), result3.Bar.Timestamp);

            Assert.Equal(0.5m, result4.Bar.High.Value);
            Assert.Equal(0.5m, result4.Bar.Low.Value);
            Assert.Equal(StubDateTime.Now() + Duration.FromSeconds(4), result4.Bar.Timestamp);
        }

        [Fact]
        internal void GivenTicks_WhenSecondBarAndSecondBarsMissed2_ReturnsValidBars()
        {
            // Arrange
            var symbolBarSpec = new SymbolBarSpec(
                this.symbol,
                new BarSpecification(BarQuoteType.Bid, BarResolution.Second, 1));

            var props = Props.Create(() => new BarAggregator(
                this.container,
                BlackBoxService.Data,
                symbolBarSpec,
                0.00001m));

            var barAggregatorRef = this.ActorOfAsTestActorRef<BarAggregator>(props, TestActor);

            var quote1 = new Tick(
                this.symbol,
                Price.Create(1, 1),
                Price.Create(1, 1),
                StubDateTime.Now() + Duration.FromMilliseconds(1));

            var quote2 = new Tick(
                this.symbol,
                Price.Create(2, 1),
                Price.Create(2, 1),
                StubDateTime.Now() + Duration.FromSeconds(1));

            var quote3 = new Tick(
                this.symbol,
                Price.Create(3, 1),
                Price.Create(3, 1),
                StubDateTime.Now() + Duration.FromSeconds(3));

            var quote4 = new Tick(
                this.symbol,
                Price.Create(0.5m, 1),
                Price.Create(1m, 1),
                StubDateTime.Now() + Duration.FromSeconds(5));

            // Act
            barAggregatorRef.Tell(quote1);
            barAggregatorRef.Tell(quote2);
            barAggregatorRef.Tell(quote3);
            barAggregatorRef.Tell(quote4);

            var result1 = ExpectMsg<BarDataEvent>();
            var result2 = ExpectMsg<BarDataEvent>();
            var result3 = ExpectMsg<BarDataEvent>();
            var result4 = ExpectMsg<BarDataEvent>();
            var result5 = ExpectMsg<BarDataEvent>();

            // Assert
            Assert.Equal(1, result1.Bar.Open.Value);
            Assert.Equal(2, result1.Bar.High.Value);
            Assert.Equal(StubDateTime.Now() + Duration.FromSeconds(1), result1.Bar.Timestamp);

            Assert.Equal(3, result2.Bar.Open.Value);
            Assert.Equal(3, result2.Bar.High.Value);
            Assert.Equal(3, result2.Bar.Low.Value);
            Assert.Equal(3, result2.Bar.Close.Value);
            Assert.Equal(StubDateTime.Now() + Duration.FromSeconds(2), result2.Bar.Timestamp);

            Assert.Equal(3, result3.Bar.Open.Value);
            Assert.Equal(3, result3.Bar.High.Value);
            Assert.Equal(3, result3.Bar.Low.Value);
            Assert.Equal(3, result3.Bar.Close.Value);
            Assert.Equal(StubDateTime.Now() + Duration.FromSeconds(3), result3.Bar.Timestamp);

            Assert.Equal(0.5m, result4.Bar.Open.Value);
            Assert.Equal(0.5m, result4.Bar.High.Value);
            Assert.Equal(0.5m, result4.Bar.Low.Value);
            Assert.Equal(0.5m, result4.Bar.Close.Value);
            Assert.Equal(StubDateTime.Now() + Duration.FromSeconds(4), result4.Bar.Timestamp);

            Assert.Equal(0.5m, result5.Bar.Open.Value);
            Assert.Equal(0.5m, result5.Bar.High.Value);
            Assert.Equal(0.5m, result5.Bar.Low.Value);
            Assert.Equal(0.5m, result5.Bar.Close.Value);
            Assert.Equal(StubDateTime.Now() + Duration.FromSeconds(5), result5.Bar.Timestamp);
        }
    }
}
