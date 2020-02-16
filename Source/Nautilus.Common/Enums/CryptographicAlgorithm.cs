//--------------------------------------------------------------------------------------------------
// <copyright file="CryptographicAlgorithm.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Enums
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents a cryptographic algorithm specification.
    /// </summary>
    public enum CryptographicAlgorithm
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// There is encryption specified.
        /// </summary>
        None = 1,

        /// <summary>
        /// The Curve25519 elliptic curve algorithm.
        /// </summary>
        Curve = 2,
    }
}
