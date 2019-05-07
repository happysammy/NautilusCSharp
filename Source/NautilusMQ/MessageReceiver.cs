// -------------------------------------------------------------------------------------------------
// <copyright file="MessageReceiver.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace NautilusMQ
{
    using System;
    using System.Collections.Generic;
    using NautilusMQ.Internal;

    /// <summary>
    /// The abstract base class for all message consumers.
    /// </summary>
    public abstract class MessageReceiver
    {
        private readonly MessageProcessor processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceiver"/> class.
        /// </summary>
        protected MessageReceiver()
        {
            this.processor = new MessageProcessor();
        }

        /// <summary>
        /// Gets the consumers end point.
        /// </summary>
        public Endpoint Endpoint => this.processor.Endpoint;

        /// <summary>
        /// Gets the message input count for the processor.
        /// </summary>
        public int InputCount => this.processor.InputCount;

        /// <summary>
        /// Gets the message handler types.
        /// </summary>
        public IEnumerable<Type> HandlerTypes => this.processor.HandlerTypes;

        /// <summary>
        /// Gets the unhandled messages.
        /// </summary>
        public IEnumerable<object> UnhandledMessages => this.processor.UnhandledMessages;

        /// <summary>
        /// Register the given message type with the given handler.
        /// </summary>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <param name="handler">The handler.</param>
        public void RegisterHandler<TMessage>(Action<TMessage> handler)
        {
            this.processor.RegisterHandler(handler);
        }

        /// <summary>
        /// Register the given handler to receive any message type.
        /// </summary>ve
        /// <param name="handler">The handler.</param>
        public void RegisterHandleAny(Action<object> handler)
        {
            this.processor.RegisterHandleAny(handler);
        }

        /// <summary>
        /// Register the given handler to receive unhandled messaged.
        /// </summary>ve
        /// <param name="handler">The handler.</param>
        public void RegisterUnhandled(Action<object> handler)
        {
            this.processor.RegisterUnhandled(handler);
        }
    }
}
