//--------------------------------------------------------------------------------------------------
// <copyright file="TestComponentryContainer.cs" company="Nautech Systems Pty Ltd">
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

using Microsoft.Extensions.Logging;
using Nautilus.Common.Interfaces;
using Nautilus.TestSuite.TestKit.Stubs;
using Xunit.Abstractions;

namespace Nautilus.TestSuite.TestKit.Components
{
    /// <inheritdoc/>
    public sealed class TestComponentryContainer : IComponentryContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestComponentryContainer"/> class.
        /// </summary>
        /// <param name="clock">The test clock.</param>
        /// <param name="guidFactory">The test guid factory.</param>
        /// <param name="loggerFactory">The test logger factory.</param>
        public TestComponentryContainer(
            TestClock clock,
            TestGuidFactory guidFactory,
            ILoggerFactory loggerFactory)
        {
            this.TestClock = clock;
            this.TestGuidFactory = guidFactory;
            this.LoggerFactory = loggerFactory;
        }

        /// <inheritdoc/>
        public IZonedClock Clock => this.TestClock;

        /// <inheritdoc/>
        public IGuidFactory GuidFactory => this.TestGuidFactory;

        /// <inheritdoc/>
        public ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Gets the containers test clock.
        /// </summary>
        public TestClock TestClock { get; }

        /// <summary>
        /// Gets the containers test guid factory.
        /// </summary>
        public TestGuidFactory TestGuidFactory { get; }

        /// <summary>
        /// Return a test componentry container.
        /// </summary>
        /// <param name="output">The test output helper for the test logger.</param>
        /// <returns>The container.</returns>
        public static TestComponentryContainer Create(ITestOutputHelper output)
        {
            var clock = new TestClock();
            clock.FreezeSetTime(StubZonedDateTime.UnixEpoch());

            return new TestComponentryContainer(
                clock,
                new TestGuidFactory(),
                new LoggerFactory(new[] { new TestLoggerProvider(output) }));
        }
    }
}
