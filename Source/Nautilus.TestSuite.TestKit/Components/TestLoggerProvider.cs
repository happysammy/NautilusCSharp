//--------------------------------------------------------------------------------------------------
// <copyright file="TestLoggerProvider.cs" company="Nautech Systems Pty Ltd">
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
using Xunit.Abstractions;

namespace Nautilus.TestSuite.TestKit.Components
{
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
