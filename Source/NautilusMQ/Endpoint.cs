// -------------------------------------------------------------------------------------------------
// <copyright file="Endpoint.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace NautilusMQ
{
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents a messaging endpoint.
    /// </summary>
    [Immutable]
    public class Endpoint : IEndpoint
    {
        private readonly ITargetBlock<SendMessageTask> target;

        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint"/> class.
        /// </summary>
        /// <param name="target">The data flow target block for the end point.</param>
        public Endpoint(ITargetBlock<SendMessageTask> target)
        {
            this.target = target;
        }

        /// <summary>
        /// Send the given message to the endpoint.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void Send(object message)
        {
            this.target.Post(new SendMessageTask(message, result => new TaskCompletionSource<bool>().SetResult(result)));
        }

        /// <summary>
        /// Sends the given envelope to the endpoint.
        /// </summary>
        /// <param name="envelope">The envelope to send.</param>
        /// <typeparam name="T">The envelope message type.</typeparam>
        public void Send<T>(Envelope<T> envelope)
            where T : Message
        {
            this.target.Post(new SendMessageTask(envelope, result => new TaskCompletionSource<bool>().SetResult(result)));
        }
    }
}
