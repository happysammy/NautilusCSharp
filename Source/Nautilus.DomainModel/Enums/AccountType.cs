//--------------------------------------------------------------------------------------------------
// <copyright file="AccountType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents the type of account.
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// The account type is undefined (invalid value).
        /// </summary>
        [InvalidValue]
        UNKNOWN = 0,

        /// <summary>
        /// The simulated account type.
        /// </summary>
        SIMULATED = 1,

        /// <summary>
        /// The demo account type.
        /// </summary>
        DEMO = 2,

        /// <summary>
        /// The real account type.
        /// </summary>
        REAL = 3,
    }
}
