// -------------------------------------------------------------------------------------------------
// <copyright file="AllDataCollected.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Core.Messages.Events
{
    using System;
    using NautechSystems.CSharp.Validation;
    using NodaTime;
    using Nautilus.Core;
    using Nautilus.DomainModel.ValueObjects;

    public class AllDataCollected : Event
    {
        public AllDataCollected(
            BarSpecification barSpec,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Validate.NotNull(barSpec, nameof(barSpec));
            Validate.NotDefault(identifier, nameof(identifier));
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.BarSpecification = barSpec;
        }

        public BarSpecification BarSpecification { get; }


        /// <summary>
        /// Gets a string representation of the <see cref="AllDataCollected"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(AllDataCollected)}-{this.Id}";
    }
}
