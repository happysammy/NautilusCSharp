//--------------------------------------------------------------------------------------------------
// <copyright file="DecimalExtensions.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Core.Extensions
{
    using System;
    using Nautilus.Core.Correctness;

    /// <summary>
    /// Provides useful generic <see cref="decimal"/> extension methods.
    /// </summary>
    public static class DecimalExtensions
    {
        /// <summary>
        /// Returns the number of decimal places of this <see cref="decimal"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The number of decimal places.</returns>
        public static byte GetDecimalPlaces(this decimal value)
        {
            return BitConverter.GetBytes(decimal.GetBits(value)[3])[2];
        }

        /// <summary>
        /// Returns the decimal tick size from the given <see cref="int"/>.
        /// </summary>
        /// <param name="decimals">The value.</param>
        /// <returns>A decimal representation of the tick size.</returns>
        public static decimal ToTickSize(this int decimals)
        {
            Condition.NotNegativeInt32(decimals, nameof(decimals));

            decimal divisor = 1;

            for (var i = 0; i < decimals; i++)
            {
                divisor *= 10;
            }

            return 1 / divisor;
        }
    }
}
