//--------------------------------------------------------------------------------------------------
// <copyright file="Query.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using NodaTime;

    /// <summary>
    /// The base class for all <see cref="Query"/> messages.
    /// </summary>
    [Immutable]
    public abstract class Query : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        /// <param name="type">The query type.</param>
        /// <param name="id">The query identifier.</param>
        /// <param name="timestamp">The query timestamp.</param>
        protected Query(
            Type type,
            Guid id,
            ZonedDateTime timestamp)
            : base(
                type,
                id,
                timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));
        }
    }
}
