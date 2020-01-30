//--------------------------------------------------------------------------------------------------
// <copyright file="IdUniqueness.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// The uniqueness context of an identifier. An identifier is unique if there are no duplicates
    /// of its value within the context.
    /// </summary>
    public enum IdUniqueness
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The identifier must be unique at the strategy level.
        /// </summary>
        Strategy = 1,

        /// <summary>
        /// The identifier must be unique at the trader level.
        /// </summary>
        Trader = 2,

        /// <summary>
        /// The identifier must be unique at the fund/team level.
        /// </summary>
        Fund = 3,

        /// <summary>
        /// The identifier must be unique at the global level.
        /// </summary>
        Global = 4,
    }
}
