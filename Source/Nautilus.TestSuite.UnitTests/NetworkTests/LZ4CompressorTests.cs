// -------------------------------------------------------------------------------------------------
// <copyright file="LZ4CompressorTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.NetworkTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Nautilus.Network.Compression;
    using Nautilus.TestSuite.TestKit.Fixtures;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Correct name")]
    public sealed class LZ4CompressorTests : TestBase
    {
        public LZ4CompressorTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        internal void LZ4Compressor_CanCompressAndDecompress()
        {
            // Arrange
            var message = "hello world!";
            var compressor = new LZ4Compressor();

            // Act
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var compressed = compressor.Compress(messageBytes);
            var decompressed = compressor.Decompress(compressed);

            // Assert
            Assert.Equal(message, Encoding.UTF8.GetString(decompressed));
            this.Output.WriteLine(BitConverter.ToString(compressed));
        }

        // [Fact]
        // internal void LZ4Compressor_DecompressFromPython()
        // {
        //     // Arrange
        //     var message = "hello world!";
        //     var expected = Encoding.UTF8.GetBytes(message);
        //     var compressor = new LZ4Compressor();
        //     var fromPython = Convert.FromBase64String("BCJNGGhADAAAAAAAAABdDAAAgGhlbGxvIHdvcmxkIQAAAAA=");
        //
        //     // Act
        //     var decompressed = compressor.Decompress(fromPython);
        //
        //     // Assert
        //     Assert.Equal(expected, decompressed);
        // }
    }
}
