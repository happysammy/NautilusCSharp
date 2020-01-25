//--------------------------------------------------------------------------------------------------
// <copyright file="OrderPurpose.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// Represents a specified order purpose.
    /// </summary>
    public enum OrderPurpose
    {
        /// <summary>
        /// The order has no specified purpose.
        /// </summary>
        NONE = 0,

        /// <summary>
        /// The order purpose is specified as an entry.
        /// </summary>
        ENTRY = 1,

        /// <summary>
        /// The order purpose is specified as an exit.
        /// </summary>
        EXIT = 2,

        /// <summary>
        /// The order purpose is specified as a stop-loss.
        /// </summary>
        STOP_LOSS = 3,

        /// <summary>
        /// The order purpose is specified as a take_profit.
        /// </summary>
        TAKE_PROFIT = 4,
    }
}
