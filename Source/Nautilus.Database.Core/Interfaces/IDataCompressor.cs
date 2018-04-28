//--------------------------------------------------------------
// <copyright file="IDataCompressor.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Database.Core.Interfaces
{
    /// <summary>
    /// Abstract adapter for data compression of <see cref="byte"/> arrays.
    /// </summary>
    public interface IDataCompressor
    {
        /// <summary>
        /// Returns a compressed <see cref="byte"/> array from the given UTF8 <see cref="string"/>.
        /// </summary>
        /// <param name="stringToCompress">The string to compress.</param>
        /// <returns>A compressed <see cref="byte"/> array.</returns>
        byte[] Write(string stringToCompress);

        /// <summary>
        /// Returns a decompressed <see cref="string"/> from the given UTF8 <see cref="byte"/> array.
        /// </summary>
        /// <param name="bytesToDecompress">The bytes to decompress.</param>
        /// <returns>A decompressed <see cref="string"/>.</returns>
        string Read(byte[] bytesToDecompress);
    }
}