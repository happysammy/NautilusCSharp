//--------------------------------------------------------------------------------------------------
// <copyright file="ICompressor.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
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
