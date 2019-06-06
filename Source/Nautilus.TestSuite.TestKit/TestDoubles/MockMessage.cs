//--------------------------------------------------------------------------------------------------
// <copyright file="MockMessage.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Enums;
    using NodaTime;

    /// <summary>
    /// Represents a mock message for testing.
    /// </summary>
    [Immutable]
    public class MockMessage : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockMessage"/> class.
        /// </summary>
        /// <param name="payload">The message payload.</param>
        /// <param name="id">The message identifier.</param>
        /// <param name="timestamp">The message timestamp.</param>
        public MockMessage(
            string payload,
            Guid id,
            ZonedDateTime timestamp)
            : base(MessageType.Request, typeof(MockMessage), id, timestamp)
        {
            this.Payload = payload;
        }

        /// <summary>
        /// Gets the messages payload.
        /// </summary>
        public string Payload { get; }
    }
}
