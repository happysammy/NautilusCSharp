// -------------------------------------------------------------------------------------------------
// <copyright file="IServiceFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using Akka.Actor;
    using Nautilus.BlackBox.Core.Setup;

    /// <summary>
    /// The <see cref="IServiceFactory"/> interface. An abstract factory which provides 
    /// <see cref="BlackBox"/> services.
    /// </summary>
    public interface IServiceFactory
    {
        /// <summary>
        /// Creates a <see cref="BlackBox"/> service and returns the <see cref="IActorRef"/> address
        /// from the given inputs.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <returns>A <see cref="IActorRef"/>.</returns>
        IActorRef Create(
            ActorSystem actorSystem,
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter);
    }
}