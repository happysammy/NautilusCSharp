// -------------------------------------------------------------------------------------------------
// <copyright file="EntryResponse.cs" company="Nautech Systems Pty Ltd.">
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
    /// The immutable sealed <see cref="EntryResponse"/> class. Represents the calculated signal 
    /// response from an <see cref="IEntryAlgorithm"/>.
    /// </summary>
    [Immutable]
    internal sealed class EntryResponse : IEntryResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntryResponse"/> class.
        /// </summary>
        /// <param name="label">The entry response label.</param>
        /// <param name="orderSide">The entry response order side.</param>
        /// <param name="entryPrice">The entry response entry price.</param>
        /// <param name="time">The entry response time.</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public EntryResponse(
            Label label,
            OrderSide orderSide,
            Price entryPrice,
            ZonedDateTime time)
        {
            Validate.NotNull(label, nameof(label));
            Validate.NotDefault(orderSide, nameof(orderSide));
            Validate.NotNull(entryPrice, nameof(entryPrice));
            Validate.NotDefault(time, nameof(time));

            this.Label = label;
            this.OrderSide = orderSide;
            this.EntryPrice = entryPrice;
            this.Time = time;
        }

        /// <summary>
        /// Gets the entry responses label.
        /// </summary>
        public Label Label { get; }

        /// <summary>
        /// Gets the entry responses order side.
        /// </summary>
        public OrderSide OrderSide { get; }

        /// <summary>
        /// Gets the entry responses price.
        /// </summary>
        public Price EntryPrice { get; }

        /// <summary>
        /// Gets the entry responses time.
        /// </summary>
        public ZonedDateTime Time { get; }
    }
}