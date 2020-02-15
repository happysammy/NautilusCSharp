//--------------------------------------------------------------------------------------------------
// <copyright file="LogDumper.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit
{
    using System.Threading.Tasks;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit.Abstractions;

    /// <summary>
    /// Provides a convenient way of viewing the contents of a <see cref="MockLoggingAdapter"/>.
    /// </summary>
    public static class LogDumper
    {
        /// <summary>
        /// Writes the contents of the log to the output after the given delay.
        /// </summary>
        /// <param name="loggingAdapter">The mock logging adapter.</param>
        /// <param name="output">The test output.</param>
        /// <param name="delayMilliseconds">The delay (milliseconds).</param>
        public static void DumpWithDelay(
            MockLoggingAdapter loggingAdapter,
            ITestOutputHelper output,
            int delayMilliseconds = 300)
        {
            Task.Delay(delayMilliseconds).Wait();
            loggingAdapter.WriteStashToOutput(output);
        }

        /// <summary>
        /// Writes the contents of the log to the output.
        /// </summary>
        /// <param name="loggingAdapter">The mock logging adapter.</param>
        /// <param name="output">The test output.</param>
        public static void Dump(MockLoggingAdapter loggingAdapter, ITestOutputHelper output)
        {
            loggingAdapter.WriteStashToOutput(output);
        }
    }
}
