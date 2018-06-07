//--------------------------------------------------------------------------------------------------
// <copyright file="EconomicNewsEventFrame.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Nautilus.Core.Annotations;
using Nautilus.Core.Validation;
using NodaTime;

namespace Nautilus.Database.Types
{
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;

    [Immutable]
    public class EconomicEventFrame
    {
        public EconomicEventFrame(IReadOnlyCollection<EconomicEvent> events)
        {
            Validate.ReadOnlyCollectionNotNullOrEmpty(events, nameof(events));

            this.Events = events;
        }

        public IReadOnlyCollection<CurrencyCode> CurrencySymbols =>
            this.Events.Select(e => e.Currency)
                .Distinct()
                .ToList()
                .AsReadOnly();

        /// <summary>
        /// Gets the economic news event frames list of events.
        /// </summary>
        public IReadOnlyCollection<EconomicEvent> Events { get; }

        /// <summary>
        /// Gets the economic news event frames first event time.
        /// </summary>
        public ZonedDateTime StartDateTime => this.Events.First().Time;

        /// <summary>
        /// Gets the economic news event frames last event time.
        /// </summary>
        public ZonedDateTime EndDateTime => this.Events.Last().Time;
    }
}
