// -------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessor.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace NautilusMQ
{
    using System.Threading.Tasks.Dataflow;

    /// <summary>
    /// Provides an asynchronous message processor.
    /// </summary>
    public class MessageProcessor
    {
        private readonly MessageReceiver receiver;
        private readonly ActionBlock<object> queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
        /// </summary>
        /// <param name="receiver">The consumer to send messages to.</param>
        public MessageProcessor(MessageReceiver receiver)
        {
            this.receiver = receiver;
            this.queue = new ActionBlock<object>(this.receiver.OnMessage);
            this.Endpoint = new Endpoint(this.queue);
        }

        /// <summary>
        /// Gets the message processors end point.
        /// </summary>
        public Endpoint Endpoint { get; }
    }
}
