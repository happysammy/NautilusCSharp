//--------------------------------------------------------------------------------------------------
// <copyright file="Parser.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Core
{
    /// <summary>
    /// Provides fast specific parsing operations.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parse the given input string to a <see cref="decimal"/>. Note that signs are not
        /// supported and its not expected the input will have a large number of significant digits
        /// (normally financial market prices).
        /// </summary>
        /// <param name="input">The input string to parse.</param>
        /// <returns>The parsed decimal.</returns>
        public static decimal ToDecimal(string input)
        {
            long n = 0;
            var decimalPosition = input.Length;

            for (var k = 0; k < input.Length; k++)
            {
                var c = input[k];
                if (c == '.')
                {
                    decimalPosition = k + 1;
                }
                else
                {
                    n = (n * 10) + (c - '0');
                }
            }

            var scale = (byte)(input.Length - decimalPosition);

            return new decimal((int)n, (int)(n >> 32), 0, false, scale);
        }
    }
}
