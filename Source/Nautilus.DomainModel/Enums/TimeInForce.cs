// -------------------------------------------------------------------------------------------------
// <copyright file="TimeInForce.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The time in force.
    /// </summary>
    public enum TimeInForce
    {
        /// <summary>
        /// Order has an unknown time in force.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Order in force for that trading day.
        /// </summary>
        DAY = 1,

        /// <summary>
        /// Order in force until cancelled.
        /// </summary>
        GTC = 2,

        /// <summary>
        /// Order is executed immediately or cancelled.
        /// </summary>
        IOC = 3,

        /// <summary>
        /// Order is filled or killed.
        /// </summary>
        FOC = 4,

        /// <summary>
        /// Order in force until specified date and time.
        /// </summary>
        GTD = 5
    }
}