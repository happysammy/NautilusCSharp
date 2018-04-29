//--------------------------------------------------------------------------------------------------
// <copyright file="ITrailingStopAlgorithm.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using NautechSystems.CSharp;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The <see cref="ITrailingStopAlgorithm"/> interface.
    /// </summary>
    public interface ITrailingStopAlgorithm
    {
        /// <summary>
        /// Initializes the trailing stop algorithm with the given <see cref="IBarStore"/>
        /// and <see cref="IMarketDataProvider"/>.
        /// </summary>
        /// <param name="barStore">The bar store.</param>
        /// <param name="marketDataProvider">The market data provider.</param>
        void Initialize(IBarStore barStore, IMarketDataProvider marketDataProvider);

        /// <summary>
        /// Updates the algorithm with the given trade <see cref="Bar"/>.
        /// </summary>
        /// <param name="bar">The trade bar.</param>
        void Update(Bar bar);

        /// <summary>
        /// Runs a calculation of the trailing stop algorithm for long positions, and returns a
        /// response (optional value).
        /// </summary>
        /// <returns> A <see cref="Option{ITrailingStopResponse}"/>.</returns>
        Option<ITrailingStopResponse> CalculateLong();

        /// <summary>
        /// Runs a calculation of the trailing stop algorithm for short positions, and returns a
        /// response (optional value).
        /// </summary>
        /// <returns> A <see cref="Option{IExitResponse}"/>.</returns>
        Option<ITrailingStopResponse> CalculateShort();
    }
}
