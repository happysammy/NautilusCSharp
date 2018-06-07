// -------------------------------------------------------------------------------------------------
// <copyright file="DataCompressorFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Compression
{
    using System;
    using Nautilus.Core.Validation;
    using Nautilus.Database.Interfaces;

    /// <summary>
    /// Provides a factory for the creation of <see cref="IDataCompressor"/>(s).
    /// </summary>
    public static class DataCompressorFactory
    {
        /// <summary>
        /// Returns a new <see cref="IDataCompressor"/> from the given arguments.
        /// </summary>
        /// <param name="isCompression">The is compression on boolean flag.</param>
        /// <param name="compressionCodec">The compression codec.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        /// <exception cref="ArgumentException">Throws if the compression codec is not recognized.</exception>
        public static IDataCompressor Create(bool isCompression, string compressionCodec)
        {
            Validate.NotNull(compressionCodec, nameof(compressionCodec));

            switch (compressionCodec)
            {
                case "lz4":
                    return new LZ4DataCompressor(isCompression);

                default:
                    throw new ArgumentException(
                        $"The compression codec {compressionCodec} is not recognized "
                      + $"(can be 'lz4', 'snappy', or 'gzip').");
            }
        }
    }
}
