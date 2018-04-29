//--------------------------------------------------------------------------------------------------
// <copyright file="TrailingStopResponse.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Signal
{
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="TrailingStopResponse"/> class. Represents the calculated
    /// signal response from an <see cref="ITrailingStopAlgorithm"/>.
    /// </summary>
    [Immutable]
    public sealed class TrailingStopResponse : ITrailingStopResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrailingStopResponse"/> class.
        /// </summary>
        /// <param name="label">The trailing stop response label.</param>
        /// <param name="forMarketPosition">The trailing stop response applicable market position.</param>
        /// <param name="stopLossPrice">The trailing stop response stop-loss price.</param>
        /// <param name="forUnit">The trailing stop response applicable trade unit.</param>
        /// <param name="time">The trailing stop response time.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value, or if the for unit is negative.</exception>
        public TrailingStopResponse(
            Label label,
            MarketPosition forMarketPosition,
            Price stopLossPrice,
            int forUnit,
            ZonedDateTime time)
        {
            Validate.NotNull(label, nameof(label));
            Validate.NotDefault(forMarketPosition, nameof(forMarketPosition));
            Validate.NotDefault(forMarketPosition, nameof(forMarketPosition));
            Validate.NotNull(stopLossPrice, nameof(stopLossPrice));
            Validate.Int32NotOutOfRange(forUnit, nameof(forUnit), 0, int.MaxValue);

            this.Label = label;
            this.ForMarketPosition = forMarketPosition;
            this.ForUnit = forUnit;
            this.StopLossPrice = stopLossPrice;
            this.Time = time;
        }

        /// <summary>
        /// Gets the trailing stop responses label.
        /// </summary>
        public Label Label { get; }

        /// <summary>
        /// Gets the trailing stop responses applicable market position.
        /// </summary>
        public MarketPosition ForMarketPosition { get; }

        /// <summary>
        /// Gets the trailing stop responses applicable trade unit.
        /// </summary>
        public int ForUnit { get; }

        /// <summary>
        /// Gets the trailing stop responses stop-loss price.
        /// </summary>
        public Price StopLossPrice { get; }

        /// <summary>
        /// Gets the trailing stop responses time.
        /// </summary>
        public ZonedDateTime Time { get; }
    }
}
