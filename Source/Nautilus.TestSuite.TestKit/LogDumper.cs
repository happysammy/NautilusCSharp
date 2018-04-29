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
    /// The trouble shooter.
    /// </summary>
    public static class LogDumper
    {
        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="mockLogger">
        /// The mock logger.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <param name="delayMilliseconds">
        /// The delay Milliseconds.
        /// </param>
        public static void Dump(
            MockLogger mockLogger,
            ITestOutputHelper output,
            int delayMilliseconds = 300)
        {
            Task.Delay(delayMilliseconds).Wait();
            mockLogger.WriteStashToOutput(output);
        }
    }
}
