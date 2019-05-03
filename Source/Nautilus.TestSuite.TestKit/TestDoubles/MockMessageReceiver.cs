//--------------------------------------------------------------------------------------------------
// <copyright file="MockMessageReceiver.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Collections.Generic;
    using NautilusMQ;

    /// <summary>
    /// Provides a mock message receiver for testing.
    /// </summary>
    public class MockMessageReceiver : MessageReceiver
    {
        /// <summary>
        /// Gets the list of received messages.
        /// </summary>
        public List<object> Messages { get; } = new List<object>();

        /// <summary>
        /// Add the message to the received messages list.
        /// </summary>
        /// <param name="message">The received message.</param>
        private new void OnMessage(object message)
        {
            this.Messages.Add(message);
        }
    }
}
