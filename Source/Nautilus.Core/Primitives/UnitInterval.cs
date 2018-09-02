//--------------------------------------------------------------------------------------------------
// <copyright file="UnitInterval.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Primitives
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Represents a double-precision floating-point number within the range of 0 to 1 inclusive.
    /// </summary>
    [Immutable]
    public class UnitInterval : FloatingPointNumber
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitInterval"/> class.
        /// </summary>
        /// <param name="value">The unit interval value.</param>
        public UnitInterval(double value)
            : base(value)
        {
            Debug.NotOutOfRangeDouble(value, nameof(value), 0, 1);
        }
    }
}
