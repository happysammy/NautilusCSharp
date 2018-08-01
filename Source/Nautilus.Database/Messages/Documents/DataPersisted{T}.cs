// -------------------------------------------------------------------------------------------------
// <copyright file="MarketDataPersisted.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Documents
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// A message representing that all bar data has been persisted.
    /// </summary>
    [Immutable]
    public sealed class DataPersisted<T> : Document
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataPersisted{T}"/> class.
        /// </summary>
        /// <param name="dataType">The message data type.</param>
        /// <param name="lastDataTime">The message last data time.</param>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="timestamp">THe message timestamp</param>
        public DataPersisted(
            T dataType,
            ZonedDateTime lastDataTime,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotNull(dataType, nameof(dataType));
            Debug.NotDefault(lastDataTime, nameof(lastDataTime));
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));

            this.DataType = dataType;
            this.LastDataTime = lastDataTime;
        }

        /// <summary>
        /// Gets the messages symbol bar specification.
        /// </summary>
        public T DataType { get; }

        /// <summary>
        /// Gets the messages last bar time.
        /// </summary>
        public ZonedDateTime LastDataTime { get; }
    }
}
