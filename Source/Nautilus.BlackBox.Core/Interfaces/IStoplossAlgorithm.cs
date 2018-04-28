//--------------------------------------------------------------
// <copyright file="IStopLossAlgorithm.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The <see cref="IStopLossAlgorithm"/> interface.
    /// </summary>
    public interface IStopLossAlgorithm
    {
        /// <summary>
        /// Initializes the stop-loss algorithm with the given <see cref="IBarStore"/>. and
        /// <see cref="IMarketDataProvider"/>.
        /// </summary>
        /// <param name="barStore">The bar store.</param>
        /// <param name="marketDataProvider">The market data provider.</param>
        void Initialize(IBarStore barStore, IMarketDataProvider marketDataProvider);

        /// <summary>
        /// Updates the stop-loss algorithm with the given trade bar.
        /// </summary>
        /// <param name="bar">The bar.</param>
        void Update(Bar bar);

        /// <summary>
        /// Calculates and returns the buy side stop-loss <see cref="Price"/>.
        /// </summary>
        /// <param name="entryPrice">The buy entry price.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        Price CalculateBuy(Price entryPrice);

        /// <summary>
        /// Calculates and returns the sell side stop-loss <see cref="Price"/>.
        /// </summary>
        /// <param name="entryPrice">The sell entry price.</param>
        /// <returns>A <see cref="Price"/>.</returns>
        Price CalculateSell(Price entryPrice);
    }
}