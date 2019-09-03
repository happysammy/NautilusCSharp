//--------------------------------------------------------------------------------------------------
// <copyright file="Uniqueness.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The uniqueness level of an identifier. An identifier is unique if there are no duplicates
    /// of its value.
    /// </summary>
    public enum Uniqueness
    {
        /// <summary>
        /// The identifier must be unique at the strategy level.
        /// </summary>
        Strategy = 0,

        /// <summary>
        /// The identifier must be unique at the trader level.
        /// </summary>
        Trader = 1,

        /// <summary>
        /// The identifier must be unique at the fund/team level.
        /// </summary>
        Fund = 2,

        /// <summary>
        /// The identifier must be unique at the global level.
        /// </summary>
        Global = 3,
    }
}
