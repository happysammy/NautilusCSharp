//--------------------------------------------------------------
// <copyright file="IEntryResponse.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The <see cref="IEntryResponse"/> interface. Represents the calculated signal response from
    /// an <see cref="IEntryAlgorithm"/>.
    /// </summary>
    public interface IEntryResponse
    {
        /// <summary>
        /// Gets the entry responses label.
        /// </summary>
        Label Label { get; }

        /// <summary>
        /// Gets the entry responses order side.
        /// </summary>
        OrderSide OrderSide { get; }

        /// <summary>
        /// Gets the entry responses entry price.
        /// </summary>
        Price EntryPrice { get; }

        /// <summary>
        /// Gets the entry responses time.
        /// </summary>
        ZonedDateTime Time { get; }
    }
}