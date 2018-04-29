//--------------------------------------------------------------------------------------------------
// <copyright file="BarsBackTrail.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Algorithms.TrailingStop
{
    using System;
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.AlphaModel.Algorithm;
    using Nautilus.BlackBox.AlphaModel.Signal;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The bars back trail.
    /// </summary>
    public sealed class BarsBackTrail : TrailingStopAlgorithmBase, ITrailingStopAlgorithm
    {
        private readonly int barsBack;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarsBackTrail"/> class.
        /// </summary>
        /// <param name="tradeProfile">
        /// The trade Profile.
        /// </param>
        /// <param name="instrument">
        /// The instrument.
        /// </param>
        /// <param name="barsBack">
        /// The bars back.
        /// </param>
        /// <param name="forUnit">
        /// The for unit.
        /// </param>
        public BarsBackTrail(
            TradeProfile tradeProfile,
            Instrument instrument,
            int barsBack,
            int forUnit)
            : base(new Label(nameof(BarsBackTrail)), tradeProfile, instrument, forUnit)
        {
           Validate.NotNull(tradeProfile, nameof(tradeProfile));
           Validate.NotNull(instrument, nameof(instrument));
           Validate.Int32NotOutOfRange(barsBack, nameof(barsBack), 0, int.MaxValue, RangeEndPoints.Exclusive);
           Validate.Int32NotOutOfRange(forUnit, nameof(barsBack), 0, int.MaxValue);

            this.barsBack = barsBack;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="bar">
        /// The bar.
        /// </param>
        public void Update(Bar bar)
        {
        }

        /// <summary>
        /// The process signal long.
        /// </summary>
        /// <returns>
        /// The <see cref="TrailingStopResponse"/>.
        /// </returns>
        public Option<ITrailingStopResponse> CalculateLong()
        {
            var newStoploss = Math.Round(this.BarStore.GetMinLow(this.barsBack, 0) - this.TickSize, this.DecimalPlaces);

            return this.SignalResponseLong(true, Price.Create(newStoploss, this.TickSize));
        }

        /// <summary>
        /// The process signal short.
        /// </summary>
        /// <returns>
        /// The <see cref="TrailingStopResponse"/>.
        /// </returns>
        public Option<ITrailingStopResponse> CalculateShort()
        {
            var newStoploss = Math.Round(this.BarStore.GetMaxHigh(this.barsBack, 0) + this.AverageSpread + this.TickSize, this.DecimalPlaces);

            return this.SignalResponseShort(true, Price.Create(newStoploss, this.TickSize));
        }
    }
}