//--------------------------------------------------------------------------------------------------
// <copyright file="IEconomicEventCollector.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.Core.CQS;
    using Nautilus.DomainModel.Frames;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The economic event collector interface.
    /// </summary>
    public interface IEconomicEventCollector
    {
        /// <summary>
        /// Returns all <see cref="EconomicEvent"/> of the applicable currency symbol.
        /// </summary>
        /// <returns>A query result of <see cref="IReadOnlyCollection{EconomicNewsEvent}"/>.</returns>
        QueryResult<IReadOnlyCollection<EconomicEvent>> GetAllEvents();

        /// <summary>
        /// Returns a <see cref="BarDataFrame"/> of the bars data from and including the given
        /// <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="fromDateTime">The from date time.</param>
        /// <returns>A query result of <see cref="IReadOnlyCollection{EconomicNewsEvent}"/>.</returns>
        QueryResult<IReadOnlyCollection<EconomicEvent>> GetEvents(ZonedDateTime fromDateTime);
    }
}
