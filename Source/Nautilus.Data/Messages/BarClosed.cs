//--------------------------------------------------------------------------------------------------
// <copyright file="BarClosedEvent.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Validation;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The event where a trade bar was closed.
    /// </summary>
    [Immutable]
    public sealed class BarClosed : Event
    {
        public BarClosed(
            Symbol symbol,
            BarSpecification barSpecification,
            Bar bar,
            Tick lastTick,
            Guid id) : base(id, bar.Timestamp)
        {
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotNull(barSpecification, nameof(barSpecification));
            Debug.NotNull(bar, nameof(bar));
            Debug.NotNull(lastTick, nameof(lastTick));
            Debug.NotDefault(id, nameof(id));

            this.Symbol = symbol;
            this.BarSpecification = barSpecification;
            this.Bar = bar;
            this.LastTick = lastTick;
        }

        /// <summary>
        /// Gets the messages symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the messages bar specification.
        /// </summary>
        public BarSpecification BarSpecification { get; }

        /// <summary>
        /// Gets the messages bar.
        /// </summary>
        public Bar Bar { get; }

        /// <summary>
        /// Gets the last tick at bar close.
        /// </summary>
        public Tick LastTick { get; }
    }
}
