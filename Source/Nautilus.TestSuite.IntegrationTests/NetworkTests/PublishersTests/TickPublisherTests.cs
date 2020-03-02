// -------------------------------------------------------------------------------------------------
// <copyright file="TickPublisherTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.NetworkTests.PublishersTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Threading.Tasks;
    using Nautilus.Common.Data;
    using Nautilus.Data.Publishers;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Network;
    using Nautilus.Network.Compression;
    using Nautilus.Network.Encryption;
    using Nautilus.Serialization.DataSerializers;
    using Nautilus.TestSuite.TestKit.Components;
    using Nautilus.TestSuite.TestKit.Fixtures;
    using Nautilus.TestSuite.TestKit.Stubs;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class TickPublisherTests : NetMQTestBase
    {
        private const string TestAddress = "tcp://localhost:55606";
        private readonly TickPublisher publisher;

        public TickPublisherTests(ITestOutputHelper output)
            : base(output)
        {
            // Fixture Setup
            var container = TestComponentryContainer.Create(output);
            this.publisher = new TickPublisher(
                container,
                DataBusFactory.Create(container),
                new TickDataSerializer(),
                new CompressorBypass(),
                EncryptionSettings.None(),
                new Port(55606));

            this.publisher.Start().Wait();
        }

        [Fact]
        internal void GivenTickMessage_WithSubscriber_PublishesMessage()
        {
            // Arrange
            var symbol = new Symbol("AUDUSD", new Venue("FXCM"));

            var subscriber = new SubscriberSocket(TestAddress);
            subscriber.Connect(TestAddress);
            subscriber.Subscribe(symbol.Value);

            Task.Delay(100).Wait();  // Allow subscribers to connect

            var tick = StubTickProvider.Create(symbol);

            // Act
            this.publisher.Endpoint.Send(tick);

            var topic = subscriber.ReceiveFrameBytes();
            var message = subscriber.ReceiveFrameBytes();

            // Assert
            Assert.Equal(1, this.publisher.SentCount);
            Assert.Equal(tick.Symbol.Value, Encoding.UTF8.GetString(topic));
            Assert.Equal(tick.ToString(), Encoding.UTF8.GetString(message));

            // Tear Down
            subscriber.Disconnect(TestAddress);
            subscriber.Dispose();
            this.publisher.Stop().Wait();
            this.publisher.Dispose();
        }
    }
}
