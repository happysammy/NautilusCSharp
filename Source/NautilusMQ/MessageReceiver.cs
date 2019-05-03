// -------------------------------------------------------------------------------------------------
// <copyright file="MessageReceiver.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace NautilusMQ
{
    /// <summary>
    /// The base class for all message consumers.
    /// </summary>
    public abstract class MessageReceiver
    {
        private readonly MessageProcessor processor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceiver"/> class.
        /// </summary>
        public MessageReceiver()
        {
            this.processor = new MessageProcessor(this);
            this.Endpoint = this.processor.Endpoint;
        }

        /// <summary>
        /// Gets the consumers end point.
        /// </summary>
        public Endpoint Endpoint { get; }

        /// <summary>
        /// TBA.
        /// </summary>
        /// <param name="message">The received message.</param>
        public void OnMessage(object message)
        {
            // Handle unknown object.
        }

        /// <summary>
        /// TBA.
        /// </summary>
        /// <param name="message">The unhandled message.</param>
        protected void Unhandled(object message)
        {
            // Handle unknown object.
        }
    }
}
