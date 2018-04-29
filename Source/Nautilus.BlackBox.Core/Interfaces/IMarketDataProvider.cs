//--------------------------------------------------------------------------------------------------
// <copyright file="IMarketDataProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The <see cref="IMarketDataProvider"/> interface.
    /// </summary>
    public interface IMarketDataProvider
    {
        /// <summary>
        /// Gets the market data providers last quote.
        /// </summary>
        Tick LastQuote { get; }

        /// <summary>
        /// Gets the market data providers average spread.
        /// </summary>
        decimal AverageSpread { get; }
    }
}