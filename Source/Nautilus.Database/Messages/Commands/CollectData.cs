// -------------------------------------------------------------------------------------------------
// <copyright file="CollectData.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Messages.Commands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Messaging;
    using NodaTime;

    [Immutable]
    public sealed class CollectData : CommandMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectData"/> class.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public CollectData(
            DataType dataType,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Validate.NotDefault(identifier, nameof(identifier));
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.DataType = dataType;
        }

        /// <summary>
        /// Gets the command messages data type.
        /// </summary>
        public DataType DataType { get; }
    }
}
