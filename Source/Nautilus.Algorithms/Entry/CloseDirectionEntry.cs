//--------------------------------------------------------------
// <copyright file="CloseDirectionEntry.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.Algorithms.Entry
{
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.AlphaModel.Algorithm;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Test strategy which will generate a buy or sell signal on every bar.
    /// </summary>
    public sealed class CloseDirectionEntry : EntryAlgorithmBase, IEntryAlgorithm
    {
        private readonly IEntryStopAlgorithm entryStopAlgorithm;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloseDirectionEntry"/> class.
        /// </summary>
        /// <param name="tradeProfile">
        /// The trade Profile.
        /// </param>
        /// <param name="instrument">
        /// The instrument.
        /// </param>
        /// <param name="entryStopAlgorithm">
        /// The entry stop algorithm.
        /// </param>
        public CloseDirectionEntry(
            TradeProfile tradeProfile,
            Instrument instrument,
            IEntryStopAlgorithm entryStopAlgorithm)
            : base(new Label(nameof(CloseDirectionEntry)), tradeProfile, instrument)
        {
           Validate.NotNull(tradeProfile, nameof(tradeProfile));
           Validate.NotNull(instrument, nameof(instrument));
           Validate.NotNull(entryStopAlgorithm, nameof(entryStopAlgorithm));

            this.entryStopAlgorithm = entryStopAlgorithm;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="bar"></param>
        public void Update(Bar bar)
        {
           Validate.NotNull(bar, nameof(bar));

            this.entryStopAlgorithm.Update(bar);
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

            var isSignal = this.GetClose(0) >= this.GetOpen(0);

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

            var isSignal = this.GetClose(0) < this.GetOpen(0);

            return this.SignalResponseSell(isSignal, entryPrice);
        }
    }
}