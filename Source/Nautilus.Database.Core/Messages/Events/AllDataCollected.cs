// -------------------------------------------------------------------------------------------------
// <copyright file="AllDataCollected.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Core.Messages.Events
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Validation;
    using Nautilus.Database.Core.Types;
    using NodaTime;

    public class AllDataCollected : Event
    {
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

        public SymbolBarSpec SymbolBarSpec { get; }


        /// <summary>
        /// Gets a string representation of the <see cref="AllDataCollected"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(AllDataCollected)}-{this.Id}";
    }
}
