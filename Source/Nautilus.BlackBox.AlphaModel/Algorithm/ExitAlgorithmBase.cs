//--------------------------------------------------------------------------------------------------
// <copyright file="ExitAlgorithmBase.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Algorithm
{
    using Nautilus.BlackBox.AlphaModel.Signal;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The base class for all exit algorithms.
    /// </summary>
    public abstract class ExitAlgorithmBase : AlgorithmBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExitAlgorithmBase"/> class.
        /// </summary>
        /// <param name="algorithmLabel">The exit algorithm label.</param>
        /// <param name="tradeProfile">The exit algorithm trade profile.</param>
        /// <param name="instrument">The exit algorithm instrument.</param>
        /// <param name="forUnit">The exit algorithm applicable trade unit number.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if the
        /// for unit is negative.</exception>
        protected ExitAlgorithmBase(
            Label algorithmLabel,
            TradeProfile tradeProfile,
            Instrument instrument,
            int forUnit)
            : base(algorithmLabel, tradeProfile, instrument)
        {
            Validate.NotNull(algorithmLabel, nameof(algorithmLabel));
            Validate.NotNull(tradeProfile, nameof(tradeProfile));
            Validate.NotNull(instrument, nameof(instrument));
            Validate.Int32NotOutOfRange(forUnit, nameof(forUnit), 0, int.MaxValue);

            this.ForUnit = forUnit;
        }

        /// <summary>
        /// Gets the exit algorithms applicable trade unit number.
        /// </summary>
        protected int ForUnit { get; }

        /// <summary>
        /// Runs a calculation of the exit algorithm for long position exits, and returns an exit
        /// response (optional value).
        /// </summary>
        /// <param name="isSignal">The boolean flag indicating whether there is an exit signal.</param>
        /// <returns> A <see cref="Option{IExitResponse}"/>.</returns>
        protected Option<IExitResponse> SignalResponseLong(bool isSignal)
        {
            return isSignal
                ? Option<IExitResponse>.Some(
                    new ExitResponse(
                        this.AlgorithmLabel,
                        MarketPosition.Long,
                        this.ForUnit,
                        this.BarStore.Timestamp))

                : Option<IExitResponse>.None();
        }

        /// <summary>
        /// Runs a calculation of the exit algorithm for short position exits, and returns an exit
        /// response (optional value).
        /// </summary>
        /// <param name="isSignal">The boolean flag indicating whether there is an exit signal.</param>
        /// <returns> A <see cref="Option{IExitResponse}"/>.</returns>
        protected Option<IExitResponse> SignalResponseShort(bool isSignal)
        {
            return isSignal
                ? Option<IExitResponse>.Some(
                    new ExitResponse(
                        this.AlgorithmLabel,
                        MarketPosition.Short,
                        this.ForUnit,
                        this.BarStore.Timestamp))

                : Option<IExitResponse>.None();
        }
    }
}
