// -------------------------------------------------------------------------------------------------
// <copyright file="TDPoint.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Indicators.Objects
{
    using NautechSystems.CSharp.Annotations;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable <see cref="TDPoint"/> class.
    /// </summary>
    [Immutable]
    public class TDPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TDPoint"/> class.
        /// </summary>
        /// <param name="price">The point price.</param>
        /// <param name="barIndex">The point bar index.</param>
        /// /// <param name="timestamp">The point timestamp.</param>
        public TDPoint(Price price, int barIndex, ZonedDateTime timestamp)
        {
            this.Timestamp = timestamp;
            this.Price = price;
            this.BarIndex = barIndex;
        }

        /// <summary>
        /// Gets the point price.
        /// </summary>
        public Price Price { get; }

        /// <summary>
        /// Gets the point bar index.
        /// </summary>
        public int BarIndex { get; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        public ZonedDateTime Timestamp { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="TDPoint"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"TDPoint: SignalTimestamp={this.Timestamp} Price={this.Price}";
        }
    }
}