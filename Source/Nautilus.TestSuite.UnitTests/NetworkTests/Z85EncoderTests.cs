//--------------------------------------------------------------------------------------------------
// <copyright file="Z85EncoderTests.cs" company="Nautech Systems Pty Ltd">
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Nautilus.Network.Encryption;
using Xunit;

namespace Nautilus.TestSuite.UnitTests.NetworkTests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class Z85EncoderTests
    {
        /// <summary>
        /// From the ZeroMQ RFC - https://rfc.zeromq.org/spec:32/Z85/
        /// As a test case, a frame containing these 8 bytes: 0x86 | 0x4F | 0xD2 | 0x6F | 0xB5 | 0x59 | 0xF7 | 0x5B
        /// SHALL encode as the following 10 characters: HelloWorld.
        /// </summary>
        [Fact]
        public void FromZ85String_HelloWorld_Success()
        {
            var encodedString = "HelloWorld";
            var expectedOutput = new byte[] { 0x86, 0x4F, 0xD2, 0x6F, 0xB5, 0x59, 0xF7, 0x5B };

            var output = Z85Encoder.FromZ85String(encodedString);

            Assert.Equal(expectedOutput, output);
        }

        [Fact]
        public void EncodeDecode_NoPaddingRequired_Success()
        {
            var input = "RiAZ3bax"; // 8 bytes - divisible by 4 with no remainder
            var inputBytes = System.Text.Encoding.Default.GetBytes(input);

            var outputBytes = Z85Encoder.FromZ85String(Z85Encoder.ToZ85String(inputBytes, false));

            var output = System.Text.Encoding.Default.GetString(outputBytes);
            Assert.Equal(input, output);
        }

        [Fact]
        public void EncodeDecode_PaddingRequired_Success()
        {
            var input = "HelloWorld"; // 10 bytes - NOT divisible by 4 with no remainder
            var inputBytes = System.Text.Encoding.Default.GetBytes(input);

            var outputBytes = Z85Encoder.FromZ85String(Z85Encoder.ToZ85String(inputBytes, true));

            var output = System.Text.Encoding.Default.GetString(outputBytes);
            Assert.Equal(input, output);
        }

        [Fact]
        public void ToZ85String_NoPaddingRequired_OutputSizeIsCorrect()
        {
            var input = "ABCD1234";
            var inputBytes = Encoding.Default.GetBytes(input);
            var expectedEncodeSize = 10; // 4 bytes become 5

            var output = Z85Encoder.ToZ85String(inputBytes, false);

            Assert.Equal(expectedEncodeSize, output.Length);
        }

        [Fact]
        public void ToZ85String_PaddingRequired_OutputSizeIsCorrect()
        {
            var input = "HelloWorld";
            var inputBytes = System.Text.Encoding.Default.GetBytes(input);
            var expectedEncodeSize = 15 + 1; // 4 bytes become 5, so with padding HelloWorld is 15 bytes encoded (+1 for padding value)

            var output = Z85Encoder.ToZ85String(inputBytes, true);

            Assert.Equal(expectedEncodeSize, output.Length);
        }

        /// <summary>
        /// From the ZeroMQ RFC - https://rfc.zeromq.org/spec:32/Z85/
        /// The binary frame SHALL have a length that is divisible by 4 with no remainder.
        /// </summary>
        [Fact]
        public void FromZ85String_OutputSizeIsCorrect()
        {
            var encodedText = "ABCDE12345";
            var expectedDecodeSize = 8; // 5 characters decode to 4 bytes

            var output = Z85Encoder.FromZ85String(encodedText);

            Assert.Equal(expectedDecodeSize, output.Length);
        }

        /// <summary>
        /// From the ZeroMQ RFC - https://rfc.zeromq.org/spec:32/Z85/
        /// The string frame SHALL have a length that is divisible by 5 with no remainder.
        /// </summary>
        [Fact]
        public void FromZ85String_InputSizeIncorrect_ThrowsException()
        {
            var encodedText = "WrongSize"; // Length - 1 not divisible by 5

            Assert.Throws<ArgumentException>(() => Z85Encoder.FromZ85String(encodedText));
        }

        [Fact]
        public void FromZ85String_PaddingCharacterNotDigit_ThrowsException()
        {
            var encodedText = "IllegalChrA"; // Padding char must be 0, 1, 2 or 3

            Assert.Throws<ArgumentException>(() => Z85Encoder.FromZ85String(encodedText));
        }

        [Fact]
        public void FromZ85String_PaddingCharacterIncorrectValue_ThrowsException()
        {
            var encodedText = "InvalidChr4"; // Padding char must be 0, 1, 2 or 3

            Assert.Throws<ArgumentException>(() => Z85Encoder.FromZ85String(encodedText));
        }
    }
}
