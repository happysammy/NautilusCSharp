//--------------------------------------------------------------------------------------------------
// <copyright file="ExceptionFactoryTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests.CorrectnessTests
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Enums;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class ExceptionFactoryTests
    {
        [Fact]
        internal void InvalidSwitchArgument_WithObjectArgument_ReturnsExpectedException()
        {
            // Arrange
            var someArgument = "-1";

            // Act
            var exception = ExceptionFactory.InvalidSwitchArgument(someArgument, nameof(someArgument));

            // Assert
            Assert.Equal(typeof(ArgumentOutOfRangeException), exception.GetType());
            Assert.Equal(someArgument, exception.ActualValue);
            Assert.Equal("The value of argument 'someArgument' of type String was invalid out of range for this switch. (Parameter 'someArgument')\nActual value was -1.", exception.Message);
        }

        [Fact]
        internal void InvalidSwitchArgument_WithEnumArgument_ReturnsExpectedException()
        {
            // Arrange
            var someArgument = PriceType.Last;

            // Act
            var exception = ExceptionFactory.InvalidSwitchArgument(someArgument, nameof(someArgument));

            // Assert
            Assert.Equal(typeof(InvalidEnumArgumentException), exception.GetType());
            Assert.Equal("The value of argument 'someArgument' (4) is invalid for Enum type 'PriceType'. (Parameter 'someArgument')", exception.Message);
        }
    }
}
