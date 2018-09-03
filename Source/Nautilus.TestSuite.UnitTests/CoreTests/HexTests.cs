//--------------------------------------------------------------------------------------------------
// <copyright file="HexTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CoreTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Nautilus.Core;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("ReSharper", "PossibleUnintendedReferenceComparison", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("ReSharper", "EqualExpressionComparison", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class HexTests
    {
        [Fact]
        internal void Test_can_convert_hex_string_to_string()
        {
            // Arrange
            var hex = "68656c6c6f20776f726c64";

            // Act
            var result = Encoding.UTF8.GetString(Hex.FromHexString(hex));

            // Assert
            Assert.Equal("hello world", result);
        }

        [Fact]
        internal void Test_can_convert_bytes_to_hex_string()
        {
            // Arrange
            var bytes = Encoding.UTF8.GetBytes("hello world");

            // Act
            var result = Hex.ToHexString(bytes);

            // Assert
            Assert.Equal("68656c6c6f20776f726c64", result);
        }

        [Fact]
        internal void Test_can_pretty_print_byte_array()
        {
            // Arrange
            var bytes = Encoding.UTF8.GetBytes("a");

            // Act
            var result = Hex.PrettyPrint(bytes);

            // Assert
            Assert.StartsWith("       0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F\n0000: 61", result);
        }
    }
}
