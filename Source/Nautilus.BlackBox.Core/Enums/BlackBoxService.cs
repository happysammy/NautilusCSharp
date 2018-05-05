//--------------------------------------------------------------------------------------------------
// <copyright file="BlackBoxService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Enums
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
        /// The black box execution service context.
        /// </summary>
        Execution,

        /// <summary>
        /// The black box brokerage service context.
        /// </summary>
        Brokerage,
    }
}
