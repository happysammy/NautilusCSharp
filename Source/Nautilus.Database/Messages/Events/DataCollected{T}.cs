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
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// A message representing that all data of T has been collected.
    /// </summary>
    [Immutable]
    public sealed class DataCollected<T> : DocumentMessage
    {
        /// <summary>
        /// Initializes a new intance of the <see cref="DataCollected{T}"/> class.
        /// </summary>
        /// <param name="dataType">The message data type.</param>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        public DataCollected(
            T dataType,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotNull(dataType, nameof(dataType));
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.DataType = dataType;
        }

        /// <summary>
        /// Gets the messages data type.
        /// </summary>
        public T DataType { get; }
    }
}
