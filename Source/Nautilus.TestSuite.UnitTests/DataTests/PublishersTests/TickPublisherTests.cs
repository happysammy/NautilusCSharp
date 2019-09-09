// -------------------------------------------------------------------------------------------------
// <copyright file="TickPublisherTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests.PublishersTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Threading.Tasks;
    using Nautilus.Common.Data;
    using Nautilus.Data.Publishers;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Network;
    using Nautilus.Serialization;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("ReSharper", "SA1310", Justification = "Easier to read.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class TickPublisherTests
    {
        private const string TEST_ADDRESS = "tcp://localhost:55506";
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter loggingAdapter;
        private readonly TickPublisher publisher;

        public TickPublisherTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerFactory();
            var container = containerFactory
            .Create();
            this.loggingAdapter = containerFactory
            .LoggingAdapter;
            this.publisher = new TickPublisher(
                container,
                DataBusFactory.Create(container),
                new Utf8TickSerializer(),
                new NetworkPort(55506));
        }

        [Fact]
        internal void GivenTickMessage_WithSubscriber_PublishesMessage()
        {
            // Arrange
            this.publisher.Start();
            Task.Delay(100).Wait();

            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));

            var subscriber = new SubscriberSocket(TEST_ADDRESS);
            subscriber.Connect(TEST_ADDRESS);
            subscriber.Subscribe(symbol.Value);
            Task.Delay(100).Wait();

            var tick = StubTickFactory.Create(symbol);

            // Act
            this.publisher.Endpoint.Send(tick);

            var receivedTopic = subscriber.ReceiveFrameBytes();
            var receivedMessage = subscriber.ReceiveFrameBytes();

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(tick.Symbol.Value, Encoding.UTF8.GetString(receivedTopic));
            Assert.Equal(tick.ToString(), Encoding.UTF8.GetString(receivedMessage));

            // Tear Down
            subscriber.Unsubscribe(symbol.ToString());
            subscriber.Disconnect(TEST_ADDRESS);
            subscriber.Dispose();
            this.publisher.Stop();
            Task.Delay(100).Wait();  // Allows sockets to dispose
        }
    }
}
