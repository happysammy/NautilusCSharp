//--------------------------------------------------------------
// <copyright file="MarketPosition.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using Nautilus.DomainModel.Aggregates;

    /// <summary>
    /// The <see cref="MarketPosition"/> enumeration. Represents the relative market position of
    /// a <see cref="Position"/>, <see cref="TradeUnit"/> and or <see cref="Trade"/>.
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
        Short
    }
}