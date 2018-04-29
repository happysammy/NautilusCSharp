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
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// The immutable sealed <see cref="DataServiceFactory"/> class. Creates the
    /// <see cref="DataService"/> from the given inputs.
    /// </summary>
    [Immutable]
    public sealed class DataServiceFactory : IServiceFactory
    {
        /// <summary>
        /// Returns the <see cref="IActorRef"/> address of the created <see cref="DataService"/>.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <returns>A <see cref="IActorRef"/> address.</returns>
        /// <exception cref="ValidationException">Throws if the validation fails.</exception>
        public IActorRef Create(
            ActorSystem actorSystem,
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter)
        {
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            return actorSystem.ActorOf(Props.Create(
                () => new DataService(
                    setupContainer,
                    messagingAdapter)));
        }
    }
}
