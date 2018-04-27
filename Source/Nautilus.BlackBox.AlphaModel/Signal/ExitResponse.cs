// -------------------------------------------------------------------------------------------------
// <copyright file="ExitResponse.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Signal
{
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="ExitResponse"/> class. Represents the calculated signal
    /// response from an <see cref="IExitAlgorithm"/>.
    /// </summary>
    [Immutable]
    internal sealed class ExitResponse : IExitResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExitResponse"/> class.
        /// </summary>
        /// <param name="label">The exit response label.</param>
        /// <param name="forMarketPosition">The exit response applicable market position.</param>
        /// <param name="forUnit">The exit response applicable trade unit.</param>
        /// <param name="time">The exit response time.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value, or if the for unit is negative.</exception>
        public ExitResponse(
            Label label,
            MarketPosition forMarketPosition,
            int forUnit,
            ZonedDateTime time)
        {
            Validate.NotNull(label, nameof(label));
            Validate.NotDefault(forMarketPosition, nameof(forMarketPosition));
            Validate.Int32NotOutOfRange(forUnit, nameof(forUnit), 0, int.MaxValue);
            Validate.NotDefault(time, nameof(time));

            this.Label = label;
            this.ForMarketPosition = forMarketPosition;
            this.ForUnit = forUnit;
            this.Time = time;
        }

        /// <summary>
        /// Gets the exit responses label.
        /// </summary>
        public Label Label { get; }

        /// <summary>
        /// Gets the exit responses applicable market position.
        /// </summary>
        public MarketPosition ForMarketPosition { get; }

        /// <summary>
        /// Gets the exit responses applicable trade unit.
        /// </summary>
        public int ForUnit { get; }

        /// <summary>
        /// Gets the exit responses time.
        /// </summary>
        public ZonedDateTime Time { get; }
    }
}
