//--------------------------------------------------------------------------------------------------
// <copyright file="LogDumper.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit
{
    using System.Threading.Tasks;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit.Abstractions;

    /// <summary>
    /// Provides a convenient way of viewing the contents of the mock logging adapter.
    /// </summary>
    public static class LogDumper
    {
        /// <summary>
        /// Writes the contents of the log to the output.
        /// </summary>
        /// <param name="mockLoggingAdapter">The mock logging adapter.</param>
        /// <param name="output">The test output.</param>
        /// <param name="delayMilliseconds">The delay (milliseconds).</param>
        public static void Dump(
            MockLoggingAdapter mockLoggingAdapter,
            ITestOutputHelper output,
            int delayMilliseconds=300)
        {
            Task.Delay(delayMilliseconds).Wait();
            mockLoggingAdapter.WriteStashToOutput(output);
        }
    }
}
