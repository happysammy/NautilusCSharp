//--------------------------------------------------------------------------------------------------
// <copyright file="TraderId.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a valid trader identifier. The <see cref="Name"/> and <see cref="OrderIdTag"/>
    /// combination identifier value must be unique at the fund level.
    ///
    /// <para>
    /// The string representation is the abbreviated name of the trader with an order identifier tag
    /// separated by a hyphen '-'.
    /// </para>
    ///
    /// <para>
    /// The design specification will use the <see cref="OrderIdTag"/> in identifiers sent to a
    /// brokerage, and will not use the <see cref="Name"/>.
    /// </para>
    /// </summary>
    [Immutable]
    public sealed class TraderId : Identifier<TraderId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraderId"/> class.
        /// </summary>
        /// <param name="name">The trader name identifier value.</param>
        /// <param name="orderIdTag">The trader order identifier tag value.</param>
        public TraderId(string name, string orderIdTag)
            : base($"{name}-{orderIdTag}")
        {
            Debug.NotEmptyOrWhiteSpace(name, nameof(name));
            Debug.NotEmptyOrWhiteSpace(orderIdTag, nameof(orderIdTag));

            this.Name = new Label(name);
            this.OrderIdTag = new IdTag(orderIdTag);
        }

        /// <summary>
        /// Gets the trader identifiers name label.
        /// </summary>
        public Label Name { get; }

        /// <summary>
        /// Gets the trader identifiers order identifier tag.
        /// </summary>
        public IdTag OrderIdTag { get; }

        /// <summary>
        /// Return a new <see cref="TraderId"/> parsed from the given string value.
        /// It is expected a trader identifier is the abbreviated name of the trader with
        /// an order identifier tag separated by a hyphen '-'.
        /// </summary>
        /// <param name="value">The trader identifier value.</param>
        /// <returns>The trader identifier.</returns>
        public static TraderId FromString(string value)
        {
            var splitString = value.Split("-", 2);

            return new TraderId(splitString[0], splitString[1]);
        }
    }
}
