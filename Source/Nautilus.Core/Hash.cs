//--------------------------------------------------------------------------------------------------
// <copyright file="Hash.cs" company="Nautech Systems Pty Ltd">
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

using Nautilus.Core.Annotations;

namespace Nautilus.Core
{
    /// <summary>
    /// Provides standardized hash code generation.
    /// </summary>
    public static class Hash
    {
        private const int Initializer = 17;
        private const int Multiplier = 37;

        /// <summary>
        /// Returns a hash code for the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <returns>The hash code <see cref="int"/>.</returns>
        [PerformanceOptimized]
        public static int GetCode<T>(T value)
        {
            unchecked
            {
                return (Initializer * Multiplier) + (value?.GetHashCode() ?? 0);
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
        [PerformanceOptimized]
        public static int GetCode<T1, T2>(T1 value1, T2 value2)
        {
            unchecked
            {
                var hash = Initializer;
                hash = (hash * Multiplier) + (value1?.GetHashCode() ?? 0);
                hash = (hash * Multiplier) + (value2?.GetHashCode() ?? 0);

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
        [PerformanceOptimized]
        public static int GetCode<T1, T2, T3>(T1 value1, T2 value2, T3 value3)
        {
            unchecked
            {
                var hash = Initializer;
                hash = (hash * Multiplier) + (value1?.GetHashCode() ?? 0);
                hash = (hash * Multiplier) + (value2?.GetHashCode() ?? 0);
                hash = (hash * Multiplier) + (value3?.GetHashCode() ?? 0);

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
        [PerformanceOptimized]
        public static int GetCode<T1, T2, T3, T4, T5, T6>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
        {
            unchecked
            {
                var hash = Initializer;
                hash = (hash * Multiplier) + (value1?.GetHashCode() ?? 0);
                hash = (hash * Multiplier) + (value2?.GetHashCode() ?? 0);
                hash = (hash * Multiplier) + (value3?.GetHashCode() ?? 0);
                hash = (hash * Multiplier) + (value4?.GetHashCode() ?? 0);
                hash = (hash * Multiplier) + (value5?.GetHashCode() ?? 0);
                hash = (hash * Multiplier) + (value6?.GetHashCode() ?? 0);

                return hash;
            }
        }
    }
}
