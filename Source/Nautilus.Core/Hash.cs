//--------------------------------------------------------------------------------------------------
// <copyright file="Hash.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Provides standardized hash code generation.
    /// </summary>
    [PerformanceOptimized]
    public static class Hash
    {
        private const int INITIALIZER = 17;
        private const int MULTIPLIER = 29;

        /// <summary>
        /// Returns a hash code for the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public static int GetCode<T>(T value)
        {
            unchecked
            {
                return (INITIALIZER * MULTIPLIER) + (value is null ? 0 : value.GetHashCode());
            }
        }

        /// <summary>
        /// Returns a hash code for the given values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <typeparam name="T1">The type of value1.</typeparam>
        /// <typeparam name="T2">The type of value2.</typeparam>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public static int GetCode<T1, T2>(T1 value1, T2 value2)
        {
            unchecked
            {
                var hash = INITIALIZER;
                hash = (hash * MULTIPLIER) + (value1 is null ? 0 : value1.GetHashCode());
                hash = (hash * MULTIPLIER) + (value2 is null ? 0 : value2.GetHashCode());

                return hash;
            }
        }

        /// <summary>
        /// Returns a hash code for the given values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="value3">The third value.</param>
        /// <typeparam name="T1">The type of value1.</typeparam>
        /// <typeparam name="T2">The type of value2.</typeparam>
        /// <typeparam name="T3">The type of value3.</typeparam>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public static int GetCode<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
        {
            unchecked
            {
                var hash = INITIALIZER;
                hash = (hash * MULTIPLIER) + (value1 is null ? 0 : value1.GetHashCode());
                hash = (hash * MULTIPLIER) + (value2 is null ? 0 : value2.GetHashCode());
                hash = (hash * MULTIPLIER) + (value3 is null ? 0 : value3.GetHashCode());

                return hash;
            }
        }

        /// <summary>
        /// Returns a hash code for the given values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="value3">The third value.</param>
        /// <param name="value4">The fourth value.</param>
        /// <typeparam name="T1">The type of value1.</typeparam>
        /// <typeparam name="T2">The type of value2.</typeparam>
        /// <typeparam name="T3">The type of value3.</typeparam>
        /// <typeparam name="T4">The type of value4.</typeparam>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public static int GetCode<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            unchecked
            {
                var hash = INITIALIZER;
                hash = (hash * MULTIPLIER) + (value1 is null ? 0 : value1.GetHashCode());
                hash = (hash * MULTIPLIER) + (value2 is null ? 0 : value2.GetHashCode());
                hash = (hash * MULTIPLIER) + (value3 is null ? 0 : value3.GetHashCode());
                hash = (hash * MULTIPLIER) + (value4 is null ? 0 : value4.GetHashCode());

                return hash;
            }
        }

        /// <summary>
        /// Returns a hash code for the given values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="value3">The third value.</param>
        /// <param name="value4">The fourth value.</param>
        /// <param name="value5">The fifth value.</param>
        /// <typeparam name="T1">The type of value1.</typeparam>
        /// <typeparam name="T2">The type of value2.</typeparam>
        /// <typeparam name="T3">The type of value3.</typeparam>
        /// <typeparam name="T4">The type of value4.</typeparam>
        /// <typeparam name="T5">The type of value5.</typeparam>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public static int GetCode<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            unchecked
            {
                var hash = INITIALIZER;
                hash = (hash * MULTIPLIER) + (value1 is null ? 0 : value1.GetHashCode());
                hash = (hash * MULTIPLIER) + (value2 is null ? 0 : value2.GetHashCode());
                hash = (hash * MULTIPLIER) + (value3 is null ? 0 : value3.GetHashCode());
                hash = (hash * MULTIPLIER) + (value4 is null ? 0 : value4.GetHashCode());
                hash = (hash * MULTIPLIER) + (value5 is null ? 0 : value5.GetHashCode());

                return hash;
            }
        }

        /// <summary>
        /// Returns a hash code for the given values.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <param name="value3">The third value.</param>
        /// <param name="value4">The fourth value.</param>
        /// <param name="value5">The fifth value.</param>
        /// <param name="value6">The sixth value.</param>
        /// <typeparam name="T1">The type of value1.</typeparam>
        /// <typeparam name="T2">The type of value2.</typeparam>
        /// <typeparam name="T3">The type of value3.</typeparam>
        /// <typeparam name="T4">The type of value4.</typeparam>
        /// <typeparam name="T5">The type of value5.</typeparam>
        /// <typeparam name="T6">The type of value6.</typeparam>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public static int GetCode<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
        {
            unchecked
            {
                var hash = INITIALIZER;
                hash = (hash * MULTIPLIER) + (value1 is null ? 0 : value1.GetHashCode());
                hash = (hash * MULTIPLIER) + (value2 is null ? 0 : value2.GetHashCode());
                hash = (hash * MULTIPLIER) + (value3 is null ? 0 : value3.GetHashCode());
                hash = (hash * MULTIPLIER) + (value4 is null ? 0 : value4.GetHashCode());
                hash = (hash * MULTIPLIER) + (value5 is null ? 0 : value5.GetHashCode());
                hash = (hash * MULTIPLIER) + (value6 is null ? 0 : value6.GetHashCode());

                return hash;
            }
        }
    }
}
