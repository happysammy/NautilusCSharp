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
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Identifiers;

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
        /// <param name="retentionTimeTicksDays">The rolling retention time for ticks in days.</param>
        /// <param name="retentionTimeBarsDays">The rolling retention time for time bar structures in days.</param>
        public DataConfiguration(
            IReadOnlyCollection<Symbol> subscribingSymbols,
            int retentionTimeTicksDays,
            Dictionary<BarStructure, int> retentionTimeBarsDays)
        {
            this.SubscribingSymbols = subscribingSymbols;
            this.RetentionTimeTicksDays = retentionTimeTicksDays;
            this.RetentionTimeBarsDays = retentionTimeBarsDays;
        }

        /// <summary>
        /// Gets the subscribing symbols.
        /// </summary>
        public IReadOnlyCollection<Symbol> SubscribingSymbols { get; }

        /// <summary>
        /// Gets the rolling retention time for ticks in days.
        /// </summary>
        public int RetentionTimeTicksDays { get; }

        /// <summary>
        /// Gets the rolling retention time for time bar structures in days.
        /// </summary>
        public IReadOnlyDictionary<BarStructure, int> RetentionTimeBarsDays { get; }
    }
}
