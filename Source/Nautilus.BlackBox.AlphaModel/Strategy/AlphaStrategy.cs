// -------------------------------------------------------------------------------------------------
// <copyright file="AlphaStrategy.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Strategy
{
    using System.Collections.Generic;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// The immutable sealed <see cref="AlphaStrategy"/> class. Represents the modular components
    /// of a complete financial market trading strategy.
    /// </summary>
    [Immutable]
    public sealed class AlphaStrategy : IAlphaStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlphaStrategy"/> class.
        /// </summary>
        /// <param name="instrument">The strategy instrument.</param>
        /// <param name="tradeProfile">The strategy trade profile.</param>
        /// <param name="signalLogic">The strategy signal logic finite state machine.</param>
        /// <param name="entryStopAlgorithm">The strategy entry stop algorithm.</param>
        /// <param name="stopLossAlgorithm">The strategy stop-loss algorithm.</param>
        /// <param name="profitTargetAlgorithm">The strategy profit target algorithm.</param>
        /// <param name="entryAlgorithms">The strategy entry algorithm(s).</param>
        /// <param name="trailingStopAlgorithms">The strategy trailing stop algorithm(s).</param>
        /// <param name="exitAlgorithms">The strategy exit algorithm(s).</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public AlphaStrategy(
            Instrument instrument,
            TradeProfile tradeProfile,
            ISignalLogic signalLogic,
            IEntryStopAlgorithm entryStopAlgorithm,
            IStopLossAlgorithm stopLossAlgorithm,
            IProfitTargetAlgorithm profitTargetAlgorithm,
            IReadOnlyCollection<IEntryAlgorithm> entryAlgorithms,
            IReadOnlyCollection<ITrailingStopAlgorithm> trailingStopAlgorithms,
            IReadOnlyCollection<IExitAlgorithm> exitAlgorithms)
        {
            Validate.NotNull(instrument, nameof(instrument));
            Validate.NotNull(tradeProfile, nameof(tradeProfile));
            Validate.NotNull(signalLogic, nameof(signalLogic));
            Validate.NotNull(entryStopAlgorithm, nameof(entryStopAlgorithm));
            Validate.NotNull(stopLossAlgorithm, nameof(stopLossAlgorithm));
            Validate.NotNull(profitTargetAlgorithm, nameof(profitTargetAlgorithm));
            Validate.NotNull(entryAlgorithms, nameof(entryAlgorithms));
            Validate.NotNull(trailingStopAlgorithms, nameof(trailingStopAlgorithms));
            Validate.NotNull(exitAlgorithms, nameof(exitAlgorithms));

            this.Instrument = instrument;
            this.TradeProfile = tradeProfile;
            this.SignalLogic = signalLogic;
            this.EntryStopAlgorithm = entryStopAlgorithm;
            this.StopLossAlgorithm = stopLossAlgorithm;
            this.ProfitTargetAlgorithm = profitTargetAlgorithm;
            this.EntryAlgorithms = entryAlgorithms;
            this.TrailingStopAlgorithms = trailingStopAlgorithms;
            this.ExitAlgorithms = exitAlgorithms;
        }

        /// <summary>
        /// Gets the strategies instrument.
        /// </summary>
        public Instrument Instrument { get; }

        /// <summary>
        /// Gets the strategies trade profile.
        /// </summary>
        public TradeProfile TradeProfile { get; }

        /// <summary>
        /// Gets the strategies signal logic.
        /// </summary>
        public ISignalLogic SignalLogic { get; }

        /// <summary>
        /// Gets the strategies entry stop algorithm.
        /// </summary>
        public IEntryStopAlgorithm EntryStopAlgorithm { get; }

        /// <summary>
        /// Gets the strategies stop-loss algorithm.
        /// </summary>
        public IStopLossAlgorithm StopLossAlgorithm { get; }

        /// <summary>
        /// Gets the strategies profit target algorithm.
        /// </summary>
        public IProfitTargetAlgorithm ProfitTargetAlgorithm { get; }

        /// <summary>
        /// Gets the strategies entry algorithms(s).
        /// </summary>
        public IReadOnlyCollection<IEntryAlgorithm> EntryAlgorithms { get; }

        /// <summary>
        /// Gets the strategies trailing stop-loss algorithm(s).
        /// </summary>
        public IReadOnlyCollection<ITrailingStopAlgorithm> TrailingStopAlgorithms { get; }

        /// <summary>
        /// Gets the strategies exit algorithm(s).
        /// </summary>
        public IReadOnlyCollection<IExitAlgorithm> ExitAlgorithms { get; }
    }
}
