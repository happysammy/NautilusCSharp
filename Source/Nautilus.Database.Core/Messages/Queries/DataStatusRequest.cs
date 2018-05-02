// -------------------------------------------------------------------------------------------------
// <copyright file="DataStatusRequest.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using NautechSystems.CSharp.Validation;
using NodaTime;

namespace Nautilus.Database.Core.Messages.Queries
{
    using Nautilus.Common.Messaging;
    using Nautilus.Database.Core.Types;
    using Nautilus.DomainModel.ValueObjects;

    public sealed class DataStatusRequest : Message
    {
        public DataStatusRequest(
            SymbolBarSpec barSpec,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Validate.NotNull(barSpec, nameof(barSpec));

            this.SymbolBarSpec = barSpec;
        }

        /// <summary>
        /// Gets the request messages <see cref="BarSpecification"/>.
        /// </summary>
        public SymbolBarSpec SymbolBarSpec { get; }

        /// <summary>
        /// Gets a string representation of the <see cref="StartSystem"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(DataStatusRequest)}-{this.Id}";
    }
}
