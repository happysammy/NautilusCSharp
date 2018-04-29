//--------------------------------------------------------------------------------------------------
// <copyright file="ExitSignalFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Signal
{
    using System.Collections.Generic;
    using System.Linq;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The immutable sealed <see cref="ExitSignalFactory"/> class. Creates valid exit signals for
    /// an <see cref="ExitSignalGenerator"/>.
    /// </summary>
    [Immutable]
    public sealed class ExitSignalFactory
    {
        private readonly Instrument instrument;
        private readonly TradeType tradeType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitSignalFactory"/> class.
        /// </summary>
        /// <param name="instrument">The exit signal instrument.</param>
        /// <param name="tradeType">The exit signal trade type.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public ExitSignalFactory(
            Instrument instrument,
            TradeType tradeType)
        {
            Validate.NotNull(instrument, nameof(instrument));
            Validate.NotNull(tradeType, nameof(tradeType));

            this.instrument = instrument;
            this.tradeType = tradeType;
        }

        /// <summary>
        /// Creates and returns a new exit signal based on the given collection of exit responses.
        /// </summary>
        /// <param name="exitSignalResponses">The exit signal responses collection.</param>
        /// <returns>A <see cref="ExitSignal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the given collection is null.</exception>
        public ExitSignal CreateSignalLong(IReadOnlyCollection<IExitResponse> exitSignalResponses)
        {
            Validate.NotNull(exitSignalResponses, nameof(exitSignalResponses));

            var exitUnits = exitSignalResponses
               .Select(e => e.ForUnit)
               .Distinct()
               .ToList();

            var exitLabel = LabelFactory.Exit(
                exitSignalResponses
               .Select(e => e.Label)
               .Distinct()
               .ToList());

            if (exitUnits.Any(e => e == 0))
            {
                exitUnits = new List<int> { 0 };
            }

            return new ExitSignal(
                this.instrument.Symbol,
                new EntityId(exitLabel.ToString()),
                exitLabel,
                this.tradeType,
                MarketPosition.Long,
                exitUnits,
                exitSignalResponses.First().Time);
        }

        /// <summary>
        /// Creates and returns a new exit signal based on the given collection of exit responses.
        /// </summary>
        /// <param name="exitSignalResponses">The exit signal responses collection.</param>
        /// <returns>A <see cref="ExitSignal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the given collection is null.</exception>
        public ExitSignal CreateSignalShort(IReadOnlyCollection<IExitResponse> exitSignalResponses)
        {
            Validate.NotNull(exitSignalResponses, nameof(exitSignalResponses));

            var exitUnits = exitSignalResponses
               .Select(e => e.ForUnit)
               .Distinct()
               .ToList();

            var exitLabel = LabelFactory.Exit(
                exitSignalResponses
               .Select(e => e.Label)
               .Distinct()
               .ToList());

            if (exitUnits.Any(e => e == 0))
            {
                exitUnits = new List<int> { 0 };
            }

            return new ExitSignal(
                this.instrument.Symbol,
                new EntityId(exitLabel.ToString()),
                exitLabel,
                this.tradeType,
                MarketPosition.Short,
                exitUnits,
                exitSignalResponses.First().Time);
        }
    }
}
