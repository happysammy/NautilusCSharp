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
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.AlphaModel.Strategy;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;

    /// <summary>
    /// Provides the alpha model service for the system.
    /// </summary>
    [Immutable]
    public sealed class AlphaModelServiceFactory : IServiceFactory
    {
        /// <summary>
        /// Creates an <see cref="AlphaModelService"/> and returns its endpoint.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <returns>The alpha model service endpoint.</returns>
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
                () => new AlphaModelService(
                container,
                messagingAdapter,
                new AlphaStrategyModuleStore()))));
        }
    }
}
