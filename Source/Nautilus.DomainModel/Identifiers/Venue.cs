//--------------------------------------------------------------------------------------------------
// <copyright file="Venue.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.Core.Extensions;
using Nautilus.Core.Types;

namespace Nautilus.DomainModel.Identifiers
{
    /// <summary>
    /// Represents valid venue identifier. A venue is the tradeable location for financial market
    /// instruments. The identifier value must be unique at the fund level.
    /// </summary>
    [Immutable]
    public class Venue : Identifier<Venue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Venue"/> class.
        /// </summary>
        /// <param name="name">The venue name identifier value.</param>
        public Venue(string name)
            : base(name.ToUpperInvariant())
        {
            Condition.NotEmptyOrWhiteSpace(name, nameof(name));
            Debug.True(name.IsAllUpperCase(), $"The venue value '{name}' was not all upper case.");
        }
    }
}
