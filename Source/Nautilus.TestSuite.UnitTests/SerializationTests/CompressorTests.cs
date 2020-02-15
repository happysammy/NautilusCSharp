// -------------------------------------------------------------------------------------------------
// <copyright file="CompressorTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.SerializationTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Nautilus.Serialization.Compressors;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class CompressorTests
    {
        [Fact]
        internal void LZ4Compressor_CanCompressAndDecompress()
        {
            // Arrange
            var message = "Hello world!";
            var compressor = new LZ4Compressor();

            // Act
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var compressed = compressor.Compress(messageBytes);
            var decompressed = compressor.Decompress(compressed);

            // Assert
            Assert.Equal(message, Encoding.UTF8.GetString(decompressed));
        }
    }
}
