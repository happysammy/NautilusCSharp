//--------------------------------------------------------------
// <copyright file="Bar.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using System.Collections.Generic;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="Bar"/> class. Represents a financial market trade bar.
    /// </summary>
    [Immutable]
    public sealed class Bar : ValueObject<Bar>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bar"/> class.
        /// </summary>
        /// <param name="open">The open price.</param>
        /// <param name="high">The high price.</param>
        /// <param name="low">The low price.</param>
        /// <param name="close">The close price.</param>
        /// <param name="volume">The trading volume.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public Bar(
            Price open,
            Price high,
            Price low,
            Price close,
            Quantity volume,
            ZonedDateTime timestamp)
        {
            Validate.NotNull(open, nameof(open));
            Validate.NotNull(high, nameof(high));
            Validate.NotNull(low, nameof(low));
            Validate.NotNull(close, nameof(close));
            Validate.NotNull(volume, nameof(volume));
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.Open = open;
            this.High = high;
            this.Low = low;
            this.Close = close;
            this.Volume = volume;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the bars open <see cref="Price"/>.
        /// </summary>
        public Price Open { get; }

        /// <summary>
        /// Gets the bars high <see cref="Price"/>.
        /// </summary>
        public Price High { get; }

        /// <summary>
        /// Gets the bars low <see cref="Price"/>.
        /// </summary>
        public Price Low { get; }

        /// <summary>
        /// Gets the bars close <see cref="Price"/>.
        /// </summary>
        public Price Close { get; }

        /// <summary>
        /// Gets the bars volume.
        /// </summary>
        public Quantity Volume { get; }

        /// <summary>
        /// Gets the bars timestamp.
        /// </summary>
        public ZonedDateTime Timestamp { get; }

        /// <summary>
        /// Returns a collection of objects to be included in equality checks.
        /// </summary>
        /// <returns>A collection of objects.</returns>
        protected override IEnumerable<object> GetMembersForEqualityCheck()
        {
            return new object[]
                       {
                           this.Open,
                           this.High,
                           this.Low,
                           this.Close,
                           this.Volume,
                           this.Timestamp
                       };
        }
    }
}
