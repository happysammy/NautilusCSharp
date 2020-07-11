// -------------------------------------------------------------------------------------------------
// <copyright file="LZ4CompressorTests.cs" company="Nautech Systems Pty Ltd">
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
// -------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Nautilus.Network.Compression;
using Nautilus.TestSuite.TestKit.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace Nautilus.TestSuite.UnitTests.NetworkTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class LZ4CompressorTests : TestBase
    {
        public LZ4CompressorTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        internal void LZ4Compressor_CanCompressAndDecompressBlock()
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
        // internal void LZ4Compressor_DecompressBlockFromPython()
        // {
        //     // Arrange
        //     var message = "hello world!";
        //     var expected = Encoding.UTF8.GetBytes(message);
        //     var compressor = new LZ4Compressor();
        //     var fromPython = Convert.FromBase64String("DAAAAMBoZWxsbyB3b3JsZCE=");
        //     this.Output.WriteLine(Encoding.UTF8.GetString(fromPython));
        //
        //     // Act
        //     var decompressed = compressor.Decompress(fromPython);
        //
        //     // Assert
        //     Assert.Equal(expected, decompressed);
        // }

        [Fact]
        internal void LZ4Compressor_CanCompressAndDecompressFrame()
        {
            // Arrange
            var message = "hello world!";
            var compressor = new LZ4Compressor();

            // Act
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var compressed = compressor.CompressFrame(messageBytes);
            var decompressed = compressor.DecompressFrame(compressed);

            // Assert
            Assert.Equal(message, Encoding.UTF8.GetString(decompressed));
            this.Output.WriteLine(BitConverter.ToString(compressed));
        }

        // [Fact]
        // internal void LZ4Compressor_DecompressFrameFromPython()
        // {
        //     // Arrange
        //     var message = "hello world!";
        //     var expected = Encoding.UTF8.GetBytes(message);
        //     var compressor = new LZ4Compressor();
        //     var fromPython = Convert.FromBase64String("BCJNGGhADAAAAAAAAABdDAAAgGhlbGxvIHdvcmxkIQAAAAA=");
        //     this.Output.WriteLine(Encoding.UTF8.GetString(fromPython));
        //
        //     // Act
        //     var decompressed = compressor.DecompressFrame(fromPython);
        //
        //     // Assert
        //     Assert.Equal(expected, decompressed);
        // }
    }
}
