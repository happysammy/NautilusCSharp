//--------------------------------------------------------------
// <copyright file="ExhaustionBarExit.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Algorithms.Exit
{
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.AlphaModel.Algorithm;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators;
    using Nautilus.Indicators.Enums;

    /// <summary>
    /// An exit algorithm based on the fuzzy size of the last closed bar.
    /// </summary>
    public sealed class ExhaustionBarExit : ExitAlgorithmBase, IExitAlgorithm
    {
        private readonly FuzzyCandlesticks fuzzyCandlesticks;
        private readonly CandleSize exitCandleSizeTrigger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExhaustionBarExit"/> class.
        /// </summary>
        /// <param name="tradeProfile">
        /// The trade Profile.
        /// </param>
        /// <param name="instrument">
        /// The instrument.
        /// </param>
        /// <param name="exitCandleSizeTrigger">
        /// The exit candle size trigger.
        /// </param>
        /// <param name="forUnit">
        /// The for units.
        /// </param>
        public ExhaustionBarExit(
            TradeProfile tradeProfile,
            Instrument instrument,
            CandleSize exitCandleSizeTrigger,
            int forUnit)
            : base(new Label(nameof(ExhaustionBarExit)), tradeProfile, instrument, forUnit)
        {
            Validate.NotNull(tradeProfile, nameof(tradeProfile));
            Validate.NotNull(instrument, nameof(instrument));
            Validate.Int32NotOutOfRange(forUnit, nameof(forUnit), 0, int.MaxValue);

            this.fuzzyCandlesticks = new FuzzyCandlesticks(this.TradePeriod, this.TickSize);
            this.exitCandleSizeTrigger = exitCandleSizeTrigger;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="bar">
        /// The bar.
        /// </param>
        public void Update(Bar bar)
        {
            Validate.NotNull(bar, nameof(bar));

            this.fuzzyCandlesticks.Update(bar);
        }

        /// <summary>
        /// The process signal long.
        /// </summary>
        /// <returns>
        /// The <see cref="IExitResponse"/>.
        /// </returns>
        public Option<IExitResponse> CalculateLong()
        {
            var isSignal = this.fuzzyCandlesticks.Size >= this.exitCandleSizeTrigger;

            return this.SignalResponseLong(isSignal);
        }

        /// <summary>
        /// The process signal short.
        /// </summary>
        /// <returns>
        /// The <see cref="IExitResponse"/>.
        /// </returns>
        public Option<IExitResponse> CalculateShort()
        {
            var isSignal = this.fuzzyCandlesticks.Size >= this.exitCandleSizeTrigger;

            return this.SignalResponseShort(isSignal);
        }
    }
}