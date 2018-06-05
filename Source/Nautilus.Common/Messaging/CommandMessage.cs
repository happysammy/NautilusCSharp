//--------------------------------------------------------------------------------------------------
// <copyright file="CommandMessage.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using System;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using NodaTime;

    /// <summary>
    /// The base class for all command messages.
    /// </summary>
    [Immutable]
    public abstract class CommandMessage : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMessage"/> class.
        /// </summary>
        /// <param name="id">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        protected CommandMessage(Guid id, ZonedDateTime timestamp)
            : base(id, timestamp)
        {
            Debug.NotDefault(id, nameof(id));
            Debug.NotDefault(timestamp, nameof(timestamp));
        }
    }
}
