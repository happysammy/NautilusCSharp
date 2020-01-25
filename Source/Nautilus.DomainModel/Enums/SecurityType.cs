//--------------------------------------------------------------------------------------------------
// <copyright file="SecurityType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// The type of security.
    /// </summary>
    public enum SecurityType
    {
        /// <summary>
        /// The security type is unknown (invalid value).
        /// </summary>
        [InvalidValue]
        UNKNOWN = 0,

        /// <summary>
        /// The foreign exchange security type.
        /// </summary>
        FOREX = 1,

        /// <summary>
        /// The bond security type.
        /// </summary>
        BOND = 2,

        /// <summary>
        /// The equity security type.
        /// </summary>
        EQUITY = 3,

        /// <summary>
        /// The futures security type.
        /// </summary>
        FUTURE = 4,

        /// <summary>
        /// The contract for difference security type.
        /// </summary>
        CFD = 5,

        /// <summary>
        /// The option security type.
        /// </summary>
        OPTION = 6,

        /// <summary>
        /// The crypto security type.
        /// </summary>
        CRYPTO = 7,
    }
}
