// -------------------------------------------------------------------------------------------------
// <copyright file="TickPublisherTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.NetworkTests.PublishersTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Threading.Tasks;
    using Nautilus.Common.Data;
    using Nautilus.Data.Publishers;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Network;
    using Nautilus.Serialization.Bson;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class TickPublisherTests : IDisposable
    {
        private const string TestAddress = "tcp://localhost:55606";
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter loggingAdapter;
        private readonly TickPublisher publisher;

        public TickPublisherTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerProvider();
            var container = containerFactory
            .Create();
            this.loggingAdapter = containerFactory
            .LoggingAdapter;
            this.publisher = new TickPublisher(
                container,
                DataBusFactory.Create(container),
                new TickDataSerializer(),
                EncryptionConfig.None(),
                new NetworkPort(55606));
        }

        public void Dispose()
        {
            Task.Delay(100).Wait();
            NetMQConfig.Cleanup();
            Task.Delay(100).Wait();
        }

        [Fact]
        internal void GivenTickMessage_WithSubscriber_PublishesMessage()
        {
            // Arrange
            this.publisher.Start();
            Task.Delay(100).Wait();

            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));

            var subscriber = new SubscriberSocket(TestAddress);
            subscriber.Connect(TestAddress);
            subscriber.Subscribe(symbol.Value);
            Task.Delay(100).Wait();

            var tick = StubTickProvider.Create(symbol);

            // Act
            this.publisher.Endpoint.Send(tick);

            var receivedTopic = subscriber.ReceiveFrameBytes();
            var receivedMessage = subscriber.ReceiveFrameBytes();

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(tick.Symbol.Value, Encoding.UTF8.GetString(receivedTopic));
            Assert.Equal(tick.ToString(), Encoding.UTF8.GetString(receivedMessage));

            subscriber.Disconnect(TestAddress);
            subscriber.Close();
            this.publisher.Stop();
        }
    }
}
