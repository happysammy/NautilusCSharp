// -------------------------------------------------------------------------------------------------
// <copyright file="IExitResponse.cs" company="Nautech Systems Pty Ltd.">
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
    /// The <see cref="IExitResponse"/> interface. Represents the calculated signal response from an
    /// <see cref="IExitAlgorithm"/>.
    /// </summary>
    public interface IExitResponse
    {
        /// <summary>
        /// Gets the exit responses label.
        /// </summary>
        Label Label { get; }

        /// <summary>
        /// Gets the exit responses applicable market position.
        /// </summary>
        MarketPosition ForMarketPosition { get; }

        /// <summary>
        /// Gets the exit responses applicable trade unit.
        /// </summary>
        int ForUnit { get; }

        /// <summary>
        /// Gets the exit responses time time.
        /// </summary>
        ZonedDateTime Time { get; }
    }
}