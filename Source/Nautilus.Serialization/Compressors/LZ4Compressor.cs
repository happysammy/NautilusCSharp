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
            var target = new byte[LZ4Codec.MaximumOutputSize(source.Length)];
            LZ4Codec.Encode(
                source,
                0,
                source.Length,
                target,
                0,
                target.Length);

            return target;
        }

        /// <inheritdoc />
        public byte[] Decompress(byte[] source)
        {
            // Unknown decompressed size (so as per Milos Krajewski source.Length * 255 to be safe)
            var target = new byte[source.Length * 255];
            var decoded = LZ4Codec.Decode(
                source,
                0,
                source.Length,
                target,
                0,
                target.Length);

            return target;
        }
    }
}
