//--------------------------------------------------------------------------------------------------
// <copyright file="PositionSizer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio.Orders
{
    using System;
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.BlackBox.Core;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="PositionSizer"/> class.
    /// </summary>
    public sealed class PositionSizer : ComponentBase
    {
        private readonly Instrument instrument;

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionSizer"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="instrument">The instrument.</param>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public PositionSizer(
            BlackBoxSetupContainer setupContainer,
            Instrument instrument)
            : base(
            BlackBoxService.Portfolio,
            LabelFactory.Component(nameof(PositionSizer), instrument.Symbol),
            setupContainer)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(instrument, nameof(instrument));

            this.instrument = instrument;
        }

        /// <summary>
        /// Calculates the position size based on the inputs and returns a <see cref="Quantity"/>
        /// per trade unit.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <param name="cashBalance">The cash balance.</param>
        /// <param name="tradeRisk">The trade risk.</param>
        /// <param name="hardLimit">The hard limit.</param>
        /// <param name="exchangeRate">The exchange rate.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if the
        /// exchange rate is zero or negative.</exception>
        public Quantity Calculate(
            EntrySignal signal,
            Money cashBalance,
            Percentage tradeRisk,
            Option<Quantity> hardLimit,
            decimal exchangeRate)
        {
            Validate.NotNull(signal, nameof(signal));
            Validate.NotNull(cashBalance, nameof(cashBalance));
            Validate.NotNull(tradeRisk, nameof(tradeRisk));
            Validate.NotNull(hardLimit, nameof(hardLimit));
            Validate.DecimalNotOutOfRange(exchangeRate, nameof(exchangeRate), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);

            var riskDollars = GetRiskDollars(cashBalance, tradeRisk);

            var riskPoints = this.GetRiskPoints(
                signal.EntryPrice,
                signal.StopLossPrice);

            var tickValueSize = this.instrument.TickSize * exchangeRate;

            var positionSize = Math.Floor(((riskDollars / riskPoints) / tickValueSize) / this.instrument.ContractSize);

            if (hardLimit.HasValue)
            {
                positionSize = Math.Min(hardLimit.Value.Value, positionSize);
            }

            var units = signal.TradeProfile.Units.Value;
            var unitBatchSize = signal.TradeProfile.UnitBatches.Value;

            var positionSizeBatched = Math.Floor(positionSize / units / unitBatchSize) * unitBatchSize;

            this.LogPositionSizing(
                tradeRisk,
                riskDollars,
                riskPoints,
                tickValueSize,
                positionSize,
                units,
                unitBatchSize,
                positionSizeBatched);

            return positionSizeBatched > 0 ? Quantity.Create(Convert.ToInt32(positionSizeBatched)) : Quantity.Zero();
        }

        private static Money GetRiskDollars(Money freeEquity, Percentage tradeRisk)
        {
            Debug.NotNull(freeEquity, nameof(freeEquity));
            Debug.NotNull(tradeRisk, nameof(tradeRisk));

            var riskDollars = Math.Floor(tradeRisk.PercentOf(freeEquity.Value));

            return Money.Create(riskDollars, freeEquity.Currency);
        }

        private int GetRiskPoints(Price entryPrice, Price stoplossPrice)
        {
            Debug.NotNull(entryPrice, nameof(entryPrice));
            Debug.NotNull(stoplossPrice, nameof(stoplossPrice));

            return Convert.ToInt32(Math.Ceiling(Math.Abs(entryPrice - stoplossPrice) / this.instrument.TickSize));
        }

        private void LogPositionSizing(
            Percentage riskPerTrade,
            Money riskDollars,
            decimal riskPoints,
            decimal tickValueSize,
            decimal positionSize,
            int units,
            int unitBatches,
            decimal positionSizeFinal)
        {
            var logtext3 = $"RiskPerTrade={riskPerTrade} ((RiskDollars={riskDollars} / RiskPoints={riskPoints})";
            var logtext4 = $"/ tickValueSize={tickValueSize}): TotalPositionSize={positionSize}";
            var logtext5 = $"/ Units={units} (Batches={unitBatches}), FinalPositionSize={positionSizeFinal}";
            this.Log.Debug("{logtext3} {logtext4} {logtext5}");
        }
    }
}
