//--------------------------------------------------------------------------------------------------
// <copyright file="NautilusService.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Enums
{
    /// <summary>
    /// Represents the name of the Nautilus service context.
    /// </summary>
    public enum NautilusService
    {
        /// <summary>
        /// The system core service context.
        /// </summary>
        Core,

        /// <summary>
        /// The messaging service context.
        /// </summary>
        Messaging,

        /// <summary>
        /// The alpha model service context.
        /// </summary>
        AlphaModel,

        /// <summary>
        /// The black box core service context.
        /// </summary>
        BlackBox,

        /// <summary>
        /// The risk service context.
        /// </summary>
        Risk,

        /// <summary>
        /// The portfolio service context.
        /// </summary>
        Portfolio,

        /// <summary>
        /// The execution service context.
        /// </summary>
        Execution,

        /// <summary>
        /// The brokerage service context.
        /// </summary>
        Brokerage,

        /// <summary>
        /// The FIX service context.
        /// </summary>
        // ReSharper disable once InconsistentNaming (FIX is the name).
        FIX,

        /// <summary>
        /// The data service context.
        /// </summary>
        Data,

        /// <summary>
        /// The scheduler service context.
        /// </summary>
        Scheduler,

        /// <summary>
        /// The database task manager service context.
        /// </summary>
        DatabaseTaskManager,

        /// <summary>
        /// The data collection manager service context.
        /// </summary>
        DataCollectionManager,

        /// <summary>
        /// The bar aggregation controller service context.
        /// </summary>
        BarAggregationController,

        /// <summary>
        /// The tick publisher service context.
        /// </summary>
        TickPublisher,

        /// <summary>
        /// The bar publisher service context.
        /// </summary>
        BarPublisher,

        /// <summary>
        /// The ASP.NET core host service context.
        /// </summary>
        AspCoreHost,
    }
}
