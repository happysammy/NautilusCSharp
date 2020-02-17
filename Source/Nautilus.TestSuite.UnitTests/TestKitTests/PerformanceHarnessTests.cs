//--------------------------------------------------------------------------------------------------
// <copyright file="PerformanceHarnessTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.TestKitTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.TestSuite.TestKit;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class PerformanceHarnessTests
    {
        [Fact]
        internal void Test_CanPerformTest_ReturnsReasonableTimeSpan()
        {
            // Arrange
            var numbersList = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // Act
            var result = PerformanceHarness.Test(() => numbersList.ToArray(), 3, 1000000);

            // Assert
            Assert.True(result.Milliseconds < 200);
        }
    }
}
