//--------------------------------------------------------------
// <copyright file="IEconomicNewsEventCollector.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

using System.Collections.Generic;
using NautechSystems.CSharp.CQS;
using NautilusDB.Core.Types;
using NodaTime;

namespace Nautilus.Database.Core.Interfaces
{
    public interface IEconomicNewsEventCollector
    {
        /// <summary>
        /// Returns all <see cref="EconomicNewsEvent"/> of the applicable currency symbol.
        /// </summary>
        /// <returns>A query result of <see cref="IReadOnlyCollection{EconomicNewsEvent}"/>.</returns>
        QueryResult<IReadOnlyCollection<EconomicNewsEvent>> GetAllEvents();

        /// <summary>
        /// Returns a <see cref="MarketDataFrame"/> of the bars data from and including the given
        /// <see cref="ZonedDateTime"/>.
        /// </summary>
        /// <param name="fromDateTime">The from date time.</param>
        /// <returns>A query result of <see cref="IReadOnlyCollection{EconomicNewsEvent}"/>.</returns>
        QueryResult<IReadOnlyCollection<EconomicNewsEvent>> GetEvents(ZonedDateTime fromDateTime);
    }
}