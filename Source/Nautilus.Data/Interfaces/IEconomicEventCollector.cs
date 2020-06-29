//--------------------------------------------------------------------------------------------------
// <copyright file="IEconomicEventCollector.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
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
