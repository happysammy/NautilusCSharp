// -------------------------------------------------------------------------------------------------
// <copyright file="DataPublisherTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using Nautilus.Common.Data;
using Nautilus.Data.Network;
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

namespace Nautilus.TestSuite.IntegrationTests.NetworkTests.PublishersTests
{
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
            Assert.Equal("Bar:AUDUSD.FXCM-1-MINUTE-ASK", Encoding.UTF8.GetString(topic));
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
