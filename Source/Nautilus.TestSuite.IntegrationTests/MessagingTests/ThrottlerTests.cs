//--------------------------------------------------------------------------------------------------
// <copyright file="ThrottlerTests.cs" company="Nautech Systems Pty Ltd">
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
//--------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Nautilus.Common.Interfaces;
using Nautilus.Common.Messaging;
using Nautilus.TestSuite.TestKit.Components;
using Nautilus.TestSuite.TestKit.Mocks;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace Nautilus.TestSuite.IntegrationTests.MessagingTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class ThrottlerTests
    {
        private readonly IComponentryContainer container;
        private readonly MockComponent receiver;

        public ThrottlerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.container = TestComponentryContainer.Create(output);
            this.receiver = new MockComponent(this.container);
            this.receiver.RegisterHandler<string>(this.receiver.OnMessage);
        }

        [Fact]
        internal void CanThrottleMessagesPer100Milliseconds_Test1()
        {
            // Arrange
            var throttler = new Throttler<string>(
                this.container,
                this.receiver.Endpoint.Send,
                Duration.FromMilliseconds(100),
                10,
                "1");

            // Act
            for (var i = 0; i < 21; i++)
            {
                throttler.Endpoint.SendAsync($"Message-{i + 1}");
            }

            Task.Delay(10).Wait();

            // Should receives only the first 10 messages
            var count1 = this.receiver.Messages.Count;

            // Wait for the throttle duration interval
            // Should receive the next 10 messages
            Task.Delay(100).Wait();
            throttler.Stop().Wait();

            // Assert
            Assert.Equal(10, count1);
            Assert.Equal(20, this.receiver.Messages.Count);
            Assert.Equal(1, throttler.QueueCount);
            Assert.True(throttler.IsActive);
        }

        [Fact]
        internal void CanThrottleMessagesPer100Milliseconds_Test2()
        {
            // Arrange
            var throttler = new Throttler<string>(
                this.container,
                this.receiver.Endpoint.Send,
                Duration.FromMilliseconds(100),
                10,
                "1");

            // Act
            for (var i = 0; i < 11; i++)
            {
                throttler.Endpoint.SendAsync($"Message-{i + 1}");
            }

            Task.Delay(50).Wait();

            // Should receive only the first 10 messages
            var count1 = this.receiver.Messages.Count;

            for (var i = 0; i < 20; i++)
            {
                throttler.Endpoint.SendAsync($"Message2-{i + 1}");
            }

            // Wait for all messages to send
            // Receives the next 100 messages
            Task.Delay(500).Wait();
            throttler.Stop().Wait();

            // Assert
            Assert.Equal(10, count1);
            Assert.Equal(31, this.receiver.Messages.Count);
            Assert.Equal(0, throttler.QueueCount);
            Assert.True(throttler.IsIdle);
        }
    }
}
