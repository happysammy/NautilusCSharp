//--------------------------------------------------------------------------------------------------
// <copyright file="Signal.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Entities
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The base class for all signal types.
    /// </summary>
    [Immutable]
    public abstract class Signal : Entity<Signal>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Signal"/> class.
        /// </summary>
        /// <param name="symbol">The signal symbol.</param>
        /// <param name="signalId">The signal identifier.</param>
        /// <param name="signalLabel">The signal label.</param>
        /// <param name="tradeType">The signal trade type.</param>
        /// <param name="signalTimestamp">The signal timestamp.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        protected Signal(
            Symbol symbol,
            SignalId signalId,
            Label signalLabel,
            TradeType tradeType,
            ZonedDateTime signalTimestamp)
            : base(
                  signalId,
                  signalTimestamp)
        {
            Debug.NotNull(symbol, nameof(symbol));
            Debug.NotNull(signalId, nameof(signalId));
            Debug.NotNull(signalLabel, nameof(signalLabel));
            Debug.NotNull(tradeType, nameof(tradeType));
            Debug.NotDefault(signalTimestamp, nameof(signalTimestamp));

            this.Symbol = symbol;
            this.SignalLabel = signalLabel;
            this.TradeType = tradeType;
        }

        /// <summary>
        /// Gets the signals symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the signals label.
        /// </summary>
        public Label SignalLabel { get; }

        /// <summary>
        /// Gets the signals trade type.
        /// </summary>
        public TradeType TradeType { get; }

        /// <summary>
        /// Gets the signals timestamp.
        /// </summary>
        public ZonedDateTime SignalTimestamp => this.Timestamp;

        /// <summary>
        /// Returns a string representation of the <see cref="Signal"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.SignalLabel.Value;
    }
}
