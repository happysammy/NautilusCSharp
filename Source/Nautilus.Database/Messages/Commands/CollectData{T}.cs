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
    using Nautilus.Core;
    using NodaTime;

    /// <summary>
    /// A messages which represents the command to collect data of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
