//--------------------------------------------------------------------------------------------------
// <copyright file="Brokerage.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents a valid brokerage identifier. The identifier value must be unique at the global
    /// level.
    /// </summary>
    [Immutable]
    public sealed class Brokerage : Identifier<Brokerage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Brokerage"/> class.
        /// </summary>
        /// <param name="name">The brokerage name identifier value.</param>
        public Brokerage(string name)
            : base(name)
        {
            Debug.NotEmptyOrWhiteSpace(name, nameof(name));
        }
    }
}
