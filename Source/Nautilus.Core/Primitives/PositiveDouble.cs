//--------------------------------------------------------------------------------------------------
// <copyright file="PositiveDouble.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Primitives
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Represents a positive double-precision floating-point number.
    /// </summary>
    [Immutable]
    public class PositiveDouble : FloatingPointNumber
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositiveDouble"/> class.
        /// </summary>
        /// <param name="value">The positive value.</param>
        public PositiveDouble(double value)
            : base(value)
        {
            Debug.PositiveDouble(value, nameof(value));
        }
    }
}
