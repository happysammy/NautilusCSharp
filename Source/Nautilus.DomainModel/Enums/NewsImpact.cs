//--------------------------------------------------------------------------------------------------
// <copyright file="NewsImpact.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// The relative historic impact on a financial market of a particular news event type.
    /// </summary>
    public enum NewsImpact
    {
        /// <summary>
        /// The low news impact.
        /// </summary>
        Low = 0,

        /// <summary>
        /// The medium news impact.
        /// </summary>
        Medium = 1,

        /// <summary>
        /// The high news impact.
        /// </summary>
        High = 2
    }
}
