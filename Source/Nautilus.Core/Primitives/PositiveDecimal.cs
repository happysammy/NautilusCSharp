//--------------------------------------------------------------------------------------------------
// <copyright file="PositiveDecimal.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a positive decimal number.
    /// </summary>
    [Immutable]
    public class PositiveDecimal : DecimalNumber
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositiveDecimal"/> class.
        /// </summary>
        /// <param name="value">The positive value.</param>
        public PositiveDecimal(decimal value)
            : base(value)
        {
            Debug.PositiveDecimal(value, nameof(value));
        }
    }
}
