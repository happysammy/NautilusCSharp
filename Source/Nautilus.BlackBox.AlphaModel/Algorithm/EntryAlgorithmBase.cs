//--------------------------------------------------------------
// <copyright file="EntryAlgorithmBase.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Algorithm
{
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.AlphaModel.Signal;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The abstract <see cref="EntryAlgorithmBase"/> class. The base class for all entry algorithms.
    /// </summary>
    public abstract class EntryAlgorithmBase : AlgorithmBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntryAlgorithmBase"/> class.
        /// </summary>
        /// <param name="algorithmLabel">The entry algorithm label.</param>
        /// <param name="tradeProfile">The entry algorithm trade period.</param>
        /// <param name="instrument">The entry algorithm instrument.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        protected EntryAlgorithmBase(
            Label algorithmLabel,
            TradeProfile tradeProfile,
            Instrument instrument)
            : base(algorithmLabel, tradeProfile, instrument)
        {
            Validate.NotNull(algorithmLabel, nameof(algorithmLabel));
            Validate.NotNull(tradeProfile, nameof(tradeProfile));
            Validate.NotNull(instrument, nameof(instrument));
        }

        /// <summary>
        /// Returns an <see cref="Option{IEntryResponse}"/> containing a buy entry response
        /// (optional).
        /// </summary>
        /// <param name="isSignal">The is signal boolean flag.</param>
        /// <param name="entryPrice">The entry price.</param>
        /// <returns>A <see cref="Option{IEntryResponse}"/> result.</returns>
        /// <exception cref="ValidationException">Throws if the entry price is null.</exception>
        protected Option<IEntryResponse> SignalResponseBuy(bool isSignal, Price entryPrice)
        {
            Validate.NotNull(entryPrice, nameof(entryPrice));

            return isSignal && entryPrice >= this.BestEntryStopBuy
                ? Option<IEntryResponse>.Some(
                    new EntryResponse(
                        this.AlgorithmLabel,
                        OrderSide.Buy,
                        entryPrice,
                        this.BarStore.Timestamp))

                : Option<IEntryResponse>.None();
        }

        /// <summary>
        /// Returns an <see cref="Option{IEntryResponse}"/> containing a sell entry response
        /// (optional).
        /// </summary>
        /// <param name="isSignal">The is signal boolean flag.</param>
        /// <param name="entryPrice">The entry price.</param>
        /// <returns>A <see cref="Option{IEntryResponse}"/> result.</returns>
        /// <exception cref="ValidationException">Throws if the entry price is null.</exception>
        protected Option<IEntryResponse> SignalResponseSell(bool isSignal, Price entryPrice)
        {
            Validate.NotNull(entryPrice, nameof(entryPrice));

            return isSignal && entryPrice <= this.BestEntryStopSell
                ? Option<IEntryResponse>.Some(
                    new EntryResponse(
                        this.AlgorithmLabel,
                        OrderSide.Sell,
                        entryPrice,
                        this.BarStore.Timestamp))

                : Option<IEntryResponse>.None();
        }
    }
}