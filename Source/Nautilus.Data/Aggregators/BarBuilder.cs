//--------------------------------------------------------------------------------------------------
// <copyright file="BarBuilder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Aggregators
{
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides a builder for creating <see cref="Bar"/> objects.
    /// </summary>
    public sealed class BarBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarBuilder"/> class.
        /// </summary>
        /// <param name="open">The open price for the bar.</param>
        public BarBuilder(Price open)
        {
            this.Open = open;
            this.High = open;
            this.Low = open;
            this.Close = open;
            this.Volume += 1;
        }

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
        /// Updates the bar builder with the given quote price.
        /// </summary>
        /// <param name="quote">The quote price.</param>
        public void Update(Price quote)
        {
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
        /// Creates and returns a new <see cref="Bar"/> based on the values held by the builder.
        /// </summary>
        /// <param name="closeTime">The close time of the bar.</param>
        /// <returns>A <see cref="Bar"/>.</returns>
        public Bar Build(ZonedDateTime closeTime)
        {
            Debug.NotDefault(closeTime, nameof(closeTime));

            return new Bar(
                this.Open,
                this.High,
                this.Low,
                this.Close,
                Quantity.Create(this.Volume),
                closeTime);
        }
    }
}
