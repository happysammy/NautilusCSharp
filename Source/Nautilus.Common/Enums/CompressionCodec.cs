//--------------------------------------------------------------------------------------------------
// <copyright file="CompressionCodec.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Enums
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents a data compression codec.
    /// </summary>
    public enum CompressionCodec
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// There is no compression specified.
        /// </summary>
        None = 1,

        /// <summary>
        /// The Snappy compression algorithm.
        /// </summary>
        Snappy = 2,
    }
}
