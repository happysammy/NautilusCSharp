//--------------------------------------------------------------
// <copyright file="SignalEvent.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.DomainModel.Events
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Core;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="SignalEvent"/> class. Represents and wraps all signal events
    /// produced by the system.
    /// </summary>
    [Immutable]
    public sealed class SignalEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalEvent"/> class.
        /// </summary>
        /// <param name="signal">The event signal.</param>
        /// <param name="eventId">The signal event identifier.</param>
        /// <param name="eventTimestamp">The signal event timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public SignalEvent(
            Signal signal,
            Guid eventId,
            ZonedDateTime eventTimestamp)
            : base(eventId, eventTimestamp)
        {
            Validate.NotNull(signal, nameof(signal));
            Validate.NotDefault(eventId, nameof(eventId));
            Validate.NotDefault(eventTimestamp, nameof(eventTimestamp));

            this.Signal = signal;
        }

        /// <summary>
        /// Gets the events signal symbol.
        /// </summary>
        public Symbol Symbol => this.Signal.Symbol;

        /// <summary>
        /// Gets the events signal.
        /// </summary>
        public Signal Signal { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="SignalEvent"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(SignalEvent);
    }
}
