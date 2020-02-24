//--------------------------------------------------------------------------------------------------
// <copyright file="TestBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit
{
    using Xunit.Abstractions;

    /// <summary>
    /// The base class for all tests.
    /// </summary>
    public abstract class TestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestBase"/> class.
        /// </summary>
        /// <param name="output">The xunit output.</param>
        protected TestBase(ITestOutputHelper output)
        {
            this.Output = output;
        }

        /// <summary>
        /// Gets the XUnit output.
        /// </summary>
        protected ITestOutputHelper Output { get; }
    }
}
