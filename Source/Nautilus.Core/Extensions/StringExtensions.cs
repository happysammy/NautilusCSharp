//--------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Extensions
{
    using System;
    using System.Linq;

    /// <summary>
    /// Provides useful generic <see cref="string"/> extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns an enumerator of the given type parsed from this string.
        /// </summary>
        /// <param name="input">This string.</param>
        /// <typeparam name="T">The enumerator type.</typeparam>
        /// <returns>The parsed enumerator.</returns>
        /// <exception cref="ArgumentException">If the input cannot be parsed.</exception>
        public static T ToEnum<T>(this string input)
            where T : struct
        {
            var stripped = input.Replace("_", string.Empty);  // Strip snake case of underscores
            if (Enum.TryParse<T>(stripped, true, out var result))
            {
                return result;
            }

            return Enum.Parse<T>("Undefined");
        }

        /// <summary>
        /// Returns an upper case SNAKE_CASE string from this string.
        /// </summary>
        /// <param name="input">This string.</param>
        /// <param name="upperCase">If the parsed string should be upper case.</param>
        /// <returns>The parsed string.</returns>
        public static string ToSnakeCase(this string input, bool upperCase = true)
        {
            var snaked = string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x)
                ? "_" + x
                : x.ToString()));
            return upperCase
                ? snaked.ToUpper()
                : snaked.ToLower();
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
