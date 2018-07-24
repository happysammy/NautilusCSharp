﻿//--------------------------------------------------------------------------------------------------
// <copyright file="TimeInForce.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents the time in force for an order.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum TimeInForce
    {
        /// <summary>
        /// The time in force is unknown.
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// The order is in force for that trading day.
        /// </summary>
        DAY = 1,

        /// <summary>
        /// The order is in force until cancelled.
        /// </summary>
        GTC = 2,

        /// <summary>
        /// The order is executed immediately or cancelled.
        /// </summary>
        IOC = 3,

        /// <summary>
        /// The order is filled entirely or killed.
        /// </summary>
        FOC = 4,

        /// <summary>
        /// The order is in force until the specified date and time.
        /// </summary>
        GTD = 5
    }
}
