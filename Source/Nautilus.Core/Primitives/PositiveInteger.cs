//--------------------------------------------------------------------------------------------------
// <copyright file="PositiveInteger.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a positive integer number.
    /// </summary>
    [Immutable]
    public class PositiveInteger : IntegerNumber
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositiveInteger"/> class.
        /// </summary>
        /// <param name="value">The positive value.</param>
        public PositiveInteger(int value)
            : base(value)
        {
            Debug.PositiveInt32(value, nameof(value));
        }
    }
}
