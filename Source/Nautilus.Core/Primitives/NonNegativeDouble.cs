//--------------------------------------------------------------------------------------------------
// <copyright file="NonNegativeDouble.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a non-negative double-precision floating-point number.
    /// </summary>
    [Immutable]
    public class NonNegativeDouble : FloatingPointNumber
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonNegativeDouble"/> class.
        /// </summary>
        /// <param name="value">The non-negative value.</param>
        public NonNegativeDouble(double value)
            : base(value)
        {
            Debug.NotNegativeDouble(value, nameof(value));
        }
    }
}
