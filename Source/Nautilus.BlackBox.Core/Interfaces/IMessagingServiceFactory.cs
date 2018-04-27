// -------------------------------------------------------------------------------------------------
// <copyright file="IMessagingServiceFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using Akka.Actor;
    using Nautilus.BlackBox.Core.Enums;

    /// <summary>
    /// The <see cref="IMessagingServiceFactory"/> interface.
    /// </summary>
    public interface IMessagingServiceFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="IMessagingAdapter"/> from the given inputs.
        /// </summary>
        /// <param name="actorSystem">The actor system.</param>
        /// <param name="environment">The black box environment.</param>
        /// <param name="clock">The black box system clock.</param>
        /// <param name="loggerFactory">The logging adapter.</param>
        /// <returns>A <see cref="IMessagingAdapter"/>.</returns>
        IMessagingAdapter Create(
            ActorSystem actorSystem,
            BlackBoxEnvironment environment,
            IZonedClock clock,
            ILoggerFactory loggerFactory);
    }
}