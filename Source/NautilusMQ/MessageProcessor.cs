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
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using NautilusMQ.Internal;

    /// <summary>
    /// Provides an asynchronous message processor.
    /// </summary>
    public class MessageProcessor
    {
        private readonly ActionBlock<object> processor;
        private readonly List<KeyValuePair<Type, Handler>> handlers = new List<KeyValuePair<Type, Handler>>();

        private Action<object> handleAny;
        private Func<object, Task> handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
        /// </summary>
        public MessageProcessor()
        {
            this.handleAny = this.Unhandled;
            this.handle = ExpressionBuilder.Build(this.handlers.Select(h => h.Value).ToList(), this.handleAny);

            this.processor = new ActionBlock<object>(async message =>
            {
                await this.handle(message);
            });

            this.Endpoint = new Endpoint(this.processor.Post);
        }

        /// <summary>
        /// Gets the processors end point.
        /// </summary>
        public Endpoint Endpoint { get; }

        /// <summary>
        /// Gets the message input count for the processor.
        /// </summary>
        public int InputCount => this.processor.InputCount;

        /// <summary>
        /// Gets the list of messages types which can be handled.
        /// </summary>
        /// <returns>The list.</returns>
        public IEnumerable<Type> HandlerTypes => this.handlers.Select(h => h.Key).ToList().AsReadOnly();

        /// <summary>
        /// Gets the list of unhandled messages.
        /// </summary>
        public List<object> UnhandledMessages { get; } = new List<object>();

        /// <summary>
        /// Register the given message type with the gin handler.
        /// </summary>ve
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="handler">The handler.</param>
        public void RegisterHandler<TMessage>(Action<TMessage> handler)
        {
            var type = typeof(TMessage);
            if (this.handlers.Select(h => h.Key).ToList().Contains(type))
            {
                throw new ArgumentException($"The internal handlers already contain a handler for {type} type messages.");
            }

            this.handlers.Add(new KeyValuePair<Type, Handler>(type, Handler.Create(handler)));
            this.handle = ExpressionBuilder.Build(this.handlers.Select(h => h.Value).ToArray(), this.handleAny);
        }

        /// <summary>
        /// Register the given handler to receive any objects.
        /// </summary>
        /// <param name="handler">The handler delegate.</param>
        public void RegisterHandleAny(Action<object> handler)
        {
            this.handleAny = Handler.Create(handler).Handle;
            this.handle = ExpressionBuilder.Build(this.handlers.Select(h => h.Value).ToArray(), this.handleAny);
        }

        /// <summary>
        /// Register the given handler to receive unhandled messages.
        /// </summary>ve
        /// <param name="handler">The handler.</param>
        public void RegisterUnhandled(Action<object> handler)
        {
            this.RegisterHandleAny(handler);
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
