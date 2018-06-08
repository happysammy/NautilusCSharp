// -------------------------------------------------------------------------------------------------
// <copyright file="DataStatusRequest.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Messages.Queries
{
    using System;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// The data status request message.
    /// </summary>
    public sealed class DataStatusRequest : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataStatusRequest"/> message.
        /// </summary>
        /// <param name="symbolBarSpec">The message symbol bar specification.</param>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        public DataStatusRequest(
            SymbolBarSpec symbolBarSpec,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Validate.NotNull(symbolBarSpec, nameof(symbolBarSpec));

            this.SymbolBarSpec = symbolBarSpec;
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
