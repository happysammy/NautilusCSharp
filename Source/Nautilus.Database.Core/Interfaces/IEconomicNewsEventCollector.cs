//--------------------------------------------------------------
// <copyright file="IEconomicNewsEventCollector.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Database.Core.Interfaces
{
    using System.Collections.Generic;
    using NautechSystems.CSharp.CQS;
    using Nautilus.Database.Core.Types;
    using NodaTime;
    using Nautilus.DomainModel.Entities;

    public interface IEconomicNewsEventCollector
    {
        /// <summary>
        /// Returns all <see cref="EconomicEvent"/> of the applicable currency symbol.
        /// </summary>
        /// <returns>A query result of <see cref="IReadOnlyCollection{EconomicNewsEvent}"/>.</returns>
        QueryResult<IReadOnlyCollection<EconomicEvent>> GetAllEvents();

        /// <summary>
        /// Returns a <see cref="MarketDataFrame"/> of the bars data from and including the given
        /// <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="fromDateTime">The from date time.</param>
        /// <returns>A query result of <see cref="IReadOnlyCollection{EconomicNewsEvent}"/>.</returns>
        QueryResult<IReadOnlyCollection<EconomicEvent>> GetEvents(ZonedDateTime fromDateTime);
    }
}
