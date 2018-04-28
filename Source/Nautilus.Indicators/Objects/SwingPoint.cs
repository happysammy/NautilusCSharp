//--------------------------------------------------------------
// <copyright file="SwingPoint.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Indicators.Objects
{
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators.Enums;
    using NodaTime;

    /// <summary>
    /// The immutable <see cref="SwingPoint"/> class.
    /// </summary>
    [Immutable]
    public class SwingPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwingPoint"/> class.
        /// </summary>
        /// <param name="swingType">The swing type.</param>
        /// <param name="price">The swing price.</param>
        /// <param name="barIndex">The swing bar index.</param>
        /// /// <param name="timestamp">The swing timestamp.</param>
        public SwingPoint(SwingType swingType, Price price, int barIndex, ZonedDateTime timestamp)
        {
            Validate.NotNull(price, nameof(price));
            Validate.Int32NotOutOfRange(barIndex, nameof(barIndex), 0, int.MaxValue, RangeEndPoints.Exclusive);

            this.Type = swingType;
            this.Price = price;
            this.BarIndex = barIndex;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the swing type.
        /// </summary>
        public SwingType Type { get; }

        /// <summary>
        /// Gets the swing price.
        /// </summary>
        public Price Price { get; }

        /// <summary>
        /// Gets the swing bar index.
        /// </summary>
        public int BarIndex { get; }

        /// <summary>
        /// Gets the swing timestamp.
        /// </summary>
        public ZonedDateTime Timestamp { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="SwingPoint"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"Swing{this.Type}: SignalTimestamp={this.Timestamp} Price={this.Price} BarIndex={this.BarIndex}";
        }
    }
}