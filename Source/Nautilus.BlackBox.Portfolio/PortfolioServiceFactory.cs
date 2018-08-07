//--------------------------------------------------------------------------------------------------
// <copyright file="PortfolioServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio
{
    using Akka.Actor;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Provides the portfolio service for the system.
    /// </summary>
    [Immutable]
    public class PortfolioServiceFactory : IServiceFactory
    {
        /// <summary>
        /// Creates a new <see cref="PortfolioService"/> and returns its <see cref="IActorRef"/>
        /// address.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging Adapter.</param>
        /// <returns>The portfolio services endpoint.</returns>
        public IEndpoint Create(
            ActorSystem actorSystem,
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter)
        {
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            var portfolioStore = new SecurityPortfolioStore();
            return new ActorEndpoint(
                actorSystem.ActorOf(Props.Create(() => new PortfolioService(
                container,
                messagingAdapter,
                portfolioStore))));
        }
    }
}
