// -------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessor.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    /// <summary>
    /// Provides an asynchronous message processor.
    /// </summary>
    internal class MessageProcessor
    {
        private readonly ActionBlock<object> processor;
        private readonly CancellationToken cancel = new CancellationToken(false);
        private readonly Dictionary<Type, Handler> registeredHandlers = new Dictionary<Type, Handler>();

        private Action<object> handleAny;
        private Handler[] handlers = { };
        private int handlersLength = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
        /// </summary>
        internal MessageProcessor()
        {
            this.handleAny = this.AddToUnhandledMessages;

            this.processor = new ActionBlock<object>(
                async message =>
                {
                    await this.HandleMessage(message);
                },
                new ExecutionDataflowBlockOptions { CancellationToken = this.cancel });

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
        /// Gets the types which can be handled.
        /// </summary>
        /// <returns>The list.</returns>
        public IEnumerable<Type> HandlerTypes => this.registeredHandlers.Keys.ToList().AsReadOnly();

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

            if (type == typeof(object))
            {
                throw new ArgumentException($"Cannot register handler for Object type " +
                                            $"(use .RegisterHandleAny or .RegisterUnhandled).");
            }

            if (this.registeredHandlers.ContainsKey(type))
            {
                throw new ArgumentException($"Cannot register handler " +
                                            $"(the internal handlers already contain a handler for {type} type messages).");
            }

            this.registeredHandlers[type] = Handler.Create(handler);
            this.BuildHandlers();
        }

        /// <summary>
        /// Register the given handler to receive any objects.
        /// </summary>
        /// <param name="handler">The handler delegate.</param>
        public void RegisterHandleAny(Action<object> handler)
        {
            this.handleAny = handler;
            this.BuildHandlers();
        }

        /// <summary>
        /// Register the given handler to receive unhandled messages.
        /// </summary>ve
        /// <param name="handler">The handler.</param>
        public void RegisterUnhandled(Action<object> handler)
        {
            this.RegisterHandleAny(handler);
        }

        private void BuildHandlers()
        {
            this.handlers = this.registeredHandlers.Values.ToArray();
            this.handlersLength = this.handlers.Length;
        }

        /// <summary>
        /// Handle the given message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <returns>The completed task.</returns>
        private Task HandleMessage(object message)
        {
            for (var i = 0; i < this.handlersLength; i++)
            {
                if (this.handlers[i].Handle(message))
                {
                    return Task.CompletedTask;
                }
            }

            return this.Unhandled(message);
        }

        /// <summary>
        /// Handles unhandled messages.
        /// </summary>
        /// <param name="message">The unhandled message.</param>
        private Task Unhandled(object message)
        {
            this.handleAny(message);
            return Task.CompletedTask;
        }

        /// <summary>
        /// The default handle any delegate which adds the given messages to the list of unhandled messages.
        /// </summary>
        /// <param name="message">The unhandled message.</param>
        private void AddToUnhandledMessages(object message)
        {
            this.UnhandledMessages.Add(message);
        }
    }
}
