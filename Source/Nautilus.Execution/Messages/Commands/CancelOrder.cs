﻿//--------------------------------------------------------------------------------------------------
// <copyright file="CancelOrder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution.Messages.Commands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.Execution.Messages.Commands.Base;
    using NodaTime;

    /// <summary>
    /// Represents a command to cancel an order.
    /// </summary>
    [Immutable]
    public sealed class CancelOrder : OrderCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelOrder"/> class.
        /// </summary>
        /// <param name="order">The commands order to cancel.</param>
        /// <param name="cancelReason">The commands cancel reason.</param>
        /// <param name="commandIdentifier">The commands identifier.</param>
        /// <param name="commandTimestamp">The commands timestamp.</param>
        public CancelOrder(
            Order order,
            string cancelReason,
            Guid commandIdentifier,
            ZonedDateTime commandTimestamp)
            : base(
                order,
                commandIdentifier,
                commandTimestamp)
        {
            Debug.NotEmptyOrWhiteSpace(cancelReason, nameof(cancelReason));
            Debug.NotDefault(commandIdentifier, nameof(commandIdentifier));
            Debug.NotDefault(commandTimestamp, nameof(commandTimestamp));

            this.Reason = cancelReason;
        }

        /// <summary>
        /// Gets the commands cancel reason.
        /// </summary>
        public string Reason { get; }
    }
}
