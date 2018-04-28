//--------------------------------------------------------------
// <copyright file="TrailingStopSignalFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

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
    /// The immutable sealed <see cref="TrailingStopSignalFactory"/> class. Creates valid trailing
    /// stop signals for a <see cref="TrailingStopSignalGenerator"/>.
    /// </summary>
    [Immutable]
    public sealed class TrailingStopSignalFactory
    {
        private readonly Instrument instrument;
        private readonly TradeType tradeType;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrailingStopSignalFactory"/> class.
        /// </summary>
        /// <param name="instrument">The trailing stop signal factory instrument.</param>
        /// <param name="tradeType">The trailing stop signal factory trade type.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public TrailingStopSignalFactory(
            Instrument instrument,
            TradeType tradeType)
        {
            Validate.NotNull(instrument, nameof(instrument));
            Validate.NotNull(tradeType, nameof(tradeType));

            this.instrument = instrument;
            this.tradeType = tradeType;
        }

        /// <summary>
        /// Creates and returns a new trailing stop signal based on the given collection of exit
        /// responses.
        /// </summary>
        /// <param name="trailingStopSignalResponses">The trailing stop signal responses.</param>
        /// <returns>A <see cref="TrailingStopSignal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the given collection is null.</exception>
        public TrailingStopSignal CreateSignalLong(IReadOnlyCollection<ITrailingStopResponse> trailingStopSignalResponses)
        {
            Validate.NotNull(trailingStopSignalResponses, nameof(trailingStopSignalResponses));

            var signalString = string.Empty;

            var forUnitStoplossPrices = new Dictionary<int, Price>();

            foreach (var response in trailingStopSignalResponses)
            {

                if (!forUnitStoplossPrices.ContainsKey(response.ForUnit))
                {
                    forUnitStoplossPrices.Add(response.ForUnit, response.StopLossPrice);
                }

                // The dictionary already contains a stoploss for this unit
                // and the response stoploss is greater than the current for unit stoploss.
                if (response.StopLossPrice.Value > forUnitStoplossPrices[response.ForUnit].Value)
                {
                    forUnitStoplossPrices[response.ForUnit] = response.StopLossPrice;
                }

                signalString = LabelFactory.TrailingStop(signalString, response.Label);
            }

            if (forUnitStoplossPrices.ContainsKey(0))
            {
                // Removes any stoploss which is less than the all units (0) trailing stop.
                foreach (var forUnitTrailingStop in forUnitStoplossPrices.ToList())
                {
                    if (forUnitTrailingStop.Value.Value < forUnitStoplossPrices[0].Value)
                    {
                        forUnitStoplossPrices.Remove(forUnitTrailingStop.Key);
                    }
                }
            }

            return new TrailingStopSignal(
                this.instrument.Symbol,
                new EntityId(signalString),
                new Label(signalString),
                this.tradeType,
                MarketPosition.Long,
                forUnitStoplossPrices,
                trailingStopSignalResponses.First().Time);
        }

        /// <summary>
        /// Creates and returns a new trailing stop signal based on the given collection of exit
        /// responses.
        /// </summary>
        /// <param name="trailingStopSignalResponses">The trailing stop signal responses collection.</param>
        /// <returns>A <see cref="TrailingStopSignal"/>.</returns>
        /// <exception cref="ValidationException">Throws if the given collection is null.</exception>
        public TrailingStopSignal CreateSignalShort(IReadOnlyCollection<ITrailingStopResponse> trailingStopSignalResponses)
        {
            Validate.NotNull(trailingStopSignalResponses, nameof(trailingStopSignalResponses));

            var signalString = string.Empty;

            var forUnitStoplossPrices = new Dictionary<int, Price>();

            foreach (var response in trailingStopSignalResponses)
            {
                if (!forUnitStoplossPrices.ContainsKey(response.ForUnit))
                {
                    forUnitStoplossPrices.Add(response.ForUnit, response.StopLossPrice);
                }

                // The dictionary already contains a stoploss for this unit
                // and the response stoploss is less than the current for unit stoploss.
                if (response.StopLossPrice.Value < forUnitStoplossPrices[response.ForUnit].Value)
                {
                    forUnitStoplossPrices[response.ForUnit] = response.StopLossPrice;
                }

                signalString = LabelFactory.TrailingStop(signalString, response.Label);
            }

            if (forUnitStoplossPrices.ContainsKey(0))
            {
                // Removes any stoploss which is greater than the all units (0) trailing stop.
                foreach (var forUnitTrailingStop in forUnitStoplossPrices.ToList())
                {
                    if (forUnitTrailingStop.Value.Value > forUnitStoplossPrices[0].Value)
                    {
                        forUnitStoplossPrices.Remove(forUnitTrailingStop.Key);
                    }
                }
            }

            return new TrailingStopSignal(
                this.instrument.Symbol,
                new EntityId(signalString),
                new Label(signalString),
                this.tradeType,
                MarketPosition.Short,
                forUnitStoplossPrices,
                trailingStopSignalResponses.First().Time);
        }
    }
}
