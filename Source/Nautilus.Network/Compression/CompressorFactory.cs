// -------------------------------------------------------------------------------------------------
// <copyright file="CompressorFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Network.Compression
{
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;

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
                    return new CompressorBypass();
                case CompressionCodec.Snappy:
                    return new SnappyCompressor();
                case CompressionCodec.Undefined:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(codec, nameof(@codec));
            }
        }
    }
}
