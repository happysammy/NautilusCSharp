//--------------------------------------------------------------------------------------------------
// <copyright file="EconomicEventFrame.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Types
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Nautilus.Core.Annotations;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using NodaTime;

    /// <summary>
    /// A container for <see cref="EconomicEvent"/>s.
    /// </summary>
    [Immutable]
    [PerformanceOptimized]
    public sealed class EconomicEventFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EconomicEventFrame"/> class.
        /// </summary>
        /// <param name="events">The list of economic events.</param>
        public EconomicEventFrame(IEnumerable<EconomicEvent> events)
        {
            this.Events = events.ToImmutableList();
        }

        /// <summary>
        /// Gets the economic event frames currency symbols.
        /// </summary>
        public IReadOnlyCollection<Currency> CurrencySymbols =>
            this.Events.Select(e => e.Currency)
                .Distinct()
                .ToList()
                .AsReadOnly();

        /// <summary>
        /// Gets the economic event frames list of events.
        /// </summary>
        public ImmutableList<EconomicEvent> Events { get; }

        /// <summary>
        /// Gets the economic event frames first event time.
        /// </summary>
        public ZonedDateTime StartDateTime => this.Events.First().Time;

        /// <summary>
        /// Gets the economic event frames last event time.
        /// </summary>
        public ZonedDateTime EndDateTime => this.Events.Last().Time;
    }
}
