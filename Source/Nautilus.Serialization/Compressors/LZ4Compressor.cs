// -------------------------------------------------------------------------------------------------
// <copyright file="LZ4Compressor.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.Compressors
{
    using K4os.Compression.LZ4;
    using Nautilus.Common.Interfaces;

    /// <inheritdoc />
    // ReSharper disable once InconsistentNaming (correct name)
    public sealed class LZ4Compressor : ICompressor
    {
        /// <inheritdoc />
        public byte[] Compress(byte[] source)
        {
            return LZ4Pickler.Pickle(source);
        }

        /// <inheritdoc />
        public byte[] Decompress(byte[] source)
        {
            return LZ4Pickler.Unpickle(source);
        }
    }
}
