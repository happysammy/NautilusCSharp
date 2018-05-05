//--------------------------------------------------------------------------------------------------
// <copyright file="AlphaModelServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel
{
    using Akka.Actor;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.AlphaModel.Strategy;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// The immutable sealed <see cref="AlphaModelServiceFactory"/> class. Creates the
    /// <see cref="AlphaModelService"/> from the given inputs.
    /// </summary>
    [Immutable]
    public sealed class AlphaModelServiceFactory : IServiceFactory
    {
        /// <summary>
        /// Returns the <see cref="IActorRef"/> address of the created <see cref="AlphaModelService"/>.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <returns>A <see cref="IActorRef"/>.</returns>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public IActorRef Create(
            ActorSystem actorSystem,
            ComponentryContainer container,
            IMessagingAdapter messagingAdapter)
        {
            Validate.NotNull(actorSystem, nameof(actorSystem));
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            return actorSystem.ActorOf(Props.Create(
                () => new AlphaModelService(
                container,
                messagingAdapter,
                new AlphaStrategyModuleStore())));
        }
    }
}
