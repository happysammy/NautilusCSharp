//--------------------------------------------------------------------------------------------------
// <copyright file="NetMQTestBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Fixtures
{
    using System;
    using System.Threading.Tasks;
    using NetMQ;
    using Xunit.Abstractions;

    /// <summary>
    /// The base class for all tests involving NetMQ.
    /// </summary>
    // ReSharper disable once InconsistentNaming (correct name)
    public abstract class NetMQTestBase : TestBase, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetMQTestBase"/> class.
        /// </summary>
        /// <param name="output">The xunit output.</param>
        protected NetMQTestBase(ITestOutputHelper output)
            : base(output)
        {
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            NetMQConfig.Cleanup(false);
            Task.Delay(50).Wait();
        }
    }
}
