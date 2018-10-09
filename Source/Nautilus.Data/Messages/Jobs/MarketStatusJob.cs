// -------------------------------------------------------------------------------------------------
// <copyright file="MarketStatusJob.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Messages.Jobs
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// A scheduled job to check for and potentially change market status.
    /// </summary>
    [Immutable]
    public sealed class MarketStatusJob : IScheduledJob
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketStatusJob"/> class.
        /// </summary>
        /// <param name="isMarketOpen">The is market open flag.</param>
        public MarketStatusJob(bool isMarketOpen)
        {
            this.IsMarketOpen = isMarketOpen;
        }

        /// <summary>
        /// Gets a value indicating whether the is market open.
        /// </summary>
        public bool IsMarketOpen { get; }
    }
}
