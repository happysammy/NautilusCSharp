//--------------------------------------------------------------------------------------------------
// <copyright file="MarketPosition.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
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
        /// An unknown market position.
        /// </summary>
        [InvalidValue]
        Unknown = -1,

        /// <summary>
        /// A flat market position.
        /// </summary>
        Flat = 0,

        /// <summary>
        /// A long market position.
        /// </summary>
        Long = 1,

        /// <summary>
        /// A short market position.
        /// </summary>
        Short = 2,
    }
}
