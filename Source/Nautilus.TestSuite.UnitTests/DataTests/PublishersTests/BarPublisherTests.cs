// -------------------------------------------------------------------------------------------------
// <copyright file="BarPublisherTests.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.ValueObjects;
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
    public sealed class BarPublisherTests
    {
        private const string TEST_ADDRESS = "tcp://localhost:55511";
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter loggingAdapter;
        private readonly Utf8BarSerializer serializer;
        private readonly BarPublisher publisher;

        public BarPublisherTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerFactory();
            var container = containerFactory.Create();
            this.loggingAdapter = containerFactory.LoggingAdapter;
            this.serializer = new Utf8BarSerializer();

            this.publisher = new BarPublisher(
                container,
                DataBusFactory.Create(container),
                this.serializer,
                new NetworkPort(55511));
        }

        [Fact]
        internal void GivenBarData_WithSubscriber_PublishesMessage()
        {
            // Arrange
            this.publisher.Start();
            Task.Delay(100).Wait();  // Allow publisher to start

            var barType = StubBarType.AUDUSD();

            var subscriber = new SubscriberSocket(TEST_ADDRESS);
            subscriber.Connect(TEST_ADDRESS);
            subscriber.Subscribe(barType.ToString());
            Task.Delay(100).Wait();

            var bar = StubBarData.Create();
            var data = new BarData(barType, bar);

            // Act
            this.publisher.Endpoint.Send(data);

            var topic = subscriber.ReceiveFrameBytes();
            var message = subscriber.ReceiveFrameBytes();

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(barType.ToString(), Encoding.UTF8.GetString(topic));
            Assert.Equal(bar.ToString(), Encoding.UTF8.GetString(message));
            Assert.Equal(bar, this.serializer.Deserialize(message));

            // Tear Down
            subscriber.Unsubscribe(barType.ToString());
            subscriber.Disconnect(TEST_ADDRESS);
            this.publisher.Stop();
            Task.Delay(100).Wait();
        }
    }
}
