// -------------------------------------------------------------------------------------------------
// <copyright file="BacktestGuidFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Backtest
{
    using System;
    using Nautilus.BlackBox.Core.Interfaces;

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
