//--------------------------------------------------------------------------------------------------
// <copyright file="SafeConvert.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the Apache 2.0 license
//  as found in the LICENSE.txt file.
//  https://github.com/nautechsystems/Nautilus.Core
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Provides safe methods for parsing strings into various value types.
    /// </summary>
    [Immutable]
    public static class SafeConvert
    {
        /// <summary>
        /// Return a valid decimal number (or zero if the given string is unable to be parsed).
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The converted decimal.</returns>
        public static decimal ToDecimal([CanBeNull] string input)
        {
            return decimal.TryParse(input, out var output)
                 ? output
                 : decimal.Zero;
        }

        /// <summary>
        /// Return a valid decimal number (or the given alternative value if the given string is
        /// unable to be parsed).
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="alternativeValue">The alternative decimal value to be returned.</param>
        /// <returns>The converted decimal.</returns>
        public static decimal ToDecimalOr([CanBeNull] string input, decimal alternativeValue)
        {
            Debug.DecimalNotOutOfRange(
                alternativeValue,
                nameof(alternativeValue),
                decimal.MinValue,
                decimal.MaxValue);

            return decimal.TryParse(input, out var output)
                 ? output
                 : alternativeValue;
        }
    }
}
