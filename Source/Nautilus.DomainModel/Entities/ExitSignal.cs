//--------------------------------------------------------------------------------------------------
// <copyright file="ExitSignal.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Entities
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="ExitSignal"/> class. Represents a trade exit signal.
    /// </summary>
    [Immutable]
    public sealed class ExitSignal : Signal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExitSignal"/> class.
        /// </summary>
        /// <param name="symbol">The exit signal symbol.</param>
        /// <param name="signalId">The exit signal identifier.</param>
        /// <param name="signalLabel">The exit signal label.</param>
        /// <param name="tradeType">The exit signal trade type.</param>
        /// <param name="forMarketPosition">The exit signal market position type.</param>
        /// <param name="forUnit">The exit signal unit type.</param>
        /// <param name="signalTimestamp">The exit signal timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value, or if the for market position is flat.</exception>
        public ExitSignal(
            Symbol symbol,
            EntityId signalId,
            Label signalLabel,
            TradeType tradeType,
            MarketPosition forMarketPosition,
            List<int> forUnit,
            ZonedDateTime signalTimestamp)
            : base(
                  symbol,
                  signalId,
                  signalLabel,
                  tradeType,
                  signalTimestamp)
        {
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotNull(signalId, nameof(signalId));
            Validate.NotNull(signalLabel, nameof(signalLabel));
            Validate.NotNull(tradeType, nameof(tradeType));
            Validate.NotDefault(forMarketPosition, nameof(forMarketPosition));
            Validate.NotEqualTo(forMarketPosition, nameof(forMarketPosition), MarketPosition.Flat);
            Validate.NotNull(forUnit, nameof(forUnit));
            Validate.NotDefault(signalTimestamp, nameof(signalTimestamp));

            this.ForMarketPosition = forMarketPosition;
            this.ForUnit = forUnit.ToImmutableList();
        }

        /// <summary>
        /// Gets exit signals market position type.
        /// </summary>
        public MarketPosition ForMarketPosition { get; }

        /// <summary>
        /// Gets the for unit.
        /// </summary>
        public IReadOnlyList<int> ForUnit { get; }
    }
}