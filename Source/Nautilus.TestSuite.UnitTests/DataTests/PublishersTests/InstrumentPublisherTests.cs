// -------------------------------------------------------------------------------------------------
// <copyright file="InstrumentPublisherTests.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Common.Data;
    using Nautilus.Data.Publishers;
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
    public sealed class InstrumentPublisherTests
    {
        private const string TEST_ADDRESS = "tcp://localhost:55512";
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdapter loggingAdapter;
        private readonly MsgPackInstrumentSerializer serializer;
        private readonly InstrumentPublisher publisher;

        public InstrumentPublisherTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var containerFactory = new StubComponentryContainerFactory();
            var container = containerFactory.Create();
            this.loggingAdapter = containerFactory.LoggingAdapter;
            this.serializer = new MsgPackInstrumentSerializer();

            this.publisher = new InstrumentPublisher(
                container,
                DataBusFactory.Create(container),
                this.serializer,
                NetworkAddress.LocalHost,
                new NetworkPort(55512));
        }

        [Fact]
        internal void GivenInstrument_WithSubscriber_PublishesMessage()
        {
            // Arrange
            this.publisher.Start();
            Task.Delay(100).Wait();

            var instrument = StubInstrumentFactory.AUDUSD();

            var subscriber = new SubscriberSocket(TEST_ADDRESS);
            subscriber.Connect(TEST_ADDRESS);
            subscriber.Subscribe(instrument.Symbol.ToString());
            Task.Delay(100).Wait();

            // Act
            this.publisher.Endpoint.Send(instrument);

            var topic = subscriber.ReceiveFrameBytes();
            var message = subscriber.ReceiveFrameBytes();

            LogDumper.DumpWithDelay(this.loggingAdapter, this.output);

            // Assert
            Assert.Equal(instrument.Symbol.ToString(), Encoding.UTF8.GetString(topic));
            Assert.Equal(instrument, this.serializer.Deserialize(message));

            // Tear Down
            subscriber.Unsubscribe(instrument.Symbol.ToString());
            subscriber.Disconnect(TEST_ADDRESS);
            subscriber.Dispose();
            this.publisher.Stop();
            Task.Delay(100).Wait();  // Allows sockets to dispose
        }
    }
}
