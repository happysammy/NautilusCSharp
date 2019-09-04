//--------------------------------------------------------------------------------------------------
// <copyright file="OrderStatus.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// The status of an order at the brokerage.
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// The unknown order status (invalid value).
        /// </summary>
        [InvalidValue]
        Unknown = 0,

        /// <summary>
        /// Initialized order status.
        /// </summary>
        Initialized = 1,

        /// <summary>
        /// The denied order status.
        /// </summary>
        Denied = 2,

        /// <summary>
        /// The invalid order status.
        /// </summary>
        Invalid = 3,

        /// <summary>
        /// The submitted order status.
        /// </summary>
        Submitted = 4,

        /// <summary>
        /// The accepted order status.
        /// </summary>
        Accepted = 5,

        /// <summary>
        /// The rejected order status.
        /// </summary>
        Rejected = 6,

        /// <summary>
        /// The working order status.
        /// </summary>
        Working = 7,

        /// <summary>
        /// The cancelled order status.
        /// </summary>
        Cancelled = 8,

        /// <summary>
        /// Expired order status.
        /// </summary>
        Expired = 9,

        /// <summary>
        /// The over filled order status.
        /// </summary>
        OverFilled = 10,

        /// <summary>
        /// The partially filled order status.
        /// </summary>
        PartiallyFilled = 11,

        /// <summary>
        /// The completely filled order status.
        /// </summary>
        Filled = 12,
    }
}
