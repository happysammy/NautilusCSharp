// -------------------------------------------------------------------------------------------------
// <copyright file="IMarketDataProvider.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

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