//--------------------------------------------------------------------------------------------------
// <copyright file="CloseBar.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Commands
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Represents a command to close a <see cref="Bar"/> of the given <see cref="BarSpecification"/>.
    /// </summary>
    [Immutable]
    public sealed class CloseBar : Command, IScheduledJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseBar"/> class.
        /// </summary>
        /// <param name="barSpec">The bar specification.</param>
        /// <param name="scheduledTime">The scheduled job time.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandTimestamp">The command timestamp.</param>
        public CloseBar(
            BarSpecification barSpec,
            ZonedDateTime scheduledTime,
            Guid commandId,
            ZonedDateTime commandTimestamp)
            : base(
                typeof(CloseBar),
                commandId,
                commandTimestamp)
        {
            this.ScheduledTime = scheduledTime;
            this.BarSpecification = barSpec;
        }

        /// <summary>
        /// Gets the commands bar specification to close.
        /// </summary>
        public BarSpecification BarSpecification { get; }

        /// <inheritdoc />
        public ZonedDateTime ScheduledTime { get; }
    }
}
