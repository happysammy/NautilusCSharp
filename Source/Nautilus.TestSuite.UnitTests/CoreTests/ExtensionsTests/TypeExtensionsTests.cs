//--------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensionsTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.ExtensionsTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using Nautilus.Messaging;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class TypeExtensionsTests
    {
        [Fact]
        internal void NameFormatted_FromReferenceType_ReturnsExpectedName()
        {
            // Arrange
            // Act
            var result = typeof(Console).NameFormatted();

            // Assert
            Assert.Equal("Console", result);
        }

        [Fact]
        internal void NameFormatted_FromSingleGenericType_ReturnsExpectedName()
        {
            // Arrange
            // Act
            var result1 = typeof(Envelope<Message>).Name;
            var result2 = typeof(Envelope<Message>).NameFormatted();

            // Assert
            Assert.Equal("Envelope`1", result1);
            Assert.Equal("Envelope<Message>", result2);
        }

        [Fact]
        internal void NameFormatted_FromMultipleGenericType_ReturnsExpectedName()
        {
            // Arrange
            // Act
            var result1 = typeof(Func<int, int, string>).Name;
            var result2 = typeof(Func<int, int, string>).NameFormatted();

            // Assert
            Assert.Equal("Func`3", result1);
            Assert.Equal("Func<Int32,Int32,String>", result2);
        }
    }
}
