//--------------------------------------------------------------------------------------------------
// <copyright file="ITickProcessor.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Interfaces
{
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides an interface to the tick data processor for the <see cref="Nautilus"/> system.
    /// </summary>
    public interface ITickProcessor
    {
        /// <summary>
        /// Creates a new validated <see cref="Tick"/> and sends it to the tick publisher and bar
        /// data aggregation controller for the system.
        /// </summary>
        /// <param name="symbol">The tick symbol.</param>
        /// <param name="venue">The tick exchange.</param>
        /// <param name="bid">The tick bid price.</param>
        /// <param name="ask">The tick ask price.</param>
        /// <param name="decimals">The decimal precision of the tick prices.</param>
        /// <param name="timestamp">The tick timestamp.</param>
        void OnTick(
            string symbol,
            Venue venue,
            decimal bid,
            decimal ask,
            int decimals,
            ZonedDateTime timestamp);
    }
}
