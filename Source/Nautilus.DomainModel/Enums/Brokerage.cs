//--------------------------------------------------------------------------------------------------
// <copyright file="Brokerage.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The brokerage enumeration.
    /// </summary>
    public enum Brokerage
    {
        /// <summary>
        /// The simulated brokerage.
        /// </summary>
        Simulation,

        /// <summary>
        /// The FXCM brokerage.
        /// </summary>
        FXCM,

        /// <summary>
        /// The Dukascopy brokerage.
        /// </summary>
        DUKASCOPY,

        /// <summary>
        /// The Interactive Brokers brokerage.
        /// </summary>
        IB,

        /// <summary>
        /// The LMAX exchange.
        /// </summary>
        LMAX,
    }
}
