//--------------------------------------------------------------------------------------------------
// <copyright file="OrderPurpose.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// Represents an order purpose.
    /// </summary>
    public enum OrderPurpose
    {
        /// <summary>
        /// The order has no specified purpose.
        /// </summary>
        NONE = 0,

        /// <summary>
        /// The order purpose is specified as entry.
        /// </summary>
        ENTRY = 1,

        /// <summary>
        /// The order purpose is specified as exit.
        /// </summary>
        EXIT = 2,

        /// <summary>
        /// The order purpose is specified as stop-loss.
        /// </summary>
        STOP_LOSS = 3,

        /// <summary>
        /// The order purpose is specified as take_profit.
        /// </summary>
        TAKE_PROFIT = 4,
    }
}
