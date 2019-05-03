// -------------------------------------------------------------------------------------------------
// <copyright file="Endpoint.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace NautilusMQ
{
    using System.Threading.Tasks.Dataflow;
    using Nautilus.Core;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents a messaging endpoint.
    /// </summary>
    [Immutable]
    public class Endpoint : IEndpoint
    {
        private readonly ActionBlock<object> actionBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint"/> class.
        /// </summary>
        /// <param name="actionBlock">The data flow action block for the end point.</param>
        public Endpoint(ActionBlock<object> actionBlock)
        {
            this.actionBlock = actionBlock;
        }

        /// <summary>
        /// Send the given message to the <see cref="Endpoint"/> for processing.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void Send(object message)
        {
            this.actionBlock.Post(message);
        }

        /// <summary>
        /// Sends the given envelope to the actor endpoint.
        /// </summary>
        /// <param name="envelope">The envelope to send.</param>
        /// <typeparam name="T">The envelope message type.</typeparam>
        public void Send<T>(Envelope<T> envelope)
            where T : Message
        {
            this.actionBlock.Post(envelope);
        }
    }
}
