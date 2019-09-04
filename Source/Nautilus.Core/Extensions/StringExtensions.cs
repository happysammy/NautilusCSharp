//--------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Extensions
{
    using System;

    /// <summary>
    /// Provides useful generic <see cref="string"/> extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns an enumerator of the given type (parsed from the given string).
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <typeparam name="T">The enumerator type.</typeparam>
        /// <returns>An enumerator type.</returns>
        public static T ToEnum<T>(this string input)
            where T : struct
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return default;
            }

            return (T)Enum.Parse(typeof(T), input);
        }

        /// <summary>
        /// Returns a value indicating whether this string is all upper case.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>True if all upper case, else false.</returns>
        public static bool IsAllUpperCase(this string input)
        {
            for (var i = 0; i < input.Length; i++)
            {
                if (char.IsLetter(input[i]) && !char.IsUpper(input[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
