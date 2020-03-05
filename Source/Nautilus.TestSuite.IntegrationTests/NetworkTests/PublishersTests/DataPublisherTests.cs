// -------------------------------------------------------------------------------------------------
// <copyright file="DataPublisherTests.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
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
    public sealed class DataPublisherTests : NetMQTestBase
    {
        private const string TestAddress = "tcp://localhost:55511";
        private readonly BarDataSerializer barDataSerializer;
        private readonly InstrumentDataSerializer instrumentDataSerializer;
        private readonly DataPublisher publisher;

        public DataPublisherTests(ITestOutputHelper output)
            : base(output)
        {
            // Fixture Setup
            var container = TestComponentryContainer.Create(output);
            this.barDataSerializer = new BarDataSerializer();
            this.instrumentDataSerializer = new InstrumentDataSerializer();

            this.publisher = new DataPublisher(
                container,
                DataBusFactory.Create(container),
                this.barDataSerializer,
                this.instrumentDataSerializer,
                new CompressorBypass(),
                EncryptionSettings.None(),
                new Port(55511));
        }

        [Fact]
        internal void GivenBarData_WithSubscriber_PublishesMessage()
        {
            // Arrange
            this.publisher.Start();
            Task.Delay(100).Wait();  // Allow publisher to start

            var barType = StubBarType.AUDUSD_OneMinuteAsk();

            var subscriber = new SubscriberSocket(TestAddress);
            subscriber.Connect(TestAddress);
            subscriber.Subscribe(nameof(Bar) + ":" + barType);

            Task.Delay(100).Wait(); // Allow socket to subscribe

            var bar = StubBarData.Create();
            var data = new BarData(barType, bar);

            // Act
            this.publisher.Endpoint.SendAsync(data);

            var topic = subscriber.ReceiveFrameBytes();
            var message = subscriber.ReceiveFrameBytes();

            // Assert
            Assert.Equal(1, this.publisher.SentCount);
            Assert.Equal("Bar:AUDUSD.FXCM-1-MINUTE[ASK]", Encoding.UTF8.GetString(topic));
            Assert.Equal(bar.ToString(), Encoding.UTF8.GetString(message));
            Assert.Equal(bar, this.barDataSerializer.Deserialize(message));

            // Tear Down
            subscriber.Disconnect(TestAddress);
            subscriber.Dispose();
            this.publisher.Stop().Wait();
            this.publisher.Dispose();
        }

        [Fact]
        internal void GivenInstrument_WithSubscriber_PublishesMessage()
        {
            // Arrange
            this.publisher.Start();
            Task.Delay(100).Wait(); // Allow publisher to start

            var instrument = StubInstrumentProvider.AUDUSD();

            var subscriber = new SubscriberSocket(TestAddress);
            subscriber.Connect(TestAddress);
            subscriber.Subscribe(nameof(Instrument) + ":" + instrument.Symbol.Value);
            Task.Delay(100).Wait();

            // Act
            this.publisher.Endpoint.SendAsync(instrument);

            var topic = subscriber.ReceiveFrameBytes();
            var message = subscriber.ReceiveFrameBytes();

            // Assert
            Assert.Equal("Instrument:AUDUSD.FXCM", Encoding.UTF8.GetString(topic));
            Assert.Equal(instrument, this.instrumentDataSerializer.Deserialize(message));

            // Tear Down
            subscriber.Disconnect(TestAddress);
            subscriber.Dispose();
            this.publisher.Stop().Wait();
            this.publisher.Dispose();
        }
    }
}
