//--------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Extensions
{
    using System;
    using System.Linq;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;

    /// <summary>
    /// Provides useful generic <see cref="string"/> extension methods.
    /// </summary>
    [Immutable]
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a string with all whitespace from the given string removed.
        /// </summary>
        /// <param name="input">The input string (cannot be null or white space).</param>
        /// <returns>A string with no remaining white space.</returns>
        public static string RemoveAllWhitespace(this string input)
        {
            Debug.NotNull(input, nameof(input));

            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }

        /// <summary>
        /// Returns an enumerator of the given type (parsed from the given string).
        /// </summary>
        /// <param name="enumerationString">The enumeration string.</param>
        /// <typeparam name="T">The enumerator type.</typeparam>
        /// <returns>An enumerator type.</returns>
        public static T ToEnum<T>([CanBeNull] this string enumerationString)
            where T : struct
        {
            if (string.IsNullOrWhiteSpace(enumerationString))
            {
                return default(T);
            }

            return (T)Enum.Parse(typeof(T), enumerationString);
        }
    }
}
