// -------------------------------------------------------------------------------------------------
// <copyright file="DataStatusRequest{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using NodaTime;

    /// <summary>
    /// The data status request message.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    [Immutable]
    public sealed class DataStatusRequest<T> : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataStatusRequest{T}"/> class.
        /// </summary>
        /// <param name="dataType">The message symbol bar specification.</param>
        /// <param name="identifier">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        public DataStatusRequest(
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
        /// Gets the request messages data type..
        /// </summary>
        public T DataType { get; }
    }
}
