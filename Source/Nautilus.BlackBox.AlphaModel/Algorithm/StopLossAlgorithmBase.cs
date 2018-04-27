// -------------------------------------------------------------------------------------------------
// <copyright file="StopLossAlgorithmBase.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Algorithm
{
    using System;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The abstract <see cref="StopLossAlgorithmBase"/> class. The base class for all stop loss
    /// algorithms, inherits from <see cref="AlgorithmBase"/>.
    /// </summary>
    public abstract class StopLossAlgorithmBase : AlgorithmBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StopLossAlgorithmBase"/> class.
        /// </summary>
        /// <param name="algorithmLabel">The algorithm label.</param>
        /// <param name="tradeProfile">The algorithm trade profile.</param>
        /// <param name="instrument">The algorithm instrument.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        protected StopLossAlgorithmBase(
            Label algorithmLabel,
            TradeProfile tradeProfile,
            Instrument instrument)
            : base(algorithmLabel, tradeProfile, instrument)
        {
            Validate.NotNull(algorithmLabel, nameof(algorithmLabel));
            Validate.NotNull(tradeProfile, nameof(tradeProfile));
            Validate.NotNull(instrument, nameof(instrument));
        }

        /// <summary>
        /// Calculates and returns the buy side stop-loss <see cref="Price"/>.
        /// </summary>
        /// <param name="entryPrice">The entry price.</param>
        /// <param name="stopLossPrice">The stop-loss price.</param>
        /// <returns>A <see cref="Price"/>.</returns>    
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>   
        protected Price CalculateFinalStopLossBuy(
            Price entryPrice,
            Price stopLossPrice)
        {
            Validate.NotNull(entryPrice, nameof(entryPrice));
            Validate.NotNull(stopLossPrice, nameof(stopLossPrice));
            Validate.True(stopLossPrice < entryPrice, nameof(entryPrice));

            var stopLossMinimumSizeThreshold =
                Math.Ceiling(this.Instrument.TargetDirectSpread * this.TradeProfile.MinStoplossDirectSpreadMultiple) * this.TickSize;

            if (Math.Abs(entryPrice - stopLossPrice) < stopLossMinimumSizeThreshold)
            {
                stopLossPrice = Price.Create(entryPrice - stopLossMinimumSizeThreshold, this.TickSize);
            }

            Debug.NotNull(entryPrice, nameof(entryPrice));
            Debug.NotNull(stopLossPrice, nameof(stopLossPrice));
            Debug.True(stopLossPrice < entryPrice, nameof(stopLossPrice));

            return stopLossPrice > this.BestStopLong
                ? this.BestStopLong
                : stopLossPrice;
        }

        /// <summary>
        /// Calculates and returns the sell side stop-loss <see cref="Price"/>.
        /// </summary>
        /// <param name="entryPrice">The entry price.</param>
        /// <param name="stopLossPrice">The stop-loss price.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        protected Price CalculateFinalStopLossSell(
            Price entryPrice,
            Price stopLossPrice)
        {
            Validate.NotNull(entryPrice, nameof(entryPrice));
            Validate.NotNull(stopLossPrice, nameof(stopLossPrice));
            Validate.True(stopLossPrice > entryPrice, nameof(stopLossPrice));

            var stoplossMinimumSizeThreshold =
                Math.Ceiling(this.Instrument.TargetDirectSpread * this.TradeProfile.MinStoplossDirectSpreadMultiple) * this.TickSize;

            if (Math.Abs(entryPrice - stopLossPrice) < stoplossMinimumSizeThreshold)
            {
                stopLossPrice = Price.Create(entryPrice + stoplossMinimumSizeThreshold, this.TickSize);
            }

            Debug.NotNull(entryPrice, nameof(entryPrice));
            Debug.NotNull(stopLossPrice, nameof(stopLossPrice));
            Debug.True(stopLossPrice > entryPrice, nameof(stopLossPrice));

            return stopLossPrice < this.BestStopShort
                       ? this.BestStopLong
                       : stopLossPrice;
        }
    }
}