//--------------------------------------------------------------
// <copyright file="IAlphaStrategy.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// The <see cref="IAlphaStrategy"/> interface. Represents the modular components of a complete
    /// financial market trading strategy.
    /// </summary>
    public interface IAlphaStrategy
    {
        /// <summary>
        /// Gets the strategies instrument.
        /// </summary>
        Instrument Instrument { get; }

        /// <summary>
        /// Gets the strategies trade profile.
        /// </summary>
        TradeProfile TradeProfile { get; }

        /// <summary>
        /// Gets the strategies signal logic.
        /// </summary>
        ISignalLogic SignalLogic { get; }

        /// <summary>
        /// Gets the strategies entry stop algorithm.
        /// </summary>
        IEntryStopAlgorithm EntryStopAlgorithm { get; }

        /// <summary>
        /// Gets the strategies stop-loss algorithm.
        /// </summary>
        IStopLossAlgorithm StopLossAlgorithm { get; }

        /// <summary>
        /// Gets the strategies profit target algorithm.
        /// </summary>
        IProfitTargetAlgorithm ProfitTargetAlgorithm { get; }

        /// <summary>
        /// Gets the strategies entry algorithm(s).
        /// </summary>
        IReadOnlyCollection<IEntryAlgorithm> EntryAlgorithms { get; }

        /// <summary>
        /// Gets the strategies trailing stop algorithm(s).
        /// </summary>
        IReadOnlyCollection<ITrailingStopAlgorithm> TrailingStopAlgorithms { get; }

        /// <summary>
        /// Gets the strategies exit algorithm(s).
        /// </summary>
        IReadOnlyCollection<IExitAlgorithm> ExitAlgorithms { get; }
    }
}