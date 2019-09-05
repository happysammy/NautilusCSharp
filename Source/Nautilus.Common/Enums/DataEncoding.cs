//--------------------------------------------------------------------------------------------------
// <copyright file="DataEncoding.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Enums
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents a data encoding specification.
    /// </summary>
    public enum DataEncoding
    {
        /// <summary>
        /// The encoding is unknown (this is an invalid value).
        /// </summary>
        [InvalidValue]
        Unknown = 0,

        /// <summary>
        /// The UTF-8 encoding specification.
        /// </summary>
        Utf8 = 1,

        /// <summary>
        /// The BSON encoding specification.
        /// </summary>
        Bson = 2,

        /// <summary>
        /// The MsgPack encoding specification.
        /// </summary>
        MsgPack = 3,
    }
}
