// -------------------------------------------------------------------------------------------------
// <copyright file="LZ4Compressor.cs" company="Nautech Systems Pty Ltd">
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

using System.IO;
using K4os.Compression.LZ4.Streams;
using Nautilus.Common.Interfaces;

namespace Nautilus.Network.Compression
{
    /// <inheritdoc />
    // ReSharper disable once InconsistentNaming (correct name)
    public sealed class LZ4Compressor : ICompressor
    {
        /// <inheritdoc />
        public byte[] Compress(byte[] source)
        {
            var output = new MemoryStream();
            using (LZ4EncoderStream stream = LZ4Stream.Encode(output))
            {
                var reader = new MemoryStream(source);
                reader.CopyTo(stream);
                stream.Close();
                return output.ToArray();
            }
        }

        /// <inheritdoc />
        public byte[] Decompress(byte[] source)
        {
            using (LZ4DecoderStream stream = LZ4Stream.Decode(new MemoryStream(source)))
            {
                var output = new MemoryStream();
                stream.CopyTo(output);
                return output.ToArray();
            }
        }
    }
}
