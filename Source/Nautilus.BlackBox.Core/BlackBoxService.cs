//--------------------------------------------------------------
// <copyright file="BlackBoxService.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Core
{
    /// <summary>
    /// The <see cref="BlackBoxService"/> context enumeration.
    /// </summary>
    public enum BlackBoxService
    {
        /// <summary>
        /// The black box alpha model service context.
        /// </summary>
        AlphaModel,

        /// <summary>
        /// The black box system core context.
        /// </summary>
        Core,

        /// <summary>
        /// The black box risk service context.
        /// </summary>
        Risk,

        /// <summary>
        /// The black box data service context.
        /// </summary>
        Data,

        /// <summary>
        /// The black box portfolio service context.
        /// </summary>
        Portfolio,

        /// <summary>
        /// The black box messaging service context.
        /// </summary>
        Messaging,

        /// <summary>
        /// The black box execution service context.
        /// </summary>
        Execution,

        /// <summary>
        /// The black box brokerage service context.
        /// </summary>
        Brokerage,

        /// <summary>
        /// The black box command bus context.
        /// </summary>
        CommandBus,

        /// <summary>
        /// The black box event bus context.
        /// </summary>
        EventBus,

        /// <summary>
        /// The black box service bus context.
        /// </summary>
        DocumentBus
    }
}