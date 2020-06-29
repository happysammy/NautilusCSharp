//--------------------------------------------------------------------------------------------------
// <copyright file="ObjectCacheTests.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.TestSuite.UnitTests.CommonTests.ComponentryTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Componentry;
    using Nautilus.DomainModel.Identifiers;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class ObjectCacheTests
    {
        [Fact]
        internal void Get_WhenNothingCached_ReturnsNewObject()
        {
            // Arrange
            var cache = new ObjectCache<string, Symbol>(Symbol.FromString);

            // Act
            var result = cache.Get("AUDUSD.FXCM");

            // Assert
            Assert.Equal(new Symbol("AUDUSD", "FXCM"), result);
            Assert.Single(cache.Keys);
        }

        [Fact]
        internal void Get_WhenObjectAlreadyCached_ReturnsCachedObject()
        {
            // Arrange
            var cache = new ObjectCache<string, Symbol>(Symbol.FromString);
            cache.Get("AUDUSD.FXCM");

            // Act
            var result = cache.Get("AUDUSD.FXCM");

            // Assert
            Assert.Equal(new Symbol("AUDUSD", "FXCM"), result);
            Assert.Single(cache.Keys);
        }
    }
}
