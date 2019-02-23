//--------------------------------------------------------------------------------------------------
// <copyright file="MarketPosition.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using Nautilus.DomainModel.Aggregates;

    /// <summary>
    /// Represents relative market position.
    /// </summary>
    public enum MarketPosition
    {
        /// <summary>
        /// An unknown market position.
        /// </summary>
        Unknown,

        /// <summary>
        /// A flat market position.
        /// </summary>
        Flat,

        /// <summary>
        /// A long market position.
        /// </summary>
        Long,

        /// <summary>
        /// A short market position.
        /// </summary>
        Short,
    }
}
