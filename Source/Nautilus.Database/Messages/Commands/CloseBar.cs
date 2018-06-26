//--------------------------------------------------------------------------------------------------
// <copyright file="CloseBar.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Messages.Commands
{
    using System;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The command message to close the bar of the given bar specification at the given close time.
    /// </summary>
    [Immutable]
    public sealed class CloseBar : CommandMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseBar"/> class.
        /// </summary>
        /// <param name="barType">The bar type.</param>
        /// <param name="closeTime">The close time.</param>
        /// <param name="id">The close identifier.</param>
        /// <param name="timestamp">The close timestamp.</param>
        public CloseBar(
            BarSpecification barType,
            ZonedDateTime closeTime,
            Guid id,
            ZonedDateTime timestamp) : base(id, timestamp)
        {
            Debug.NotNull(barType, nameof(barType));
            Debug.NotDefault(closeTime, nameof(closeTime));

            this.BarSpecification = barType;
            this.CloseTime = closeTime;
        }

        /// <summary>
        /// Gets the messages bar specification to close.
        /// </summary>
        public BarSpecification BarSpecification { get; }

        /// <summary>
        /// Gets the messages bar closing time.
        /// </summary>
        public ZonedDateTime CloseTime { get; }
    }
}
