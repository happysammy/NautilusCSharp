//--------------------------------------------------------------------------------------------------
// <copyright file="AccountType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// Represents the type of account.
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// The simulated account type.
        /// </summary>
        SIMULATED = 0,

        /// <summary>
        /// The demo account type.
        /// </summary>
        DEMO = 1,

        /// <summary>
        /// The real account type.
        /// </summary>
        REAL = 2,
    }
}
