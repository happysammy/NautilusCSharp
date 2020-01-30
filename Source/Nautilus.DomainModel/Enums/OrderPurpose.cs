//--------------------------------------------------------------------------------------------------
// <copyright file="OrderPurpose.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents a specified order purpose.
    /// </summary>
    public enum OrderPurpose
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The order has no specified purpose (default).
        /// </summary>
        None = 1,

        /// <summary>
        /// The order purpose is specified as an entry.
        /// </summary>
        Entry = 2,

        /// <summary>
        /// The order purpose is specified as an exit.
        /// </summary>
        Exit = 3,

        /// <summary>
        /// The order purpose is specified as a stop-loss.
        /// </summary>
        StopLoss = 4,

        /// <summary>
        /// The order purpose is specified as a take_profit.
        /// </summary>
        TakeProfit = 5,
    }
}
