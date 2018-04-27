// -------------------------------------------------------------------------------------------------
// <copyright file="Exchange.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The exchange label.
    /// </summary>
    public enum Exchange
    {
        /// <summary>
        /// The simulated exchange.
        /// </summary>
        Simulation,

        /// <summary>
        /// The FXCM venue.
        /// </summary>
        FXCM,

        /// <summary>
        /// The LMAX venue.
        /// </summary>
        LMAX,

        /// <summary>
        /// The New York Stock Exchange venue.
        /// </summary>
        NYSE,

        /// <summary>
        /// The Chicago Mercantile Exchange venue.
        /// </summary>
        CME,

        /// <summary>
        /// The ARCA venue.
        /// </summary>
        ARCA,

        /// <summary>
        /// The GLOBEX venue.
        /// </summary>
        GLOBEX
    }
}