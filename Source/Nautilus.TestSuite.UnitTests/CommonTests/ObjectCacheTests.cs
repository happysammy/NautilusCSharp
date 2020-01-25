//--------------------------------------------------------------------------------------------------
// <copyright file="ObjectCacheTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CommonTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Componentry;
    using Nautilus.DomainModel.Identifiers;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class ObjectCacheTests
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
