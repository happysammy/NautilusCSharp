//--------------------------------------------------------------------------------------------------
// <copyright file="SecurityType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
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
        /// Foreign Exchange security type.
        /// </summary>
        FOREX = 0,

        /// <summary>
        /// Bond security type.
        /// </summary>
        BOND = 1,

        /// <summary>
        /// Equity security type.
        /// </summary>
        EQUITY = 2,

        /// <summary>
        /// Futures security type.
        /// </summary>
        FUTURE = 3,

        /// <summary>
        /// Contract For Difference security type.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        CFD = 4,

        /// <summary>
        /// Option security type.
        /// </summary>
        OPTION = 5,
    }
}
