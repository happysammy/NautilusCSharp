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
    using Nautilus.Database.Core.Types;

    public class AllDataCollected : Event
    {
        public AllDataCollected(
            SymbolBarData symbolBarData,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Validate.NotNull(symbolBarData, nameof(symbolBarData));
            Validate.NotDefault(identifier, nameof(identifier));
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.SymbolBarData = symbolBarData;
        }

        public SymbolBarData SymbolBarData { get; }


        /// <summary>
        /// Gets a string representation of the <see cref="AllDataCollected"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(AllDataCollected)}-{this.Id}";
    }
}
