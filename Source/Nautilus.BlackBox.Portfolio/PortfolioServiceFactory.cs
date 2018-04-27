// -------------------------------------------------------------------------------------------------
// <copyright file="PortfolioServiceFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio
{
    using Akka.Actor;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Setup;

    /// <summary>
    /// The immutable <see cref="PortfolioServiceFactory"/> class. Provides the
    /// <see cref="PortfolioService"/> for the <see cref="BlackBox"/> system.
    /// </summary>
    [Immutable]
    public class PortfolioServiceFactory : IServiceFactory
    {
        /// <summary>
        /// Creates a new <see cref="PortfolioService"/> and returns its <see cref="IActorRef"/>
        /// address.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging Adapter.</param>
        /// <returns>A <see cref="IActorRef"/>.</returns>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public IActorRef Create(
            ActorSystem actorSystem,
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter)
        {
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            var portfolioStore = new SecurityPortfolioStore();
            return actorSystem.ActorOf(Props.Create(() => new PortfolioService(
                setupContainer,
                messagingAdapter,
                portfolioStore)));
        }
    }
}
