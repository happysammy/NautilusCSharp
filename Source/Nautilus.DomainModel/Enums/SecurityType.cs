//--------------------------------------------------------------------------------------------------
// <copyright file="SecurityType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The type of security.
    /// </summary>
    public enum SecurityType
    {
        /// <summary>
        /// The foreign exchange security type.
        /// </summary>
        FOREX = 0,

        /// <summary>
        /// The bond security type.
        /// </summary>
        BOND = 1,

        /// <summary>
        /// The equity security type.
        /// </summary>
        EQUITY = 2,

        /// <summary>
        /// The futures security type.
        /// </summary>
        FUTURE = 3,

        /// <summary>
        /// The contract For Difference security type.
        /// </summary>
        CFD = 4,

        /// <summary>
        /// The option security type.
        /// </summary>
        OPTION = 5,
    }
}
