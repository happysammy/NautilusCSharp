//--------------------------------------------------------------
// <copyright file="Broker.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The broker label.
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
        LMAX,

        /// <summary>
        /// The FXCM brokerage.
        /// </summary>
        FXCM
    }
}