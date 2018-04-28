//--------------------------------------------------------------
// <copyright file="OrderSide.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using Nautilus.DomainModel.Aggregates;

    /// <summary>
    /// The <see cref="OrderSide"/> enumeration. Represents the relative direction of an
    /// <see cref="Order"/>.
    /// </summary>
    public enum OrderSide
    {
        /// <summary>
        /// An undefined order side.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// A buy order side.
        /// </summary>
        Buy = 1,

        /// <summary>
        /// A sell order side.
        /// </summary>
        Sell = 2
    }
}