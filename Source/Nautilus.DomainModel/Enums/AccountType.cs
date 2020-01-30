//--------------------------------------------------------------------------------------------------
// <copyright file="AccountType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
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
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The simulated account type.
        /// </summary>
        Simulated = 1,

        /// <summary>
        /// The demo account type.
        /// </summary>
        Demo = 2,

        /// <summary>
        /// The real account type.
        /// </summary>
        Real = 3,
    }
}
