//--------------------------------------------------------------------------------------------------
// <copyright file="BlackBoxService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Enums
{
    /// <summary>
    /// The <see cref="NautilusService"/> context enumeration.
    /// </summary>
    public enum NautilusService
    {
        /// <summary>
        /// The alpha model Nautilus service context.
        /// </summary>
        Messaging,

        /// <summary>
        /// The alpha model Nautilus service context.
        /// </summary>
        AlphaModel,

        /// <summary>
        /// The black box core Nautilus service context.
        /// </summary>
        BlackBox,

        /// <summary>
        /// The risk Nautilus service context.
        /// </summary>
        Risk,

        /// <summary>
        /// The portfolio Nautilus service context.
        /// </summary>
        Portfolio,

        /// <summary>
        /// The execution Nautilus service context.
        /// </summary>
        Execution,

        /// <summary>
        /// The brokerage Nautilus service context.
        /// </summary>
        Brokerage,

        /// <summary>
        /// The FIX service context.
        /// </summary>
        FIX,

        /// <summary>
        /// The data Nautilus service context.
        /// </summary>
        Data,

        /// <summary>
        /// The core database service context.
        /// </summary>
        Scheduler,

        /// <summary>
        /// The task manager database service context.
        /// </summary>
        DatabaseTaskManager,

        /// <summary>
        /// The collection manager database service context.
        /// </summary>
        DataCollectionManager,

        /// <summary>
        /// The bar aggregation controller database service context.
        /// </summary>
        BarAggregationController,

        /// <summary>
        /// The tick publisher database service context.
        /// </summary>
        TickPublisher,

        /// <summary>
        /// The bar publisher database service context.
        /// </summary>
        BarPublisher,

        /// <summary>
        /// The ASP.NET core host service context.
        /// </summary>
        AspCoreHost,
    }
}
