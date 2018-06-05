//--------------------------------------------------------------------------------------------------
// <copyright file="IExitAlgorithm.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using Nautilus.Core;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The <see cref="IExitAlgorithm"/> interface.
    /// </summary>
    public interface IExitAlgorithm
    {
        /// <summary>
        /// Initializes the exit algorithm with the given <see cref="IBarStore"/>
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
        /// Runs a calculation of the exit algorithm for long position exits, and returns an exit
        /// response (optional value).
        /// </summary>
        /// <returns> A <see cref="Option{IExitResponse}"/>.</returns>
        Option<IExitResponse> CalculateLong();

        /// <summary>
        /// Runs a calculation of the exit algorithm for short position exits, and returns an exit
        /// response (optional value).
        /// </summary>
        /// <returns> A <see cref="Option{IExitResponse}"/>.</returns>
        Option<IExitResponse> CalculateShort();
    }
}