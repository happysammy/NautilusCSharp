// -------------------------------------------------------------------------------------------------
// <copyright file="TickPublisherTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests.PublishersTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Threading.Tasks;
    using Nautilus.Common.Interfaces;
    using Nautilus.Data.Network;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Network;
    using Nautilus.Serialization;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class TickPublisherTests
    {
        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer container;
        private readonly MockLoggingAdapter mockLoggingAdapter;

        public TickPublisherTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubComponentryContainerFactory();
            this.container = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;
        }

        [Fact]
        internal void GivenTickMessage_WithSubscriber_PublishesMessage()
        {
            // Arrange
            var publisher = new TickPublisher(
                this.container,
                new TickSerializer(),
                NetworkAddress.LocalHost,
                new NetworkPort(55506));
            publisher.Start();
            Task.Delay(100).Wait();

            var symbol = new Symbol("AUDUSD", Venue.FXCM);

            const string testAddress = "tcp://localhost:55506";
            var subscriber = new SubscriberSocket(testAddress);
            subscriber.Connect(testAddress);
            subscriber.Subscribe(symbol.ToString());
            Task.Delay(100).Wait();

            var tick = StubTickFactory.Create(symbol);

            // Act
            publisher.Endpoint.Send(tick);

            var receivedTopic = subscriber.ReceiveFrameBytes();
            var receivedMessage = subscriber.ReceiveFrameBytes();

            // Assert
            Assert.Equal(tick.Symbol.ToString(), Encoding.UTF8.GetString(receivedTopic));
            Assert.Equal(tick.ToString(), Encoding.UTF8.GetString(receivedMessage));

            // Tear Down
            subscriber.Unsubscribe(symbol.ToString());
            subscriber.Disconnect(testAddress);
            subscriber.Dispose();
            publisher.Stop();
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Task.Delay(100).Wait();  // Allows sockets to dispose
        }
    }
}
