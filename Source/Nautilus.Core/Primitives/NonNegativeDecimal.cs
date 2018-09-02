//--------------------------------------------------------------------------------------------------
// <copyright file="NonNegativeDecimal.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a non-negative decimal number.
    /// </summary>
    [Immutable]
    public class NonNegativeDecimal : DecimalNumber
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonNegativeDecimal"/> class.
        /// </summary>
        /// <param name="value">The non-negative value.</param>
        public NonNegativeDecimal(decimal value)
            : base(value)
        {
            Debug.NotNegativeDecimal(value, nameof(value));
        }
    }
}
