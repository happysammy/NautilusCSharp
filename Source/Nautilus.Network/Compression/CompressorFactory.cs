// -------------------------------------------------------------------------------------------------
// <copyright file="CompressorFactory.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.Common.Enums;
using Nautilus.Common.Interfaces;
using Nautilus.Core.Correctness;

namespace Nautilus.Network.Compression
{
    /// <summary>
    /// Provides a factory to create <see cref="ICompressor"/>s.
    /// </summary>
    public static class CompressorFactory
    {
        /// <summary>
        /// Creates and returns a <see cref="ICompressor"/> for the given <see cref="CompressionCodec"/>.
        /// </summary>
        /// <param name="codec">The compression codec for the compressor.</param>
        /// <returns>The messaging adapter.</returns>
        public static ICompressor Create(CompressionCodec codec)
        {
            switch (codec)
            {
                case CompressionCodec.None:
                    return new BypassCompressor();
                case CompressionCodec.Snappy:
                    return new SnappyCompressor();
                case CompressionCodec.Undefined:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(codec, nameof(codec));
            }
        }
    }
}
