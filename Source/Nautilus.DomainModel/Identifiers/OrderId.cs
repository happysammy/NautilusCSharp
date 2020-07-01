//--------------------------------------------------------------------------------------------------
// <copyright file="OrderId.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core.Correctness;
using Nautilus.Core.Types;

namespace Nautilus.DomainModel.Identifiers
{
    /// <summary>
    /// Represents a valid order identifier. The identifier value must be unique at the fund level.
    ///
    /// <para>
    /// It is expected that the identifier value starts with 'O-'.
    /// </para>
    /// </summary>
    [Immutable]
    public sealed class OrderId : Identifier<OrderId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderId"/> class.
        /// </summary>
        /// <param name="value">The order identifier value.</param>
        public OrderId(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));

            // Debug.True(value.StartsWith("O-"), $"The value did not start with 'O-', was {value}.");
        }

        /// <summary>
        /// Return a new <see cref="OrderId"/> parsed from the given string value.
        /// </summary>
        /// <param name="value">The order identifier value.</param>
        /// <returns>The order identifier.</returns>
        public static OrderId FromString(string value)
        {
            return new OrderId(value);
        }
    }
}
