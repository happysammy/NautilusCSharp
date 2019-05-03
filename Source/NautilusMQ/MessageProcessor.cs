// -------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessor.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace NautilusMQ
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks.Dataflow;

    /// <summary>
    /// Provides an asynchronous message processor.
    /// </summary>
    public class MessageProcessor
    {
        private readonly Dictionary<Type, Action<object>> handlers = new Dictionary<Type, Action<object>>();
        private readonly ActionBlock<object> queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
        /// </summary>
        public MessageProcessor()
        {
            this.queue = new ActionBlock<object>(this.HandleMessage);
            this.Endpoint = new Endpoint(this.queue);
        }

        /// <summary>
        /// Gets the message processors end point.
        /// </summary>
        public Endpoint Endpoint { get; }

        /// <summary>
        /// Gets the list of unhandled messages.
        /// </summary>
        public List<object> UnhandledMessages { get; } = new List<object>();

        /// <summary>
        /// Gets the current message input count.
        /// </summary>
        public int InputCount => this.queue.InputCount;

        /// <summary>
        /// Register the given message type with the given handler.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="handler">The handler.</param>
        public void RegisterHandler<T>(Action<object> handler)
        {
            this.handlers.Add(typeof(T), handler);
        }

        /// <summary>
        /// Handles unhandled messages.
        /// </summary>
        /// <param name="message">The unhandled message.</param>
        private void Unhandled(object message)
        {
            this.UnhandledMessages.Add(message);
        }

        /// <summary>
        /// Handle the given message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        private void HandleMessage(object message)
        {
            try
            {
                this.handlers[message.GetType()](message);
            }
            catch (KeyNotFoundException)
            {
                this.Unhandled(message);
            }
        }
    }
}
