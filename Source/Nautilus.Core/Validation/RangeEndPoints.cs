//--------------------------------------------------------------------------------------------------
// <copyright file="RangeEndPoints.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the Apache 2.0 license
//  as found in the LICENSE.txt file.
//  https://github.com/nautechsystems/Nautilus.Core
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Validation
{
    /// <summary>
    /// The defined literal end points of a range.
    /// </summary>
    public enum RangeEndPoints
    {
        /// <summary>
        /// The range is inclusive of the end points (default).
        /// </summary>
        Inclusive = 0,

        /// <summary>
        /// The range is exclusive of the lower end point.
        /// </summary>
        LowerExclusive = 1,

        /// <summary>
        /// The range is exclusive of the upper end point.
        /// </summary>
        UpperExclusive = 2,

        /// <summary>
        /// The range is exclusive of the end points.
        /// </summary>
        Exclusive = 3,
    }
}
