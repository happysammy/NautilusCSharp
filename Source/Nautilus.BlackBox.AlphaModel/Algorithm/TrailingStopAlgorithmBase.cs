//--------------------------------------------------------------------------------------------------
// <copyright file="TrailingStopAlgorithmBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Algorithm
{
    using Nautilus.Core;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.AlphaModel.Signal;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The <see cref="TrailingStopAlgorithmBase"/> class. The base class for all trailing stop
    /// algorithms, inherits from <see cref="AlgorithmBase"/>.
    /// </summary>
    public abstract class TrailingStopAlgorithmBase : AlgorithmBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrailingStopAlgorithmBase"/> class.
        /// </summary>
        /// <param name="algorithmLabel">The trailing-stop algorithm label.</param>
        /// <param name="tradeProfile">The trailing-stop algorithm trade profile.</param>
        /// <param name="instrument">The trailing-stop algorithm instrument.</param>
        /// <param name="forUnit">The trailing-stop algorithm applicable trade unit.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if the
        /// for unit is negative.</exception>
        protected TrailingStopAlgorithmBase(
            Label algorithmLabel,
            TradeProfile tradeProfile,
            Instrument instrument,
            int forUnit)
            : base(algorithmLabel, tradeProfile, instrument)
        {
            Validate.NotNull(algorithmLabel, nameof(algorithmLabel));
            Validate.NotNull(tradeProfile, nameof(tradeProfile));
            Validate.NotNull(instrument, nameof(instrument));
            Validate.Int32NotOutOfRange(forUnit, nameof(forUnit), 0, int.MaxValue);

            this.ForUnit = forUnit;
        }

        /// <summary>
        /// Gets the trailing-stop algorithms applicable trade unit number.
        /// </summary>
        protected int ForUnit { get; }

        /// <summary>
        /// Runs a calculation of the trailing stop algorithm for long positions, and returns a
        /// response (optional value).
        /// </summary>
        /// <param name="isSignal">The boolean flag indicating whether there is an exit signal.</param>
        /// <param name="stopLossPrice">The stop-loss price.</param>
        /// <returns>A <see cref="Option{ITrailingStopResponse}"/>.</returns>
        protected Option<ITrailingStopResponse> SignalResponseLong(bool isSignal, Price stopLossPrice)
        {
            Validate.NotNull(stopLossPrice, nameof(stopLossPrice));
            Validate.True(stopLossPrice + this.TickSize > 0, nameof(stopLossPrice));

            if (stopLossPrice > this.BestStopLong)
            {
                stopLossPrice = this.BestStopLong;
            }

            return isSignal
                ? Option<ITrailingStopResponse>.Some(
                    new TrailingStopResponse(
                        this.AlgorithmLabel,
                        MarketPosition.Long,
                        stopLossPrice,
                        this.ForUnit,
                        this.BarStore.Timestamp))

                : Option<ITrailingStopResponse>.None();
        }

        /// <summary>
        /// Runs a calculation of the trailing stop algorithm for short positions, and returns a
        /// response (optional value).
        /// </summary>
        /// <param name="isSignal">The boolean flag indicating whether there is an exit signal.</param>
        /// <param name="stopLossPrice">The stop-loss price.</param>
        /// <returns>A <see cref="Option{ITrailingStopResponse}"/>.</returns>
        protected Option<ITrailingStopResponse> SignalResponseShort(bool isSignal, Price stopLossPrice)
        {
            Validate.NotNull(stopLossPrice, nameof(stopLossPrice));
            Validate.True(stopLossPrice - this.AverageSpread - this.TickSize > 0, nameof(stopLossPrice));

            if (stopLossPrice < this.BestStopShort)
            {
                stopLossPrice = this.BestStopShort;
            }

            return isSignal
                ? Option<ITrailingStopResponse>.Some(
                    new TrailingStopResponse(
                        this.AlgorithmLabel,
                        MarketPosition.Short,
                        stopLossPrice,
                        this.ForUnit,
                        this.BarStore.Timestamp))

                : Option<ITrailingStopResponse>.None();
        }
    }
}