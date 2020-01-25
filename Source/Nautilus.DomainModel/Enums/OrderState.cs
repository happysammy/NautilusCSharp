//--------------------------------------------------------------------------------------------------
// <copyright file="OrderState.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// Represents the state of an order at the brokerage.
    /// </summary>
    public enum OrderState
    {
        /// <summary>
        /// The initialized order state.
        /// </summary>
        Initialized = 1,

        /// <summary>
        /// The invalid order state.
        /// </summary>
        Invalid = 2,

        /// <summary>
        /// The denied order state.
        /// </summary>
        Denied = 3,

        /// <summary>
        /// The submitted order state.
        /// </summary>
        Submitted = 4,

        /// <summary>
        /// The accepted order state.
        /// </summary>
        Accepted = 5,

        /// <summary>
        /// The rejected order state.
        /// </summary>
        Rejected = 6,

        /// <summary>
        /// The working order state.
        /// </summary>
        Working = 7,

        /// <summary>
        /// The cancelled order state.
        /// </summary>
        Cancelled = 8,

        /// <summary>
        /// Expired order state.
        /// </summary>
        Expired = 9,

        /// <summary>
        /// The over filled order state.
        /// </summary>
        OverFilled = 10,

        /// <summary>
        /// The partially filled order state.
        /// </summary>
        PartiallyFilled = 11,

        /// <summary>
        /// The completely filled order state.
        /// </summary>
        Filled = 12,
    }
}
