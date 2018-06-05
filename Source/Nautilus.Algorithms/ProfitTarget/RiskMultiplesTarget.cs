//--------------------------------------------------------------------------------------------------
// <copyright file="RiskMultiplesTarget.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Algorithms.ProfitTarget
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.AlphaModel.Algorithm;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The profit target calculator.
    /// </summary>
    public sealed class RiskMultiplesTarget : AlgorithmBase, IProfitTargetAlgorithm
    {
        private readonly int profitTargetCount;
        private readonly decimal riskMultiple;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiskMultiplesTarget"/> class.
        /// </summary>
        /// <param name="tradeProfile">
        /// The trade Profile.
        /// </param>
        /// <param name="instrument">
        /// The instrument.
        /// </param>
        /// <param name="profitTargetCount">
        /// The profit target count.
        /// </param>
        /// <param name="riskMultiple">
        /// The risk multiple.
        /// </param>
        public RiskMultiplesTarget(
            TradeProfile tradeProfile,
            Instrument instrument,
            int profitTargetCount,
            decimal riskMultiple)
            : base(new Label(nameof(RiskMultiplesTarget)), tradeProfile, instrument)
        {
           Validate.NotNull(tradeProfile, nameof(tradeProfile));
           Validate.NotNull(tradeProfile, nameof(tradeProfile));
           Validate.Int32NotOutOfRange(profitTargetCount, nameof(profitTargetCount), 0, int.MaxValue, RangeEndPoints.Exclusive);
           Validate.DecimalNotOutOfRange(riskMultiple, nameof(riskMultiple), 0, decimal.MaxValue, RangeEndPoints.Exclusive);

            this.profitTargetCount = profitTargetCount;
            this.riskMultiple = riskMultiple;
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
        /// The calculate buy.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        /// <param name="stopLoss">
        /// The stop-loss.
        /// </param>
        /// <returns>
        /// The sorted dictionary>.
        /// </returns>
        public IReadOnlyDictionary<int, Price> CalculateBuy(Price entry, Price stopLoss)
        {
           Validate.NotNull(entry, nameof(entry));
           Validate.NotNull(stopLoss, nameof(stopLoss));

            var profitTargets = new SortedDictionary<int, Price>();

            var riskSize = entry.Value - stopLoss.Value;

            for (int i = 0; i < this.profitTargetCount; i++)
            {
                var profitTarget = entry.Value + (riskSize * this.riskMultiple) + (i * riskSize * this.riskMultiple);

                profitTarget = Math.Round(profitTarget, this.DecimalPlaces);

                profitTargets.Add(i + 1, Price.Create(profitTarget, this.TickSize));
            }

            Debug.CollectionNotNullOrEmpty(profitTargets, nameof(profitTargets));

            return profitTargets.ToImmutableSortedDictionary();
        }

        /// <summary>
        /// The calculate sell.
        /// </summary>
        /// <param name="entry">
        /// The entry.
        /// </param>
        /// <param name="stopLoss">
        /// The stopLoss.
        /// </param>
        /// <returns>
        /// The sorted dictionary.
        /// </returns>
        public IReadOnlyDictionary<int, Price> CalculateSell(Price entry, Price stopLoss)
        {
           Validate.NotNull(entry, nameof(entry));
           Validate.NotNull(stopLoss, nameof(stopLoss));

            var profitTargets = new SortedDictionary<int, Price>();

            var riskSize = stopLoss.Value - entry.Value;

            for (int i = 0; i < this.profitTargetCount; i++)
            {
                var profitTarget = entry.Value - (riskSize * this.riskMultiple) - (i * riskSize * this.riskMultiple);

                profitTarget = Math.Round(profitTarget, this.DecimalPlaces);

                profitTargets.Add(i + 1, Price.Create(profitTarget, this.TickSize));
            }

            Debug.CollectionNotNullOrEmpty(profitTargets, nameof(profitTargets));

            return profitTargets.ToImmutableSortedDictionary();
        }
    }
}