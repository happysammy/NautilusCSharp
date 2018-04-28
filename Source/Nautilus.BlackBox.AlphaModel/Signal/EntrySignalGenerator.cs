//--------------------------------------------------------------
// <copyright file="EntrySignalGenerator.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Signal
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using NautechSystems.CSharp.Extensions;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="EntrySignalGenerator"/> class. Generates valid entry signals based
    /// on the given entry algorithms, instrument and trade profile.
    /// </summary>
    public sealed class EntrySignalGenerator
    {
        private readonly Instrument instrument;
        private readonly TradeProfile tradeProfile;
        private readonly IReadOnlyCollection<IEntryAlgorithm> entryAlgorithms;
        private readonly IStopLossAlgorithm stopLossAlgorithm;
        private readonly IProfitTargetAlgorithm profitTargetAlgorithm;

        private int signalCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntrySignalGenerator"/> class.
        /// </summary>
        /// <param name="instrument">The entry signal instrument.</param>
        /// <param name="tradeProfile">The entry signal trade profile.</param>
        /// <param name="entryAlgorithms">The entry signal algorithms.</param>
        /// <param name="stopLossAlgorithm">The entry signal stop-loss algorithm.</param>
        /// <param name="profitTargetAlgorithm">The entry signal profit target algorithm.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public EntrySignalGenerator(
            Instrument instrument,
            TradeProfile tradeProfile,
            IReadOnlyCollection<IEntryAlgorithm> entryAlgorithms,
            IStopLossAlgorithm stopLossAlgorithm,
            IProfitTargetAlgorithm profitTargetAlgorithm)
        {
            Validate.NotNull(instrument, nameof(instrument));
            Validate.NotNull(tradeProfile, nameof(tradeProfile));
            Validate.NotNull(entryAlgorithms, nameof(entryAlgorithms));
            Validate.NotNull(stopLossAlgorithm, nameof(stopLossAlgorithm));
            Validate.NotNull(profitTargetAlgorithm, nameof(profitTargetAlgorithm));

            this.instrument = instrument;
            this.tradeProfile = tradeProfile;
            this.entryAlgorithms = entryAlgorithms.ToImmutableList();
            this.stopLossAlgorithm = stopLossAlgorithm;
            this.profitTargetAlgorithm = profitTargetAlgorithm;
        }

        /// <summary>
        /// Updates the entry signal generator with the given trade bar.
        /// </summary>
        /// <param name="bar">The trade bar.</param>
        /// <exception cref="ValidationException">Throws if the bar is null.</exception>
        public void Update(Bar bar)
        {
            Validate.NotNull(bar, nameof(bar));

            this.entryAlgorithms.ForEach(algorithm => algorithm.Update(bar));
            this.stopLossAlgorithm.Update(bar);
            this.profitTargetAlgorithm.Update(bar);
        }

        /// <summary>
        /// Runs a calculation of all entry algorithms and produces a collection of buy entry
        /// signals (could be empty if no signals are produced).
        /// </summary>
        /// <returns>A <see cref="IReadOnlyCollection{EntrySignal}"/>.</returns>
        public IReadOnlyCollection<EntrySignal> ProcessBuy()
        {
            var buySignals = new List<EntrySignal>();

            var algorithmResults = this.entryAlgorithms
               .Select(entryAlgorithm => entryAlgorithm.CalculateBuy())
               .ToList();

            buySignals.AddRange(
                from result in algorithmResults
                where result.HasValue
                let stopLossPrice = this.stopLossAlgorithm.CalculateBuy(result.Value.EntryPrice)
                let profitTargets = this.profitTargetAlgorithm.CalculateBuy(result.Value.EntryPrice, stopLossPrice)
                select this.BuildSignal(result.Value, stopLossPrice, profitTargets));

            return buySignals.ToImmutableList();
        }

        /// <summary>
        /// Runs a calculation of all entry algorithms and produces a collection of  sell entry
        /// signals (could be empty if no signals are produced).
        /// </summary>
        /// <returns>A <see cref="IReadOnlyCollection{EntrySignal}"/>.</returns>
        public IReadOnlyCollection<EntrySignal> ProcessSell()
        {
            var sellSignals = new List<EntrySignal>();

            var algorithmResults = this.entryAlgorithms
               .Select(entryAlgorithm => entryAlgorithm.CalculateSell())
               .ToList();

            sellSignals.AddRange(
                from result in algorithmResults
                where result.HasValue
                let stoplossPrice = this.stopLossAlgorithm.CalculateSell(result.Value.EntryPrice)
                let profitTargets = this.profitTargetAlgorithm.CalculateSell(result.Value.EntryPrice, stoplossPrice)
                select this.BuildSignal(result.Value, stoplossPrice, profitTargets));

            return sellSignals.ToImmutableList();
        }

        private EntrySignal BuildSignal(
            IEntryResponse response,
            Price stoplossPrice,
            IReadOnlyDictionary<int, Price> profitTargets)
        {
            Debug.NotNull(response, nameof(response));
            Debug.NotNull(stoplossPrice, nameof(stoplossPrice));
            Debug.NotNull(profitTargets, nameof(profitTargets));

            return new EntrySignal(
                this.instrument.Symbol,
                this.GetSignalId(response),
                response.Label,
                this.tradeProfile,
                response.OrderSide,
                response.EntryPrice,
                stoplossPrice,
                profitTargets,
                response.Time);
        }

        private EntityId GetSignalId(IEntryResponse response)
        {
            Debug.NotNull(response, nameof(response));

            this.signalCount++;

            return EntityIdFactory.Signal(
                response.Time,
                this.instrument.Symbol,
                response.OrderSide,
                this.tradeProfile.TradeType,
                response.Label,
                this.signalCount);
        }
    }
}
