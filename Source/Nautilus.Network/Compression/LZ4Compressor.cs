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
using K4os.Compression.LZ4;
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
            // var target = new byte[LZ4Codec.MaximumOutputSize(source.Length)];
            //
            // LZ4Codec.Encode(source, target);
            //
            // var i = target.Length - 1;
            // while (target[i] == 0)
            // {
            //     --i;
            // }
            //
            // var temp = new byte[i + 1];
            // Array.Copy(target, temp, i + 1);
            //
            // return temp;
            return LZ4Pickler.Pickle(source);
        }

        /// <inheritdoc />
        public byte[] Decompress(byte[] source)
        {
            // var target = new byte[12];
            //
            // LZ4Codec.Decode(source, target);
            //
            // return target;
            return LZ4Pickler.Unpickle(source);
        }

        /// <summary>
        /// The d.
        /// </summary>
        /// <param name="source">source.</param>
        /// <returns>ret.</returns>
        public byte[] CompressFrame(byte[] source)
        {
            var stream = new MemoryStream(source);
            LZ4Stream.Encode(stream);

            return stream.ToArray();
        }

        /// <summary>
        /// The pp.
        /// </summary>
        /// <param name="source">source.</param>
        /// <returns>ret.</returns>
        public byte[] DecompressFrame(byte[] source)
        {
            var stream = new MemoryStream(source);
            LZ4Stream.Decode(stream, leaveOpen: false);

            return stream.ToArray();
        }
    }
}
