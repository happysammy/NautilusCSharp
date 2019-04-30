//--------------------------------------------------------------------------------------------------
// <copyright file="ActorEndpoint.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using Akka.Actor;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Provides an Akka.NET actor endpoint.
    /// </summary>
    public sealed class ActorEndpoint : IEndpoint
    {
        private readonly IActorRef actorRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorEndpoint"/> class.
        /// </summary>
        /// <param name="actorRef">The actor address.</param>
        public ActorEndpoint(IActorRef actorRef)
        {
            Validate.NotNull(actorRef, nameof(actorRef));

            this.actorRef = actorRef;
        }

        /// <summary>
        /// Sends the given message to the actor endpoint.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void Send(object message)
        {
            Debug.NotNull(message, nameof(message));

            this.actorRef.Tell(message);
        }

        /// <summary>
        /// Sends the given envelope to the actor endpoint.
        /// </summary>
        /// <param name="envelope">The envelope to send.</param>
        /// <typeparam name="T">The envelope message type.</typeparam>
        public void Send<T>(Envelope<T> envelope)
            where T : Message
        {
            Debug.NotNull(envelope, nameof(envelope));

            this.actorRef.Tell(envelope);
        }
    }
}
