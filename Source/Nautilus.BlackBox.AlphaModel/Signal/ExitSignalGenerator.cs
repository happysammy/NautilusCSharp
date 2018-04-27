// -------------------------------------------------------------------------------------------------
// <copyright file="ExitSignalGenerator.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Signal
{
    using System.Collections.Generic;
    using System.Linq;
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Extensions;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The sealed <see cref="ExitSignalGenerator"/> class. Generates valid exit signals based
    /// on the given exit algorithms, instrument and trade profile.
    /// </summary>
    public sealed class ExitSignalGenerator
    {
        private readonly IReadOnlyCollection<IExitAlgorithm> exitAlgorithms;
        private readonly ExitSignalFactory exitSignalFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitSignalGenerator"/> class.
        /// </summary>
        /// <param name="instrument">The exit signal generator instrument.</param>
        /// <param name="tradeProfile">The exit signal generator trade profile.</param>
        /// <param name="exitAlgorithms">The exit signal generator exit algorithms collection.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public ExitSignalGenerator(
            Instrument instrument,
            TradeProfile tradeProfile,
            IReadOnlyCollection<IExitAlgorithm> exitAlgorithms)
        {
            Validate.NotNull(instrument, nameof(instrument));
            Validate.NotNull(tradeProfile, nameof(tradeProfile));
            Validate.NotNull(exitAlgorithms, nameof(exitAlgorithms));

            this.exitAlgorithms = exitAlgorithms;
            this.exitSignalFactory = new ExitSignalFactory(instrument, tradeProfile.TradeType);
        }

        /// <summary>
        /// Updates the exit signal generator with the given trade bar.
        /// </summary>
        /// <param name="bar">The trade bar.</param>
        /// <exception cref="ValidationException">Throws if the bar is null.</exception>
        public void Update(Bar bar)
        {
            Validate.NotNull(bar, nameof(bar));

            this.exitAlgorithms.ForEach(algorithm => algorithm.Update(bar));
        }

        /// <summary>
        /// Processes the long position exit algorithms and returns an aggregated signal (optional
        /// value).
        /// </summary>
        /// <returns>A <see cref="Option{ExitSignal}"/>.</returns>
        public Option<ExitSignal> ProcessLong()
        {
            var longSignalResponses = this.exitAlgorithms
               .Select(exitAlgo => exitAlgo.CalculateLong())
               .Where(response => response.HasValue)
               .Select(response => response.Value)
               .ToList();

            return longSignalResponses.Count > 0
                ? this.exitSignalFactory.CreateSignalLong(longSignalResponses)
                : Option<ExitSignal>.None();
        }

        /// <summary>
        /// Processes the short position exit algorithms and returns an aggregated signal (optional
        /// value).
        /// </summary>
        /// <returns>A <see cref="Option{ExitSignal}"/>.</returns>
        public Option<ExitSignal> ProcessShort()
        {
            var shortSignalResponses = this.exitAlgorithms
               .Select(exitAlgo => exitAlgo.CalculateShort())
               .Where(response => response.HasValue)
               .Select(response => response.Value)
               .ToList();

            return shortSignalResponses.Count > 0
                ? this.exitSignalFactory.CreateSignalShort(shortSignalResponses)
                : Option<ExitSignal>.None();
        }
    }
}
