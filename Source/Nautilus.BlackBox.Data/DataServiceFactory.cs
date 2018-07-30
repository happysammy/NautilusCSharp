//--------------------------------------------------------------------------------------------------
// <copyright file="DataServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Data
{
    using Akka.Actor;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;

    /// <summary>
    /// Creates the <see cref="DataService"/> from the given inputs.
    /// </summary>
    [Immutable]
    public sealed class DataServiceFactory : IServiceFactory
    {
        /// <summary>
        /// Returns the <see cref="IActorRef"/> address of the created <see cref="DataService"/>.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <returns>The services endpoint.</returns>
        public IEndpoint Create(
            ActorSystem actorSystem,
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter)
        {
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            return new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(
                () => new DataService(
                    container,
                    messagingAdapter,
                    actorSystem.Scheduler))));
        }
    }
}
