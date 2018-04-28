//--------------------------------------------------------------
// <copyright file="EconomicNewsEvent.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

using NautechSystems.CSharp;
using NautechSystems.CSharp.Annotations;
using NautechSystems.CSharp.Validation;
using NautilusDB.Core.Enums;
using NodaTime;

namespace Nautilus.DomainModel.Entities
{
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// Represents an economic news event which affects financial markets.
    /// </summary>
    [Immutable]
    public struct EconomicEvent
    {
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
            Debug.NotNull(title, nameof(title));
            Debug.NotDefault(country, nameof(country));
            Debug.NotDefault(currency, nameof(currency));
            Debug.NotDefault(impact, nameof(impact));
            Debug.DecimalNotOutOfRange(actual, nameof(impact), decimal.Zero, decimal.MaxValue);
            Debug.DecimalNotOutOfRange(consensus, nameof(consensus), decimal.Zero, decimal.MaxValue);
            Debug.DecimalNotOutOfRange(previous, nameof(previous), decimal.Zero, decimal.MaxValue);

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
        public ZonedDateTime Time { get; private set; }

        /// <summary>
        /// Gets the economic news events title/name.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the economic news events country effected.
        /// </summary>
        public Country Country { get; private set; }

        /// <summary>
        /// Gets the economic news events currency effected.
        /// </summary>
        public CurrencyCode Currency { get; private set; }

        /// <summary>
        /// Gets the economic news events expected impact on the market.
        /// </summary>
        public NewsImpact Impact { get; private set; }

        /// <summary>
        /// Gets the economic news events actual figure.
        /// </summary>
        public Option<decimal> Actual { get; private set; }

        /// <summary>
        /// Gets the economic news events consensus figure.
        /// </summary>
        public Option<decimal> Consensus { get; private set; }

        /// <summary>
        /// Gets the economic news events previous figure.
        /// </summary>
        public Option<decimal> Previous { get; private set; }

        /// <summary>
        /// Gets a value indication whether this economic news event is a speech or meeting.
        /// </summary>
        public bool IsSpeechOrMeeting => this.Previous.HasNoValue;
    }
}
