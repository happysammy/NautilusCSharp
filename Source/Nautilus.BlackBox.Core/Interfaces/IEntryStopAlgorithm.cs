// -------------------------------------------------------------------------------------------------
// <copyright file="IEntryStopAlgorithm.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The <see cref="IEntryStopAlgorithm"/> interface.
    /// </summary>
    public interface IEntryStopAlgorithm
    {
        /// <summary>
        /// Initializes the algorithm with the given <see cref="IBarStore"/> 
        /// and <see cref="IMarketDataProvider"/>.
        /// </summary>
        /// <param name="barStore">The bar store.</param>
        /// <param name="marketDataProvider">The market data provider.</param>
        void Initialize(IBarStore barStore, IMarketDataProvider marketDataProvider);

        /// <summary>
        /// Updates with entry stop algorithm with the given trade <see cref="Bar"/>.
        /// </summary>
        /// <param name="bar">The trade bar.</param>
        void Update(Bar bar);

        /// <summary>
        /// Calculates and returns the buy side entry stop <see cref="Price"/>.
        /// </summary>
        /// <returns>A <see cref="Price"/>.</returns>
        Price CalculateBuy();

        /// <summary>
        /// Calculates and returns the sell side entry stop <see cref="Price"/>.
        /// </summary>
        /// <returns>A <see cref="Price"/>.</returns>
        Price CalculateSell();
    }
}
