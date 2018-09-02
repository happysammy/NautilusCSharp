//--------------------------------------------------------------------------------------------------
// <copyright file="NonNegativeInteger.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a non-negative integer number.
    /// </summary>
    [Immutable]
    public class NonNegativeInteger : IntegerNumber
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonNegativeInteger"/> class.
        /// </summary>
        /// <param name="value">The non-negative value.</param>
        public NonNegativeInteger(int value)
            : base(value)
        {
            Debug.NotNegativeInt32(value, nameof(value));
        }
    }
}
