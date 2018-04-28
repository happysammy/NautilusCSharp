//--------------------------------------------------------------
// <copyright file="MarketDataPoint.cs" company="Nautech Systems Pty Ltd.">
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
    /// The immutable <see cref="MarketDataPoint"/> class.
    /// </summary>
    [Immutable]
    public class MarketDataPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDataPoint"/> class.
        /// </summary>
        public MarketDataPoint()
        {
            // null
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDataPoint"/> class.
        /// </summary>
        /// <param name="price">The price.</param>
        /// <param name="volume">The volume.</param>
        /// <param name="swingType">The swing type.</param>
        /// <param name="swingLength">The swing length.</param>
        /// <param name="keltnerPosition">The keltner position.</param>
        /// <param name="momentum">The momentum.</param>
        /// <param name="timestamp">The timestamp.</param>
        public MarketDataPoint(
            Price price,
            int volume,
            SwingType swingType,
            decimal swingLength,
            decimal keltnerPosition,
            decimal momentum,
            ZonedDateTime timestamp)
        {
            Validate.NotNull(price, nameof(price));
            Validate.Int32NotOutOfRange(volume, nameof(volume), 0, int.MaxValue);
            Validate.DecimalNotOutOfRange(swingLength, nameof(swingLength), decimal.Zero, decimal.MaxValue, RangeEndPoints.UpperExclusive);
            Validate.DecimalNotOutOfRange(keltnerPosition, nameof(keltnerPosition), decimal.MinValue, decimal.MaxValue, RangeEndPoints.Exclusive);
            Validate.DecimalNotOutOfRange(momentum, nameof(momentum), decimal.MinValue, decimal.MaxValue, RangeEndPoints.Exclusive);

            this.Price = price;
            this.Volume = volume;
            this.SwingType = swingType;
            this.SwingLength = swingLength;
            this.KeltnerPosition = keltnerPosition;
            this.Momentum = momentum;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the price.
        /// </summary>
        public Price Price { get; }

        /// <summary>
        /// Gets the volume.
        /// </summary>
        public int Volume { get; }

        /// <summary>
        /// Gets the swing type.
        /// </summary>
        public SwingType SwingType { get; }

        /// <summary>
        /// Gets the swing length.
        /// </summary>
        public decimal SwingLength { get; }

        /// <summary>
        /// Gets the keltner position.
        /// </summary>
        public decimal KeltnerPosition { get; }

        /// <summary>
        /// Gets the momentum.
        /// </summary>
        public decimal Momentum { get; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        public ZonedDateTime Timestamp { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="MarketDataPoint"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"MarketDataPoint: SwingType={this.SwingType} SignalTimestamp={this.Timestamp} Price={this.Price} Volume={this.Volume} SwingLength={this.SwingLength} KeltnerPosition={this.KeltnerPosition} Momentum={this.Momentum}";
        }
    }
}