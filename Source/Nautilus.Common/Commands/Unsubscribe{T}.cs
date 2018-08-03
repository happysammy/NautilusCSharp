//--------------------------------------------------------------------------------------------------
// <copyright file="Unsubscribe{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Commands
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// A system command to unsubscribe from data of type T.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    [Immutable]
    public sealed class Unsubscribe<T> : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Unsubscribe{T}"/> class.
        /// </summary>
        /// <param name="dataType">The commands symbol.</param>
        /// <param name="identifier">The commands identifier (cannot be default).</param>
        /// <param name="timestamp">The commands timestamp (cannot be default).</param>
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
