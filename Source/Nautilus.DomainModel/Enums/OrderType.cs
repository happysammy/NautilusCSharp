//--------------------------------------------------------------------------------------------------
// <copyright file="OrderType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The order type enumeration.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum OrderType
    {
        /// <summary>
        /// An unknown order type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// A Market order type.
        /// </summary>
        MARKET = 1,

        /// <summary>
        /// A Limit order type.
        /// </summary>
        LIMIT = 2,

        /// <summary>
        /// A Stop Limit order type.
        /// </summary>
        STOP_LIMIT = 3,

        /// <summary>
        /// A Stop Market order type.
        /// </summary>
        STOP_MARKET = 4,

        /// <summary>
        /// A Market If Touched order type.
        /// </summary>
        MIT = 5,

        /// <summary>
        /// A Fill Or Kill order type.
        /// </summary>
        FOC = 6,

        /// <summary>
        /// A Immediate Or Cancel order type.
        /// </summary>
        IOC = 7
    }
}
