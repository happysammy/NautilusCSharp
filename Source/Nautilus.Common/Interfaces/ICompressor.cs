//--------------------------------------------------------------------------------------------------
// <copyright file="ICompressor.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Common.Interfaces
{
    /// <summary>
    /// Provides data compression and decompression.
    /// </summary>
    public interface ICompressor
    {
        /// <summary>
        /// Compress the data.
        /// </summary>
        /// <param name="source">The data source to compress.</param>
        /// <returns>The compressed data.</returns>
        byte[] Compress(byte[] source);

        /// <summary>
        /// Decompress the data.
        /// </summary>
        /// <param name="source">The data source to decompress.</param>
        /// <returns>The decompressed data.</returns>
        byte[] Decompress(byte[] source);
    }
}
