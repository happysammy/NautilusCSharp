//--------------------------------------------------------------------------------------------------
// <copyright file="Brokerage.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The brokerage enumeration.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Align with Python enums.")]
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
