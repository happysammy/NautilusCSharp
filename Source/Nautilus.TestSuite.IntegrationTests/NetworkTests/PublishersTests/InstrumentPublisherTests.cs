// -------------------------------------------------------------------------------------------------
// <copyright file="InstrumentPublisherTests.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Common.Interfaces;
    using Nautilus.Data.Publishers;
    using Nautilus.DomainModel.Entities;
    using Nautilus.Network;
    using Nautilus.Network.Compression;
    using Nautilus.Network.Encryption;
    using Nautilus.Serialization.DataSerializers;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class InstrumentPublisherTests : IDisposable
    {
        private const string TestAddress = "tcp://localhost:55512";
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter loggingAdapter;
        private readonly IDataSerializer<Instrument> serializer;
        private readonly InstrumentPublisher publisher;

        public InstrumentPublisherTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerProvider();
            var container = containerFactory.Create();
            this.loggingAdapter = containerFactory.LoggingAdapter;
            this.serializer = new InstrumentDataSerializer();

            this.publisher = new InstrumentPublisher(
                container,
                DataBusFactory.Create(container),
                this.serializer,
                new CompressorBypass(),
                EncryptionSettings.None(),
                new NetworkPort(55512));
        }

        public void Dispose()
        {
            NetMQConfig.Cleanup(false);
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
            subscriber.Subscribe(instrument.Symbol.Value);
            Task.Delay(100).Wait();

            // Act
            this.publisher.Endpoint.Send(instrument);

            var topic = subscriber.ReceiveFrameBytes();
            var message = subscriber.ReceiveFrameBytes();

            // Assert
            Assert.Equal(instrument.Symbol.Value, Encoding.UTF8.GetString(topic));
            Assert.Equal(instrument, this.serializer.Deserialize(message));

            // Tear Down
            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);
            subscriber.Disconnect(TestAddress);
            subscriber.Dispose();
            this.publisher.Stop();
            Task.Delay(100).Wait(); // Allow server to stop
            this.publisher.Dispose();
        }
    }
}
