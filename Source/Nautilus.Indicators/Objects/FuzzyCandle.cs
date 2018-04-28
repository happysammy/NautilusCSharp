//--------------------------------------------------------------
// <copyright file="FuzzyCandle.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Indicators.Objects
{
    using NautechSystems.CSharp.Annotations;
    using Nautilus.Indicators.Enums;

    /// <summary>
    /// The immutable <see cref="FuzzyCandle"/> class.
    /// </summary>
    [Immutable]
    public class FuzzyCandle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FuzzyCandle"/> class.
        /// </summary>
        public FuzzyCandle()
        {
            this.Direction   = CandleDirection.None;
            this.Size        = CandleSize.None;
            this.Body        = CandleBody.None;
            this.UpperWick   = CandleWick.None;
            this.LowerWick   = CandleWick.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FuzzyCandle"/> class.
        /// </summary>
        /// <param name="direction">The candle direction.</param>
        /// <param name="size">The candle size.</param>
        /// <param name="body">The candle body.</param>
        /// <param name="upperWick">The candle upper wick.</param>
        /// <param name="lowerWick">The candle lower wick.</param>
        public FuzzyCandle(
            CandleDirection direction,
            CandleSize size,
            CandleBody body,
            CandleWick upperWick,
            CandleWick lowerWick)
        {
            this.Direction   = direction;
            this.Size        = size;
            this.Body        = body;
            this.UpperWick   = upperWick;
            this.LowerWick   = lowerWick;
        }

        /// <summary>
        /// Gets the candle direction.
        /// </summary>
        public CandleDirection Direction { get; }

        /// <summary>
        /// Gets the candle size.
        /// </summary>
        public CandleSize Size { get; }

        /// <summary>
        /// Gets the candle body.
        /// </summary>
        public CandleBody Body { get; }

        /// <summary>
        /// Gets the candles upper wick.
        /// </summary>
        public CandleWick UpperWick { get; }

        /// <summary>
        /// Gets the candles lower wick.
        /// </summary>
        public CandleWick LowerWick { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="FuzzyCandle"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return $"{nameof(FuzzyCandle)}: "
                + $"Direction={this.Direction} "
                + $"Size={this.Size} "
                + $"Body={this.Body} "
                + $"UpperWick={this.UpperWick} "
                + $"LowerWick={this.LowerWick}";
        }
    }
}