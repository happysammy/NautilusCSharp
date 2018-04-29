//--------------------------------------------------------------------------------------------------
// <copyright file="IProfitTargetAlgorithm.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The <see cref="IProfitTargetAlgorithm"/> interface.
    /// </summary>
    public interface IProfitTargetAlgorithm
    {
        /// <summary>
        /// Initializes the profit target algorithm with the given <see cref="IBarStore"/>
        /// and <see cref="IMarketDataProvider"/>.
        /// </summary>
        /// <param name="barStore">The bar store.</param>
        /// <param name="marketDataProvider">The market data provider.</param>
        void Initialize(IBarStore barStore, IMarketDataProvider marketDataProvider);

        /// <summary>
        /// Updates the profit target algorithm with the given trade bar.
        /// </summary>
        /// <param name="bar">The bar.</param>
        void Update(Bar bar);

        /// <summary>
        /// Runs a calculation of the profit target algorithm for buy entries, and returns a
        /// dictionary of trade unit to price.
        /// </summary>
        /// <param name="entry">The entry price.</param>
        /// <param name="stoploss">The stop-loss price.</param>
        /// <returns>A <see cref="IReadOnlyDictionary{TKey,TValue}"/>.</returns>
        IReadOnlyDictionary<int, Price> CalculateBuy(Price entry, Price stoploss);

        /// <summary>
        /// Runs a calculation of the profit target algorithm for sell entries, and returns a
        /// dictionary of trade unit to price.
        /// </summary>
        /// <param name="entry">The entry price.</param>
        /// <param name="stoploss">The stop-loss price.</param>
        /// <returns>A <see cref="IReadOnlyDictionary{TKey,TValue}"/>.</returns>
        IReadOnlyDictionary<int, Price> CalculateSell(Price entry, Price stoploss);
    }
}