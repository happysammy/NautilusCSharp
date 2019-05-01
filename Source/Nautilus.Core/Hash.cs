//--------------------------------------------------------------------------------------------------
// <copyright file="Hash.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Core
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Provides standardized hash code generation.
    /// </summary>
    [Immutable]
    [PerformanceOptimized]
    public static class Hash
    {
        private const int Initializer = 17;
        private const int Multiplier = 29;

        /// <summary>
        /// Returns a hash code for the given value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <returns>The hash code <see cref="int"/>.</returns>
        public static int GetCode<T>(T value)
        {
            if (value is null)
            {
                return 0;
            }

            return (Initializer * Multiplier) + value.GetHashCode();
        }

        /// <summary>
        /// Returns a hash code for the given value.
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
                var hash = Initializer;

                if (value1 is null)
                {
                    // Do nothing
                }
                else
                {
                    hash += (hash * Multiplier) + value1.GetHashCode();
                }

                if (value2 is null)
                {
                    // Do nothing
                }
                else
                {
                    hash += (hash * Multiplier) + value2.GetHashCode();
                }

                return hash;
            }
        }
    }
}
