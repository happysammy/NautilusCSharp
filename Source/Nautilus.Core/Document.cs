//--------------------------------------------------------------------------------------------------
// <copyright file="Document.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using NodaTime;

    /// <summary>
    /// The base class for all documents.
    /// </summary>
    [Immutable]
    public abstract class Document : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        /// <param name="identifier">The command identifier.</param>
        /// <param name="timestamp">The command timestamp.</param>
        protected Document(Guid identifier, ZonedDateTime timestamp)
            : base(identifier, timestamp)
        {
            Debug.NotDefault(identifier, nameof(identifier));
            Debug.NotDefault(timestamp, nameof(timestamp));
        }
    }
}
