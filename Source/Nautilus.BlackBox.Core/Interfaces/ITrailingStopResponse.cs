// -------------------------------------------------------------------------------------------------
// <copyright file="ITrailingStopResponse.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The <see cref="ITrailingStopResponse"/> interface. Represents the calculated signal
    /// response from an <see cref="ITrailingStopAlgorithm"/>.
    /// </summary>
    public interface ITrailingStopResponse
    {
        /// <summary>
        /// Gets the trailing stop responses label.
        /// </summary>
        Label Label { get; }

        /// <summary>
        /// Gets the trailing stop responses for market position.
        /// </summary>
        MarketPosition ForMarketPosition { get; }

        /// <summary>
        /// Gets the trailing stop responses applicable trade unit.
        /// </summary>
        int ForUnit { get; }

        /// <summary>
        /// Gets the trailing stop responses new stop-loss price.
        /// </summary>
        Price StopLossPrice { get; }

        /// <summary>
        /// Gets the trailing stop responses time.
        /// </summary>
        ZonedDateTime Time { get; }
    }
}
