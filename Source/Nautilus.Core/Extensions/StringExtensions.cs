//--------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System;
using System.Linq;

namespace Nautilus.Core.Extensions
{
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
            return Enum.TryParse<T>(stripped, true, out var result)
                ? result
                : Enum.Parse<T>("Undefined");
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

        /// <summary>
        /// Returns a new string trimmed of all none alpha numeric chars.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The trimmed string.</returns>
        public static string ToAlphaNumeric(this string input)
        {
            var arr = input.ToCharArray();
            arr = Array.FindAll(arr, char.IsLetterOrDigit);

            return new string(arr);
        }
    }
}
