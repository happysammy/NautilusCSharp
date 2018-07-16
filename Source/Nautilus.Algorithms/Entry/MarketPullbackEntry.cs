//--------------------------------------------------------------------------------------------------
// <copyright file="MarketPullbackEntry.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Algorithms.Entry
{
    using Nautilus.Core;
    using Nautilus.Core.Validation;
    using Nautilus.Algorithms;
    using Nautilus.BlackBox.AlphaModel.Algorithm;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Indicators;

    /// <summary>
    /// The market pullback.
    /// </summary>
    public sealed class MarketPullbackEntry : EntryAlgorithmBase, IEntryAlgorithm
    {
        private readonly AverageTrueRange averageTrueRange;
        private readonly KeltnerChannel keltnerChannel;
        private readonly MarketStructure marketStructure;
        private readonly VolatilityChecker volatilityChecker;
        private readonly IEntryStopAlgorithm entryStopAlgorithm;
        private readonly IStopLossAlgorithm stopLossAlgorithm;
        private readonly IProfitTargetAlgorithm profitTargetAlgorithm;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketPullbackEntry"/> class.
        /// </summary>
        /// <param name="tradeProfile">
        /// The trade Profile.
        /// </param>
        /// <param name="instrument">
        /// The instrument.
        /// </param>
        /// <param name="keltnerMultiple">
        /// The <see cref="KeltnerChannel"/> multiple.
        /// </param>
        /// <param name="fastSma">
        /// The fast simple moving average.
        /// </param>
        /// <param name="slowSma">
        /// The slow simple moving average.
        /// </param>
        /// <param name="entryStopAlgorithm">
        /// The entry stop algorithm.
        /// </param>
        /// <param name="stopLossAlgorithm">
        /// The stop-loss algorithm.
        /// </param>
        /// <param name="profitTargetAlgorithm">
        /// The profit target algorithm.
        /// </param>
        public MarketPullbackEntry(
            TradeProfile tradeProfile,
            Instrument instrument,
            decimal keltnerMultiple,
            int fastSma,
            int slowSma,
            IEntryStopAlgorithm entryStopAlgorithm,
            IStopLossAlgorithm stopLossAlgorithm,
            IProfitTargetAlgorithm profitTargetAlgorithm)
            : base(new Label(nameof(MarketPullbackEntry)), tradeProfile, instrument)
        {
            Validate.NotNull(tradeProfile, nameof(tradeProfile));
            Validate.NotNull(instrument, nameof(instrument));
            Validate.DecimalNotOutOfRange(keltnerMultiple, nameof(keltnerMultiple), 0, int.MaxValue, RangeEndPoints.LowerExclusive);
            Validate.DecimalNotOutOfRange(fastSma, nameof(fastSma), 0, int.MaxValue, RangeEndPoints.LowerExclusive);
            Validate.DecimalNotOutOfRange(slowSma, nameof(slowSma), 0, int.MaxValue, RangeEndPoints.LowerExclusive);
            Validate.NotNull(entryStopAlgorithm, nameof(entryStopAlgorithm));
            Validate.NotNull(stopLossAlgorithm, nameof(stopLossAlgorithm));
            Validate.NotNull(profitTargetAlgorithm, nameof(profitTargetAlgorithm));

            this.averageTrueRange = new AverageTrueRange(this.TradePeriod, this.TickSize);
            this.keltnerChannel = new KeltnerChannel(this.TradePeriod, keltnerMultiple, this.TickSize);
            this.marketStructure = new MarketStructure(this.TradePeriod, keltnerMultiple, fastSma, slowSma, this.TickSize);
            this.volatilityChecker = new VolatilityChecker(this.TradeProfile.MinVolatilityAverageSpreadMultiple);

            this.entryStopAlgorithm = entryStopAlgorithm;
            this.stopLossAlgorithm = stopLossAlgorithm;
            this.profitTargetAlgorithm = profitTargetAlgorithm;
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

            this.averageTrueRange.Update(bar);
            this.keltnerChannel.Update(bar);
            this.marketStructure.Update(bar);
        }

        /// <summary>
        /// The calculate buy.
        /// </summary>
        /// <returns>
        /// The <see cref="IEntryResponse"/>.
        /// </returns>
        public Option<IEntryResponse> CalculateBuy()
        {
            var entryPrice = this.entryStopAlgorithm.CalculateBuy();

            var isSignal = this.volatilityChecker.IsTradable(this.AverageSpread, this.averageTrueRange.Value)
                         && this.marketStructure.GetSwingHighPrice(0) >= this.marketStructure.GetSwingHighPrice(1)
                         && this.marketStructure.GetSwingHighMomentum(0) >= this.marketStructure.GetHighestMomentumAtSwing(5)
                         && entryPrice.Value < this.keltnerChannel.Upper;

            return this.SignalResponseBuy(isSignal, entryPrice);
        }

        /// <summary>
        /// The calculate sell.
        /// </summary>
        /// <returns>
        /// The <see cref="IEntryResponse"/>.
        /// </returns>
        public Option<IEntryResponse> CalculateSell()
        {
            var entryPrice = this.entryStopAlgorithm.CalculateSell();

            var isSignal = this.volatilityChecker.IsTradable(this.AverageSpread, this.averageTrueRange.Value)
                           && this.marketStructure.GetSwingLowPrice(0) <= this.marketStructure.GetSwingLowPrice(1)
                           && this.marketStructure.GetSwingLowMomentum(0) <= this.marketStructure.GetLowestMomentumAtSwing(5)
                           && entryPrice.Value > this.keltnerChannel.Lower;

            return this.SignalResponseSell(isSignal, entryPrice);
        }
    }
}
