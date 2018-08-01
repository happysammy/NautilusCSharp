//--------------------------------------------------------------------------------------------------
// <copyright file="LZ4DataCompressorTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.CompressionTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Compression;
    using Xunit;
    using Xunit.Abstractions;
    using Nautilus.Data.Interfaces;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    // ReSharper disable once InconsistentNaming
    public class LZ4DataCompressorTests
    {
        private readonly ITestOutputHelper output;
        private readonly IDataCompressor compressor;

        public LZ4DataCompressorTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            this.compressor = new LZ4DataCompressor(true);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("12 3")]
        [InlineData("abc")]
        [InlineData("ab c 1")]
        [InlineData("1970-01-01T00:00:00.000,1.00000,1.00000,1.00000,1.00000,5000000|")]
        internal void Write_WithVariousStrings_ReturnsExpectedString(string stringToCompress)
        {
            // Arrange
            // Act
            var result = this.compressor.Write(stringToCompress);

            // Assert
            Assert.Equal(typeof(byte[]), result.GetType());
            Assert.Equal(stringToCompress, this.compressor.Read(result));
        }

        [Theory]
        [InlineData("123")]
        [InlineData("12 3")]
        [InlineData("abc")]
        [InlineData("ab c 1")]
        [InlineData("1970-01-01T00:00:00.000,1.00000,1.00000,1.00000,1.00000,5000000|")]
        internal void Read_WithVariousStrings_ReturnsExpectedString(string stringToDecompress)
        {
            // Arrange
            var compressedString = this.compressor.Write(stringToDecompress);

            // Act
            var result = this.compressor.Read(compressedString);

            // Assert
            Assert.Equal(typeof(string), result.GetType());
            Assert.Equal(stringToDecompress, result);
        }
    }
}
