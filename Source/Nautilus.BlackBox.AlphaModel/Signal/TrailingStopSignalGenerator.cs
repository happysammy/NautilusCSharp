//--------------------------------------------------------------------------------------------------
// <copyright file="TrailingStopSignalGenerator.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Signal
{
    using System.Collections.Generic;
    using System.Linq;
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using NautechSystems.CSharp.Extensions;
    using Nautilus.BlackBox.Core.Interfaces;

    /// <summary>
    /// The trailing stop signal processor.
    /// </summary>
    public class TrailingStopSignalGenerator
    {
        private readonly IReadOnlyCollection<ITrailingStopAlgorithm> trailingStopAlgorithms;
        private readonly TrailingStopSignalFactory trailingStopSignalFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrailingStopSignalGenerator"/> class.
        /// </summary>
        /// <param name="instrument">
        /// The instrument.
        /// </param>
        /// <param name="tradeProfile">
        /// The trade profile.
        /// </param>
        /// <param name="trailingStopAlgorithms">
        /// The trailing stop algorithms.
        /// </param>
        public TrailingStopSignalGenerator(
            Instrument instrument,
            TradeProfile tradeProfile,
            IReadOnlyCollection<ITrailingStopAlgorithm> trailingStopAlgorithms)
        {
            Validate.NotNull(instrument, nameof(instrument));
            Validate.NotNull(tradeProfile, nameof(tradeProfile));
            Validate.NotNull(trailingStopAlgorithms, nameof(trailingStopAlgorithms));

            this.trailingStopAlgorithms = trailingStopAlgorithms;
            this.trailingStopSignalFactory = new TrailingStopSignalFactory(instrument, tradeProfile.TradeType);
        }

        /// <summary>
        /// Updates the trailing stop signal generator with the given trade bar.
        /// </summary>
        /// <param name="bar">The trade bar.</param>
        /// <exception cref="ValidationException">Throws if the bar is null.</exception>
        public void Update(Bar bar)
        {
            Validate.NotNull(bar, nameof(bar));

            this.trailingStopAlgorithms.ForEach(algorithm => algorithm.Update(bar));
        }

        /// <summary>
        /// Processes the long position trailing stop algorithms and returns an aggregated signal
        /// (optional value).
        /// </summary>
        /// <returns>A <see cref="Option{TrailingStopSignal}"/>.</returns>
        public Option<TrailingStopSignal> ProcessLong()
        {
            var longSignalResponses = this.trailingStopAlgorithms
               .Select(trailingStopAlgo => trailingStopAlgo.CalculateLong())
               .Where(response => response.HasValue)
               .Select(response => response.Value)
               .ToList();

            return longSignalResponses.Count > 0
                ? this.trailingStopSignalFactory.CreateSignalLong(longSignalResponses)
                : Option<TrailingStopSignal>.None();
        }

        /// <summary>
        /// Processes the short position trailing stop algorithms and returns an aggregated signal
        /// (optional value).
        /// </summary>
        /// <returns>A <see cref="Option{TrailingStopSignal}"/>.</returns>
        public Option<TrailingStopSignal> ProcessShort()
        {
            var shortSignalResponses = this.trailingStopAlgorithms
               .Select(trailingStopAlgo => trailingStopAlgo.CalculateShort())
               .Where(response => response.HasValue)
               .Select(response => response.Value)
               .ToList();

            return shortSignalResponses.Count > 0
                ? this.trailingStopSignalFactory.CreateSignalShort(shortSignalResponses)
                : Option<TrailingStopSignal>.None();
        }
    }
}
