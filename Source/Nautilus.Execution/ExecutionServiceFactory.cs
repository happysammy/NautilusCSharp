//--------------------------------------------------------------------------------------------------
// <copyright file="ExecutionServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Execution
{
    using Akka.Actor;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Provides the <see cref="ExecutionService"/> for the system.
    /// </summary>
    [Immutable]
    public sealed class ExecutionServiceFactory
    {
        /// <summary>
        /// Creates a new <see cref="ExecutionService"/> and returns its <see cref="IActorRef"/>
        /// actor address.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <returns>The services endpoint.</returns>
        public IEndpoint Create(
            ActorSystem actorSystem,
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter)
        {
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            return new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                () => new ExecutionService(container, messagingAdapter))));
        }
    }
}
