//--------------------------------------------------------------------------------------------------
// <copyright file="Broker.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The brokerage enumeration.
    /// </summary>
    public enum Broker
    {
        /// <summary>
        /// The simulated brokerage.
        /// </summary>
        Simulation,

        /// <summary>
        /// The Interactive Brokers brokerage.
        /// </summary>
        InteractiveBrokers,

        /// <summary>
        /// The LMAX exchange.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        LMAX,

        /// <summary>
        /// The FXCM brokerage.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        FXCM
    }
}
