//--------------------------------------------------------------------------------------------------
// <copyright file="SecurityType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// The type of security.
    /// </summary>.
    public enum SecurityType
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The foreign exchange security type.
        /// </summary>
        Forex = 1,

        /// <summary>
        /// The bond security type.
        /// </summary>
        Bond = 2,

        /// <summary>
        /// The equity security type.
        /// </summary>
        Equity = 3,

        /// <summary>
        /// The futures security type.
        /// </summary>
        Futures = 4,

        /// <summary>
        /// The contract for difference security type.
        /// </summary>
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Correct name")]
        CFD = 5,

        /// <summary>
        /// The option security type.
        /// </summary>
        Option = 6,

        /// <summary>
        /// The crypto security type.
        /// </summary>
        Crypto = 7,
    }
}
