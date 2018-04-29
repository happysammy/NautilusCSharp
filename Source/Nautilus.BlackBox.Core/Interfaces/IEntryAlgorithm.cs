//---------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntryAlgorithm.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//---------------------------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using NautechSystems.CSharp;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The <see cref="IEntryAlgorithm"/> interface.
    /// </summary>
    public interface IEntryAlgorithm
    {
        /// <summary>
        /// Initializes the entry algorithm with the given <see cref="IBarStore"/>. and
        /// <see cref="IMarketDataProvider"/>.
        /// </summary>
        /// <param name="barStore">The bar store.</param>
        /// <param name="marketDataProvider">The market data provider.</param>
        void Initialize(IBarStore barStore, IMarketDataProvider marketDataProvider);

        /// <summary>
        /// Updates the entry algorithm with the given trade bar.
        /// </summary>
        /// <param name="bar">The bar.</param>
        void Update(Bar bar);

        /// <summary>
        /// Runs a calculation of the entry algorithm for buy entries, and returns an entry response
        /// (optional).
        /// </summary>
        /// <returns>A <see cref="IEntryResponse"/>.</returns>
        Option<IEntryResponse> CalculateBuy();

        /// <summary>
        /// Runs a calculation of the entry algorithm for sell entries, and returns an entry response
        /// (optional).
        /// </summary>
        /// <returns>A <see cref="Option{IEntryResponse}"/>.</returns>
        Option<IEntryResponse> CalculateSell();
    }
}