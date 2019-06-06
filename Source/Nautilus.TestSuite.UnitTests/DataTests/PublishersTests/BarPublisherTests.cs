// -------------------------------------------------------------------------------------------------
// <copyright file="BarPublisherTests.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Network;
    using Nautilus.Serialization;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NetMQ;
    using NetMQ.Sockets;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public sealed class BarPublisherTests
    {
        private readonly ITestOutputHelper output;
        private readonly IComponentryContainer setupContainer;
        private readonly MockLoggingAdapter mockLoggingAdapter;

        public BarPublisherTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubComponentryContainerFactory();
            this.setupContainer = setupFactory.Create();
            this.mockLoggingAdapter = setupFactory.LoggingAdapter;
        }

        [Fact]
        internal void GivenBarClosedMessage_WithSubscriber_PublishesMessage()
        {
            // Arrange
            var publisher = new BarPublisher(
                this.setupContainer,
                new BarSerializer(),
                NetworkAddress.LocalHost,
                new NetworkPort(55511));
            publisher.Start();
            Task.Delay(100).Wait();

            var barType = StubBarType.AUDUSD();

            const string testAddress = "tcp://localhost:55511";
            var subscriber = new SubscriberSocket(testAddress);
            subscriber.Connect(testAddress);
            subscriber.Subscribe(barType.ToString());
            Task.Delay(100).Wait();

            var bar = StubBarData.Create();
            var data = (barType, bar);

            // Act
            publisher.Endpoint.Send(data);

            var topic = subscriber.ReceiveFrameBytes();
            var message = subscriber.ReceiveFrameBytes();

            // Assert
            Assert.Equal(barType.ToString(), Encoding.UTF8.GetString(topic));
            Assert.Equal(bar.ToString(), Encoding.UTF8.GetString(message));

            // Tear Down
            subscriber.Unsubscribe(barType.ToString());
            subscriber.Disconnect(testAddress);
            subscriber.Dispose();
            publisher.Stop();
            LogDumper.Dump(this.mockLoggingAdapter, this.output);
            Task.Delay(100).Wait();  // Allows sockets to dispose
        }
    }
}
