//--------------------------------------------------------------------------------------------------
// <copyright file="EconomicEvent.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.Core.Annotations;
using Nautilus.Core.Correctness;
using Nautilus.DomainModel.Enums;
using NodaTime;

namespace Nautilus.DomainModel.ValueObjects
{
    /// <summary>
    /// Represents an economic news event which affects financial markets.
    /// </summary>
    [Immutable]
    public sealed class EconomicEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EconomicEvent"/> class.
        /// </summary>
        /// <param name="time">The event time.</param>
        /// <param name="title">The event title.</param>
        /// <param name="country">The event country.</param>
        /// <param name="currency">The event currency.</param>
        /// <param name="impact">The event impact.</param>
        /// <param name="actual">The event actual figure.</param>
        /// <param name="consensus">The event consensus figure.</param>
        /// <param name="previous">The event previous figure.</param>
        public EconomicEvent(
            ZonedDateTime time,
            string title,
            Country country,
            Currency currency,
            NewsImpact impact,
            decimal actual,
            decimal consensus,
            decimal previous)
        {
            Debug.NotDefault(time, nameof(title));
            Debug.NotEmptyOrWhiteSpace(title, nameof(title));
            Debug.NotDefault(country, nameof(country));
            Debug.NotDefault(currency, nameof(currency));
            Debug.NotDefault(impact, nameof(impact));
            Debug.NotNegativeDecimal(actual, nameof(impact));
            Debug.NotNegativeDecimal(consensus, nameof(consensus));
            Debug.NotNegativeDecimal(previous, nameof(previous));

            this.Time = time;
            this.Title = title;
            this.Country = country;
            this.Currency = currency;
            this.Impact = impact;
            this.Actual = actual;
            this.Consensus = consensus;
            this.Previous = previous;
        }

        /// <summary>
        /// Gets the economic news events scheduled time.
        /// </summary>
        public ZonedDateTime Time { get; }

        /// <summary>
        /// Gets the economic news events title/name.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the economic news events country effected.
        /// </summary>
        public Country Country { get; }

        /// <summary>
        /// Gets the economic news events currency effected.
        /// </summary>
        public Currency Currency { get; }

        /// <summary>
        /// Gets the economic news events expected impact on the market.
        /// </summary>
        public NewsImpact Impact { get; }

        /// <summary>
        /// Gets the economic news events actual figure.
        /// </summary>
        public decimal? Actual { get; }

        /// <summary>
        /// Gets the economic news events consensus figure.
        /// </summary>
        public decimal? Consensus { get; }

        /// <summary>
        /// Gets the economic news events previous figure.
        /// </summary>
        public decimal? Previous { get; }

        /// <summary>
        /// Gets a value indicating whether this economic event is a speech or meeting.
        /// </summary>
        public bool IsSpeechOrMeeting => this.Previous is null;
    }
}
