// -------------------------------------------------------------------------------------------------
// <copyright file="SnappyCompressorTests.cs" company="Nautech Systems Pty Ltd">
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
    public sealed class SnappyCompressorTests : TestBase
    {
        public SnappyCompressorTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        internal void SnappyCompressor_CanCompressAndDecompress()
        {
            // Arrange
            var message = "hello world!";
            var compressor = new SnappyCompressor();

            // Act
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var compressed = compressor.Compress(messageBytes);
            var decompressed = compressor.Decompress(compressed);

            // Assert
            Assert.Equal(message, Encoding.UTF8.GetString(decompressed));
            this.Output.WriteLine(BitConverter.ToString(compressed));
        }

        [Fact]
        internal void SnappyCompressor_DecompressFromPython()
        {
            // Arrange
            var message = "hello world!";
            var expected = Encoding.UTF8.GetBytes(message);
            var compressor = new SnappyCompressor();
            var fromPython = Convert.FromBase64String("DCxoZWxsbyB3b3JsZCE=");

            // Act
            var decompressed = compressor.Decompress(fromPython);

            // Assert
            Assert.Equal(expected, decompressed);
        }
    }
}
