//--------------------------------------------------------------------------------------------------
// <copyright file="UnsubscribeSymbolDataType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Commands
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Core;
    using NodaTime;

    /// <summary>
    /// The command message to unsubscribe from data of type T.
    /// </summary>
    [Immutable]
    public sealed class Unsubscribe<T> : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Unsubscribe{T}"/> class.
        /// </summary>
        /// <param name="dataType">The message symbol.</param>
        /// <param name="identifier">The message identifier (cannot be default).</param>
        /// <param name="timestamp">The message timestamp (cannot be default).</param>
        public Unsubscribe(
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
        /// Gets the messages data type T.
        /// </summary>
        public T DataType { get; }
    }
}
