//--------------------------------------------------------------------------------------------------
// <copyright file="OrderStatus.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The order status enumeration.
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Unknown order status.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Initialized order status.
        /// </summary>
        Initialized = 1,

        /// <summary>
        /// Submitted order status.
        /// </summary>
        Submitted = 2,

        /// <summary>
        /// Accepted order status.
        /// </summary>
        Accepted = 3,

        /// <summary>
        /// Rejected order status.
        /// </summary>
        Rejected = 4,

        /// <summary>
        /// Working order status.
        /// </summary>
        Working = 5,

        /// <summary>
        /// Cancelled order status.
        /// </summary>
        Cancelled = 6,

        /// <summary>
        /// Over filled order status.
        /// </summary>
        OverFilled = 7,

        /// <summary>
        /// Partially filled order status.
        /// </summary>
        PartiallyFilled = 8,

        /// <summary>
        /// Completely filled order status.
        /// </summary>
        Filled = 9,

        /// <summary>
        /// Expired order status.
        /// </summary>
        Expired = 10,
    }
}
