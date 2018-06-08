//--------------------------------------------------------------------------------------------------
// <copyright file="BarBuilder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Data.Market
{
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The sealed <see cref="BarBuilder"/> class.
    /// </summary>
    public sealed class BarBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarBuilder"/> class.
        /// </summary>
        /// <param name="quote">The bar builder opening quote.</param>
        /// <param name="startTime">
        /// The bar builder start time.
        /// </param>
        public BarBuilder(Price quote, ZonedDateTime startTime)
        {
            Validate.NotNull(quote, nameof(quote));
            Validate.NotDefault(startTime, nameof(startTime));

            this.StartTime = startTime;
            this.Open = quote;
            this.High = quote;
            this.Low = quote;
            this.Close = quote;
            this.Volume = 1;
            this.Timestamp = startTime;
        }

        /// <summary>
        /// Gets the bar builders start time.
        /// </summary>
        public ZonedDateTime StartTime { get; }

        /// <summary>
        /// Gets the bar builders open price.
        /// </summary>
        public Price Open { get; }

        /// <summary>
        /// Gets the bar builders high price.
        /// </summary>
        public Price High { get; private set; }

        /// <summary>
        /// Gets the bar builders low price.
        /// </summary>
        public Price Low { get; private set; }

        /// <summary>
        /// Gets the bar builders close price.
        /// </summary>
        public Price Close { get; private set; }

        /// <summary>
        /// Gets the bar builders volume.
        /// </summary>
        public int Volume { get; private set; }

        /// <summary>
        /// Gets the bar builders timestamp.
        /// </summary>
        public ZonedDateTime Timestamp { get; private set; }

        /// <summary>
        /// Updates the bar builder with the given quote price and timestamp.
        /// </summary>
        /// <param name="quote">The tick quote.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <exception cref="ValidationException">Throws if the price is null, or if the timestamp
        /// is the default value.</exception>
        public void OnQuote(Price quote, ZonedDateTime timestamp)
        {
            Validate.NotNull(quote, nameof(quote));
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.Timestamp = timestamp;

            if (quote > this.High)
            {
                this.High = quote;
            }

            if (quote < this.Low)
            {
                this.Low = quote;
            }

            this.Close = quote;

            this.Volume += 1;
        }

        /// <summary>
        /// Creates and returns a new <see cref="Bar"/> based on the values held by this
        /// <see cref="BarBuilder"/>.
        /// </summary>
        /// <param name="endTime">The end time of the bar.</param>
        /// <returns>A <see cref="Bar"/>.</returns>
        /// <exception cref="ValidationException">Throws if the end time is the default value.</exception>
        public Bar Build(ZonedDateTime endTime)
        {
            Validate.NotDefault(endTime, nameof(endTime));

           return new Bar(
                this.Open,
                this.High,
                this.Low,
                this.Close,
                Quantity.Create(this.Volume),
                endTime);
        }
    }
}
