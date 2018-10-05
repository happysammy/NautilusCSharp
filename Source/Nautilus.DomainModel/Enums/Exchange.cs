//--------------------------------------------------------------------------------------------------
// <copyright file="Exchange.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The venue an instrument is traded on.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Enums can be capitalized.")]
    public enum Venue
    {
        /// <summary>
        /// The simulated exchange venue (backtesting).
        /// </summary>
        Simulation = 0,

        /// <summary>
        /// The FXCM brokerage venue (OTC products) .
        /// </summary>
        FXCM = 1,

        /// <summary>
        /// The Dukascopy brokerage venue (OTC products).
        /// </summary>
        DUKASCOPY = 2,

        /// <summary>
        /// The GLOBEX exchange venue.
        /// </summary>
        GLOBEX = 3,

        /// <summary>
        /// The LMAX exchange venue.
        /// </summary>
        LMAX = 4,
    }
}
