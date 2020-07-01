//--------------------------------------------------------------------------------------------------
// <copyright file="PerformanceHarnessTests.cs" company="Nautech Systems Pty Ltd">
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nautilus.TestSuite.TestKit.Performance;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.TestKitTests
{
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
