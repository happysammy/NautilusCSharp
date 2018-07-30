//--------------------------------------------------------------------------------------------------
// <copyright file="RiskServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Risk
{
    using Akka.Actor;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;

    /// <summary>
    /// Provides the risk service for the system.
    /// </summary>
    [Immutable]
    public sealed class RiskServiceFactory : IServiceFactory
    {
        /// <summary>
        /// Creates a new <see cref="RiskService"/> and returns its <see cref="IActorRef"/>
        /// actor address.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <returns>The risk service endpoint.</returns>
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
                () => new RiskService(container, messagingAdapter))));
        }
    }
}
