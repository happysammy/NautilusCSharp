//--------------------------------------------------------------------------------------------------
// <copyright file="IdentifierTests.cs" company="Nautech Systems Pty Ltd">
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


using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nautilus.Core;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.CoreTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Test Suite")]
    public sealed class ParserTests
    {
        [Theory]
        [InlineData("0", 0)]
        [InlineData("0.00", 0)]
        [InlineData("1", 1)]
        [InlineData("1.0", 1)]
        [InlineData("1.00", 1.00)]
        [InlineData("1.001", 1.001)]
        [InlineData("10.001", 10.001)]
        internal void ToDecimal_WithVariousValidInputs_ReturnsExpectedDecimal(string input, decimal expected)
        {
            // Arrange
            // Act
            var result = Parser.ToDecimal(input);

            // Assert
            Assert.Equal(expected, result);
            Assert.Equal(input, result.ToString(CultureInfo.InvariantCulture));
        }
    }
}
