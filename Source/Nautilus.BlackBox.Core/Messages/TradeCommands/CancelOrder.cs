//--------------------------------------------------------------------------------------------------
// <copyright file="CancelOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Messages.TradeCommands
{
    using System;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Aggregates;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="CancelOrder"/> class.
    /// </summary>
    [Immutable]
    public sealed class CancelOrder : CommandMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelOrder"/> class.
        /// </summary>
        /// <param name="order">The message order to cancel.</param>
        /// <param name="reason">The message cancellation reason.</param>
        /// <param name="messageId">The message identifier (cannot be default).</param>
        /// <param name="messageTimestamp">The message timestamp (cannot be default).</param>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if any
        /// struct argument is the default value.</exception>
        public CancelOrder(
            Order order,
            string reason,
            Guid messageId,
            ZonedDateTime messageTimestamp)
            : base(messageId, messageTimestamp)
        {
            Validate.NotNull(order, nameof(order));
            Validate.NotNull(reason, nameof(reason));
            Validate.NotDefault(messageId, nameof(messageId));
            Validate.NotDefault(messageTimestamp, nameof(messageTimestamp));

            this.Order = order;
            this.Reason = reason;
        }

        /// <summary>
        /// Gets the messages order to cancel.
        /// </summary>
        public Order Order { get; }

        /// <summary>
        /// Gets the messages cancellation reason.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="CancelOrder"/> command message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(CancelOrder)}-{this.Order}";
    }
}
