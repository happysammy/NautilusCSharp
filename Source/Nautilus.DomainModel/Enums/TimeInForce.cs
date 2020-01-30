//--------------------------------------------------------------------------------------------------
// <copyright file="TimeInForce.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// The time in force for an order.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed")]
    public enum TimeInForce
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The order is in force for that trading day (default).
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
        GTD = 5,
    }
}
