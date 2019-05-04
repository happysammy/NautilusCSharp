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
    using System.Linq;
    using System.Threading.Tasks.Dataflow;

    /// <summary>
    /// Provides an asynchronous message processor.
    /// </summary>
    public class MessageProcessor
    {
        private readonly Dictionary<Type, Action<object>> handlers = new Dictionary<Type, Action<object>>();
        private readonly ActionBlock<object> queue;

        private Action<object> messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
        /// </summary>
        public MessageProcessor()
        {
            this.messageHandler = this.CompileHandlerExpression();
            this.queue = new ActionBlock<object>(msg => this.messageHandler.Invoke(msg));
            this.Endpoint = new Endpoint(this.queue);
        }

        /// <summary>
        /// Gets the message processors end point.
        /// </summary>
        public Endpoint Endpoint { get; }

        /// <summary>
        /// Gets the list of messages types which can be handled.
        /// </summary>
        /// <returns>The list.</returns>
        public IEnumerable<Type> HandlerTypes => this.handlers.Keys.ToList().AsReadOnly();

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
            var type = typeof(T);
            if (this.handlers.ContainsKey(type))
            {
                throw new ArgumentException($"The internal handlers already contain a handler for {type} type messages.");
            }

            this.handlers.Add(typeof(T), handler);
            this.messageHandler = this.CompileHandlerExpression();
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

        private Action<object> CompileHandlerExpression()
        {
            var anyObject = typeof(object);
            if (!this.handlers.ContainsKey(anyObject))
            {
                this.handlers.Add(anyObject, this.Unhandled);
            }

            return this.handlers[typeof(object)].Invoke;
        }
    }
}
