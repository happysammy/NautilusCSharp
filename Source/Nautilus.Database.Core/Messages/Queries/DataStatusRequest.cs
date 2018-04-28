// -------------------------------------------------------------------------------------------------
// <copyright file="DataStatusRequest.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using NautechSystems.CSharp.Validation;
using NautilusDB.Core.Types;
using NautilusDB.Messaging.Base;
using NodaTime;

namespace NautilusDB.Messaging.Queries
{
    public sealed class DataStatusRequest : Message
    {
        public DataStatusRequest(
            BarSpecification barSpec,
            Guid identifier, 
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Validate.NotNull(barSpec, nameof(barSpec));

            this.BarSpecification = barSpec;
        }

        /// <summary>
        /// Gets the request messages <see cref="BarSpecification"/>.
        /// </summary>
        public BarSpecification BarSpecification { get; }

        /// <summary>
        /// Gets a string representation of the <see cref="StartSystem"/> message.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => $"{nameof(DataStatusRequest)}-{this.Identifier}";
    }
}