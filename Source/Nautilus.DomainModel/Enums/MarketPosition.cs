//--------------------------------------------------------------------------------------------------
// <copyright file="MarketPosition.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents relative market position.
    /// </summary>
    public enum MarketPosition
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The market position is flat.
        /// </summary>
        Flat = 1,

        /// <summary>
        /// The market position is long.
        /// </summary>
        Long = 2,

        /// <summary>
        /// The market position is short.
        /// </summary>
        Short = 3,
    }
}
