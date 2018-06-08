// -------------------------------------------------------------------------------------------------
// <copyright file="AllDataCollected.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Messages.Events
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// A message representing that all data has been collected.
    /// </summary>
    [Immutable]
    public sealed class AllDataCollected : Event
    {
        /// <summary>
        /// Initializes a new intance of the <see cref="AllDataCollected"/> class.
        /// </summary>
        /// <param name="symbolBarSpec"></param>
        /// <param name="identifier"></param>
        /// <param name="timestamp"></param>
        public AllDataCollected(
            SymbolBarSpec symbolBarSpec,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Validate.NotNull(symbolBarSpec, nameof(symbolBarSpec));
            Validate.NotDefault(identifier, nameof(identifier));
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.SymbolBarSpec = symbolBarSpec;
        }

        /// <summary>
        /// Gets the messages symbol bar specification.
        /// </summary>
        public SymbolBarSpec SymbolBarSpec { get; }

        /// <summary>
        /// Gets a string representation of the <see cref="AllDataCollected"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(AllDataCollected)}-{this.Id}";
    }
}
