//--------------------------------------------------------------------------------------------------
// <copyright file="TestComponentryContainer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Components
{
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Interfaces;
    using Nautilus.TestSuite.TestKit.Stubs;
    using Xunit.Abstractions;

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
