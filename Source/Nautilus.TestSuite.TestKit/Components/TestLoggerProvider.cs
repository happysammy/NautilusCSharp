//--------------------------------------------------------------------------------------------------
// <copyright file="TestLoggerProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Components
{
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;

    /// <summary>
    /// Provides a logger for testing purposes.
    /// </summary>
    public sealed class TestLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper output;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestLoggerProvider"/> class.
        /// </summary>
        /// <param name="output">The test output helper.</param>
        public TestLoggerProvider(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <inheritdoc/>
        public ILogger CreateLogger(string sourceContext)
        {
            return new TestLogger(this.output, sourceContext);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}
