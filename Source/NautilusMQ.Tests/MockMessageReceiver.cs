//--------------------------------------------------------------------------------------------------
// <copyright file="MockMessageReceiver.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace NautilusMQ.Tests
{
    using System;
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
        /// Register the given message type with the given handler.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="handler">The handler.</param>
        public new void RegisterHandler<T>(Action<object> handler)
        {
            base.RegisterHandler<T>(handler);
        }

        /// <summary>
        /// Add the message to the received messages list.
        /// </summary>
        /// <param name="message">The received message.</param>
        public void OnMessage(object message)
        {
            this.Messages.Add(message);
        }
    }
}
