//--------------------------------------------------------------------------------------------------
// <copyright file="BarAggregationControllerTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests.AggregatorTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Nautilus.Common.Messages.Commands;
    using Nautilus.Data.Aggregation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BarAggregationControllerTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter logger;
        private readonly MockMessagingAgent receiver;
        private readonly BarAggregationController controller;

        public BarAggregationControllerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubComponentryContainerFactory();
            this.logger = setupFactory.LoggingAdapter;
            this.receiver = new MockMessagingAgent();
            var container = setupFactory.Create();
            var messagingAdapter = new MockMessagingServiceFactory(container).MessagingAdapter;

            this.controller = new BarAggregationController(
                container,
                messagingAdapter,
                this.receiver.Endpoint);
        }

        [Fact]
        internal void GivenSubscribeBarDataMessage_CreatesAggregatorAndJobs()
        {
            // Arrange
            var symbol = new Symbol("AUDUSD", Venue.FXCM);
            var barType1 = new BarType(symbol, new BarSpecification(1, Resolution.SECOND, QuoteType.BID));
            var barType2 = new BarType(symbol, new BarSpecification(1, Resolution.MINUTE, QuoteType.BID));

            var subscribe1 = new Subscribe<BarType>(
                barType1,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<BarType>(
                barType1,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.controller.Endpoint.Send(subscribe1);
            this.controller.Endpoint.Send(subscribe2);

            // LogDumper.Dump(this.logger, this.output);
            // Assert
        }

        [Fact]
        internal void GivenMultipleSubscribeBarDataMessages_CreatesNeededJobs()
        {
            // Arrange
            var symbol1 = new Symbol("AUDUSD", Venue.FXCM);
            var barType1 = new BarType(symbol1, new BarSpecification(1, Resolution.SECOND, QuoteType.BID));
            var barType2 = new BarType(symbol1, new BarSpecification(1, Resolution.MINUTE, QuoteType.BID));

            var symbol2 = new Symbol("GBPUSD", Venue.FXCM);
            var barType3 = new BarType(symbol2, new BarSpecification(1, Resolution.SECOND, QuoteType.BID));
            var barType4 = new BarType(symbol2, new BarSpecification(1, Resolution.MINUTE, QuoteType.BID));

            var subscribe1 = new Subscribe<BarType>(
                barType1,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<BarType>(
                barType2,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe3 = new Subscribe<BarType>(
                barType3,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe4 = new Subscribe<BarType>(
                barType4,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.controller.Endpoint.Send(subscribe1);
            this.controller.Endpoint.Send(subscribe2);
            this.controller.Endpoint.Send(subscribe3);
            this.controller.Endpoint.Send(subscribe4);

            // LogDumper.Dump(this.logger, this.output);
            // Assert
        }

        [Fact]
        internal void GivenUnsubscribeBarDataMessage_RemovesJobs()
        {
            // Arrange
            var symbol = new Symbol("AUDUSD", Venue.FXCM);
            var barType1 = new BarType(symbol, new BarSpecification(1, Resolution.SECOND, QuoteType.BID));
            var barType2 = new BarType(symbol, new BarSpecification(1, Resolution.MINUTE, QuoteType.BID));

            var subscribe1 = new Subscribe<BarType>(
                barType1,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<BarType>(
                barType1,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe = new Unsubscribe<BarType>(
                barType2,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.controller.Endpoint.Send(subscribe1);
            this.controller.Endpoint.Send(subscribe2);
            Task.Delay(2000).Wait();
            this.controller.Endpoint.Send(unsubscribe);

            // LogDumper.Dump(this.logger, this.output);
            // Assert
        }

        [Fact]
        internal void GivenUnsubscribeBarDataMessage_WithMultipleJobsDoesNotRemoveTrigger()
        {
            // Arrange
            var symbol1 = new Symbol("AUDUSD", Venue.FXCM);
            var barType1 = new BarType(symbol1, new BarSpecification(1, Resolution.SECOND, QuoteType.BID));
            var barType2 = new BarType(symbol1, new BarSpecification(1, Resolution.MINUTE, QuoteType.BID));

            var symbol2 = new Symbol("GBPUSD", Venue.FXCM);
            var barType3 = new BarType(symbol2, new BarSpecification(1, Resolution.SECOND, QuoteType.BID));
            var barType4 = new BarType(symbol2, new BarSpecification(1, Resolution.MINUTE, QuoteType.BID));

            var subscribe1 = new Subscribe<BarType>(
                barType1,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe2 = new Subscribe<BarType>(
                barType2,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe3 = new Subscribe<BarType>(
                barType3,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var subscribe4 = new Subscribe<BarType>(
                barType4,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            var unsubscribe = new Unsubscribe<BarType>(
                barType2,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            this.controller.Endpoint.Send(subscribe1);
            this.controller.Endpoint.Send(subscribe2);
            this.controller.Endpoint.Send(subscribe3);
            this.controller.Endpoint.Send(subscribe4);
            Task.Delay(5000).Wait();
            this.controller.Endpoint.Send(unsubscribe);

            // LogDumper.Dump(this.logger, this.output);
            // Assert
        }
    }
}
