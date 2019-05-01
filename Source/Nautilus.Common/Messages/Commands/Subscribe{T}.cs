//--------------------------------------------------------------------------------------------------
// <copyright file="Subscribe{T}.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messages.Commands
{
    using System;
    using Nautilus.Common.Messages.Commands.Base;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using NodaTime;

    /// <summary>
    /// A system command to subscribe to data of type T.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    [Immutable]
    public sealed class Subscribe<T> : SystemCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subscribe{T}"/> class.
        /// </summary>
        /// <param name="dataType">The message symbol.</param>
        /// <param name="identifier">The message identifier (cannot be default).</param>
        /// <param name="timestamp">The message timestamp (cannot be default).</param>
        public Subscribe(
            T dataType,
            Guid identifier,
            ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
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
