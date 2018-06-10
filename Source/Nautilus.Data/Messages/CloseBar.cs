//--------------------------------------------------------------------------------------------------
// <copyright file="CloseBar.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages
{
    using System;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The command message to close the bar of the contained specification at the contained close time.
    /// </summary>
    [Immutable]
    public class CloseBar : CommandMessage
    {
        public CloseBar(
            BarSpecification barSpec,
            Guid id,
            ZonedDateTime timestamp) : base(id, timestamp)
        {
            Debug.NotNull(barSpec, nameof(barSpec));
            this.BarSpecification = barSpec;

        }

        /// <summary>
        /// Gets the messages bar specification to close.
        /// </summary>
        public BarSpecification BarSpecification { get; }

        /// <summary>
        /// Gets the messages bar closing time.
        /// </summary>
        public ZonedDateTime CloseTime { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="SubscribeBarData"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => nameof(CloseBar);
    }
}
