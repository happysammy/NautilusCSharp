//--------------------------------------------------------------------------------------------------
// <copyright file="CreatePortfolio.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Messages.SystemCommands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="CreatePortfolio"/> class. Represents a service message to
    /// create a new portfolio.
    /// </summary>
    [Immutable]
    public sealed class CreatePortfolio : CommandMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePortfolio"/> class.
        /// </summary>
        /// <param name="instrument">The message instrument.</param>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public CreatePortfolio(
            Instrument instrument,
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Validate.NotNull(instrument, nameof(instrument));
            Validate.NotDefault(messageId, nameof(messageId));
            Validate.NotDefault(messageTimestamp, nameof(messageTimestamp));

            this.Instrument = instrument;
        }

        /// <summary>
        /// Gets the messages symbol.
        /// </summary>
        public Symbol Symbol => this.Instrument.Symbol;

        /// <summary>
        /// Gets the messages instrument.
        /// </summary>
        public Instrument Instrument { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="CreatePortfolio"/> service message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{base.ToString()}-{this.Symbol}";
    }
}