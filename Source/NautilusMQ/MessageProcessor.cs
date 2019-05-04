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
        private readonly BroadcastBlock<object> buffer = new BroadcastBlock<object>(message => message);
        private readonly Dictionary<Type, Action<object>> handlers = new Dictionary<Type, Action<object>>();
        private readonly List<IDisposable> registrations = new List<IDisposable>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
        /// </summary>
        public MessageProcessor()
        {
            this.Endpoint = new Endpoint(this.buffer);
            this.RegisterUnhandled(this.Unhandled);
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
        /// Register the given message type with the gin handler.
        /// </summary>ve
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
            this.Register<T>();
        }

        /// <summary>
        /// Register the given handler to receive unhandled messaged.
        /// </summary>ve
        /// <param name="handler">The handler.</param>
        public void RegisterUnhandled(Action<object> handler)
        {
            var anyObject = typeof(object);
            if (this.handlers.ContainsKey(anyObject))
            {
                this.handlers.Remove(anyObject);
            }

            this.handlers.Add(anyObject, handler);

            var sink = new ActionBlock<object>(
                this.handlers[anyObject]);

            var registration = this.buffer.LinkTo(
                sink,
                new DataflowLinkOptions { PropagateCompletion = true });

            this.registrations.Add(registration);
        }

        private void Register<T>()
        {
            var type = typeof(T);
            var sink = new ActionBlock<object>(
                this.handlers[type]);

            var registration = this.buffer.LinkTo(
                sink,
                new DataflowLinkOptions { PropagateCompletion = true },
                message => message is T);

            this.registrations.Add(registration);
        }

        /// <summary>
        /// Handles unhandled messages.
        /// </summary>
        /// <param name="message">The unhandled message.</param>
        private void Unhandled(object message)
        {
            this.UnhandledMessages.Add(message);
        }
    }
}
