//--------------------------------------------------------------------------------------------------
// <copyright file="CreateAlphaStrategyModule.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Messages.Commands
{
    using System;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="CreateAlphaStrategyModule"/> class. Represents a service
    /// message to create a new alpha strategy module.
    /// </summary>
    [Immutable]
    public sealed class CreateAlphaStrategyModule : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateAlphaStrategyModule"/> class.
        /// </summary>
        /// <param name="strategy">The message strategy.</param>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public CreateAlphaStrategyModule(
            IAlphaStrategy strategy,
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Validate.NotNull(strategy, nameof(strategy));
            Validate.NotDefault(messageId, nameof(messageId));
            Validate.NotDefault(messageTimestamp, nameof(messageTimestamp));

            this.Strategy = strategy;
        }

        /// <summary>
        /// Gets the messages symbol.
        /// </summary>
        public Symbol Symbol => this.Strategy.Instrument.Symbol;

        /// <summary>
        /// Gets the messages trade type.
        /// </summary>
        public TradeType TradeType => this.Strategy.TradeProfile.TradeType;

        /// <summary>
        /// Gets the messages strategy.
        /// </summary>
        public IAlphaStrategy Strategy { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="CreateAlphaStrategyModule"/> service
        /// message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{base.ToString()}-{this.Symbol}-{this.TradeType}";
    }
}
