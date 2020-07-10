//--------------------------------------------------------------------------------------------------
// <copyright file="DataConfiguration.cs" company="Nautech Systems Pty Ltd">
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

using System.Collections.Generic;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.Data.Configuration
{
    /// <summary>
    /// Provides the data configuration for a <see cref="DataService"/>.
    /// </summary>
    public sealed class DataConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataConfiguration"/> class.
        /// </summary>
        /// <param name="subscribingSymbols">The symbols to subscribe to.</param>
        /// <param name="barSpecifications">The bar specifications to build.</param>
        /// <param name="tickDataTrimTime">The tick data trim time.</param>
        /// <param name="barDataTrimTime">The bar data trim time.</param>
        /// <param name="tickDataTrimWindowDays">The tick data trim window in days.</param>
        /// <param name="barDataTrimWindowDays">The bar data trim window in days.</param>
        public DataConfiguration(
            IReadOnlyCollection<Symbol> subscribingSymbols,
            IReadOnlyCollection<BarSpecification> barSpecifications,
            LocalTime tickDataTrimTime,
            LocalTime barDataTrimTime,
            int tickDataTrimWindowDays,
            int barDataTrimWindowDays)
        {
            this.SubscribingSymbols = subscribingSymbols;
            this.BarSpecifications = barSpecifications;
            this.TickDataTrimTime = tickDataTrimTime;
            this.BarDataTrimTime = barDataTrimTime;
            this.TickDataTrimWindowDays = tickDataTrimWindowDays;
            this.BarDataTrimWindowDays = barDataTrimWindowDays;
        }

        /// <summary>
        /// Gets the subscribing symbols.
        /// </summary>
        public IReadOnlyCollection<Symbol> SubscribingSymbols { get; }

        /// <summary>
        /// Gets the configuration bar specifications.
        /// </summary>
        public IReadOnlyCollection<BarSpecification> BarSpecifications { get; }

        /// <summary>
        /// Gets the time to trim the tick data.
        /// </summary>
        public LocalTime TickDataTrimTime { get; }

        /// <summary>
        /// Gets the time to trim the bar data.
        /// </summary>
        public LocalTime BarDataTrimTime { get; }

        /// <summary>
        /// Gets the tick data rolling trim window in days.
        /// </summary>
        public int TickDataTrimWindowDays { get; }

        /// <summary>
        /// Gets the bar data rolling trim window in days.
        /// </summary>
        public int BarDataTrimWindowDays { get; }
    }
}
