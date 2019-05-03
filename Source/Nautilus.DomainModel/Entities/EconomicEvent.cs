//--------------------------------------------------------------------------------------------------
// <copyright file="EconomicEvent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Entities
{
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Enums;
    using NodaTime;

    /// <summary>
    /// Represents an economic news event which affects financial markets.
    /// </summary>
    [Immutable]
    public struct EconomicEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EconomicEvent"/> struct.
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
            CurrencyCode currency,
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
        public CurrencyCode Currency { get; }

        /// <summary>
        /// Gets the economic news events expected impact on the market.
        /// </summary>
        public NewsImpact Impact { get; }

        /// <summary>
        /// Gets the economic news events actual figure.
        /// </summary>
        public OptionVal<decimal> Actual { get; }

        /// <summary>
        /// Gets the economic news events consensus figure.
        /// </summary>
        public OptionVal<decimal> Consensus { get; }

        /// <summary>
        /// Gets the economic news events previous figure.
        /// </summary>
        public OptionVal<decimal> Previous { get; }

        /// <summary>
        /// Gets a value indicating whether this economic event is a speech or meeting.
        /// </summary>
        public bool IsSpeechOrMeeting => this.Previous.HasNoValue;
    }
}
