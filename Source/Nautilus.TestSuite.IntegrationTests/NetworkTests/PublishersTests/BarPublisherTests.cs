// -------------------------------------------------------------------------------------------------
// <copyright file="BarPublisherTests.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.ValueObjects;
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
    public sealed class BarPublisherTests : IDisposable
    {
        private const string TestAddress = "tcp://localhost:55511";
        private readonly ITestOutputHelper output;
        private readonly MockLogger logger;
        private readonly BarDataSerializer serializer;
        private readonly BarPublisher publisher;

        public BarPublisherTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerProvider();
            var container = containerFactory.Create();
            this.logger = containerFactory.Logger;
            this.serializer = new BarDataSerializer();

            this.publisher = new BarPublisher(
                container,
                DataBusFactory.Create(container),
                this.serializer,
                new CompressorBypass(),
                EncryptionSettings.None(),
                new NetworkPort(55511));
        }

        public void Dispose()
        {
            NetMQConfig.Cleanup(false);
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
            subscriber.Subscribe(barType.ToString());

            Task.Delay(100).Wait(); // Allow socket to subscribe

            var bar = StubBarData.Create();
            var data = new BarData(barType, bar);

            // Act
            this.publisher.Endpoint.Send(data);

            var topic = subscriber.ReceiveFrameBytes();
            var message = subscriber.ReceiveFrameBytes();

            // Assert
            Assert.Equal(barType.ToString(), Encoding.UTF8.GetString(topic));
            Assert.Equal(bar.ToString(), Encoding.UTF8.GetString(message));
            Assert.Equal(bar, this.serializer.Deserialize(message));

            // Tear Down
            LogDumper.DumpWithDelay(this.logger, this.output);
            subscriber.Disconnect(TestAddress);
            subscriber.Dispose();
            this.publisher.Stop();
            Task.Delay(100).Wait(); // Allow server to stop
            this.publisher.Dispose();
        }
    }
}
