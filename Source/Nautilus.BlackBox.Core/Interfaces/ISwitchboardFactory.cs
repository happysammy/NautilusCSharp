// -------------------------------------------------------------------------------------------------
// <copyright file="ISwitchboardFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Interfaces
{
    using Akka.Actor;

    /// <summary>
    /// The <see cref="ISwitchboardFactory"/> interface. An abstract factory which provides a
    /// <see cref="ISwitchboard"/> based on the given <see cref="IActorRef"/> address(s).
    /// </summary>
    public interface ISwitchboardFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="ISwitchboard"/> based on the given inputs.
        /// </summary>
        /// <param name="alphaModelServiceRef">The alpha model service ref.</param>
        /// <param name="dataServiceRef">The data service ref.</param>
        /// <param name="executionServiceRef">The execution service ref.</param>
        /// <param name="portfolioServiceRef">The portfolio service ref.</param>
        /// <param name="riskServiceRef">The risk service ref.</param>
        /// <returns>A <see cref="ISwitchboard"/>.</returns>
        ISwitchboard Create(
            IActorRef alphaModelServiceRef,
            IActorRef dataServiceRef,
            IActorRef executionServiceRef,
            IActorRef portfolioServiceRef,
            IActorRef riskServiceRef);
    }
}
