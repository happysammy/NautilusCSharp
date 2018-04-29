//--------------------------------------------------------------------------------------------------
// <copyright file="BacktestGuidFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Backtest
{
    using System;
    using Nautilus.Common.Interfaces;

    /// <summary>
    /// The back test <see cref="Guid"/> factory.
    /// </summary>
    public class BacktestGuidFactory : IGuidFactory
    {
        private readonly Guid stubGuid = Guid.NewGuid();

        /// <summary>
        /// Returns a new <see cref="Guid"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="Guid"/>.
        /// </returns>
        public Guid NewGuid()
        {
            return this.stubGuid;
        }
    }
}
