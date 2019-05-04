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
            this.Endpoint = this.processor.Endpoint;
        }

        /// <summary>
        /// Gets the consumers end point.
        /// </summary>
        public Endpoint Endpoint { get; }

        /// <summary>
        /// Gets the message handler types.
        /// </summary>
        public IEnumerable<Type> HandlerTypes => this.processor.HandlerTypes;

        /// <summary>
        /// Register the given message type with the given handler.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="handler">The handler.</param>
        protected void RegisterHandler<T>(Action<object> handler)
        {
            this.processor.RegisterHandler<T>(handler);
        }
    }
}
