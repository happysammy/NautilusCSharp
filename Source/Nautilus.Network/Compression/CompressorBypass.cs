// -------------------------------------------------------------------------------------------------
// <copyright file="CompressorBypass.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Compression
{
    using Nautilus.Common.Interfaces;

    /// <inheritdoc />
    public sealed class CompressorBypass : ICompressor
    {
        /// <inheritdoc/>
        public byte[] Compress(byte[] source) => source;

        /// <inheritdoc/>
        public byte[] Decompress(byte[] source) => source;
    }
}
