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
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks.Dataflow;

    /// <summary>
    /// Provides an asynchronous message processor.
    /// </summary>
    public class MessageProcessor
    {
        private readonly ActionBlock<SendMessageTask> processor;
        private readonly Dictionary<Type, Subscription> handlers = new Dictionary<Type, Subscription>();

        private Subscription[] subscriptions = { };

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
        /// </summary>
        public MessageProcessor()
        {
            this.processor = new ActionBlock<SendMessageTask>(
                async task =>
                {
                    for (var i = 0; i < this.subscriptions.Length; i++)
                    {
                        try
                        {
                            Trace.TraceInformation($"Executing subscription '{this.subscriptions[i].Id}' handler.");
                            await this.subscriptions[i].Handler.Invoke(task.Payload);
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError($"There was a problem executing subscription '{this.subscriptions[i].Id}' handler. Exception message: {ex.Message}");
                        }
                    }
                });

            this.RegisterUnhandled(this.Unhandled);
            this.Endpoint = new Endpoint(this.processor);
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
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="handler">The handler.</param>
        public void RegisterHandler<TMessage>(Action<TMessage> handler)
        {
            var type = typeof(TMessage);
            if (this.handlers.ContainsKey(type))
            {
                throw new ArgumentException($"The internal handlers already contain a handler for {type} type messages.");
            }

            this.handlers.Add(typeof(TMessage), Subscription.Create(handler));
            this.subscriptions = this.handlers.Values.ToArray();
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

            this.handlers.Add(anyObject, Subscription.Create(handler));
            this.subscriptions = this.handlers.Values.ToArray();
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
