// -------------------------------------------------------------------------------------------------
// <copyright file="CollectData{T}.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents the command to collect data of type T.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    [Immutable]
    public sealed class CollectData<T> : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectData{T}"/> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public CollectData(
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));
        }

        /// <summary>
        /// Gets the messages data type.
        /// </summary>
        public Type DataType => typeof(T);
    }
}
