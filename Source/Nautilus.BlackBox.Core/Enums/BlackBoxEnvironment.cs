//--------------------------------------------------------------------------------------------------
// <copyright file="BlackBoxEnvironment.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core.Enums
{
    /// <summary>
    /// The <see cref="BlackBoxEnvironment"/> enumeration.
    /// </summary>
    public enum BlackBoxEnvironment
    {
        /// <summary>
        /// A back test black box environment.
        /// </summary>
        Backtest = 0,

        /// <summary>
        /// A live black box environment.
        /// </summary>
        Live = 1,
    }
}
