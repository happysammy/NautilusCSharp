//--------------------------------------------------------------------------------------------------
// <copyright file="BarBuilder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Transactions;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The sealed <see cref="BarBuilder"/> class.
    /// </summary>
    public sealed class BarBuilder
    {
        /// <summary>
        /// Gets the bar builders open price.
        /// </summary>
        public Price Open { get; private set; }

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
        /// Gets a value indicating whether the bar builder is initialized.
        /// </summary>
        public bool IsInitialized => this.Open != null;

        /// <summary>
        /// Updates the bar builder with the given quote price and timestamp.
        /// </summary>
        /// <param name="quote">The tick quote.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <exception cref="ValidationException">Throws if the price is null, or if the timestamp
        /// is the default value.</exception>
        public void OnQuote(Price quote, ZonedDateTime timestamp)
        {
            Debug.NotNull(quote, nameof(quote));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.Timestamp = timestamp;

            if (this.Open is null)
            {
                this.Open = quote;
            }

            if (this.High is null)
            {
                this.High = quote;
            }

            if (this.Low is null)
            {
                this.Low = quote;
            }

            if (this.Close is null)
            {
                this.Close = quote;
            }

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
        /// <param name="closeTime">The end time of the bar.</param>
        /// <returns>A <see cref="Bar"/>.</returns>
        /// <exception cref="ValidationException">Throws if the end time is the default value.</exception>
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
