//--------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core.Extensions
{
    using System;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Provides useful generic <see cref="string"/> extension methods.
    /// </summary>
    [Immutable]
    public static class StringExtensions
    {
        /// <summary>
        /// Returns an enumerator of the given type (parsed from the given string).
        /// </summary>
        /// <param name="enumerationString">The enumeration string.</param>
        /// <typeparam name="T">The enumerator type.</typeparam>
        /// <returns>An enumerator type.</returns>
        public static T ToEnum<T>(this string enumerationString)
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
