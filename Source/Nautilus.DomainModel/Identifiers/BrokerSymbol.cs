//--------------------------------------------------------------------------------------------------
// <copyright file="BrokerSymbol.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.DomainModel.Identifiers
{
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Types;

    /// <summary>
    /// Represents a valid broker symbol identifier. This is the symbol the broker assigns to an
    /// instrument.
    /// </summary>
    [Immutable]
    public sealed class BrokerSymbol : Identifier<BrokerSymbol>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokerSymbol"/> class.
        /// </summary>
        /// <param name="value">The broker symbol value.</param>
        public BrokerSymbol(string value)
            : base(value)
        {
            Debug.NotEmptyOrWhiteSpace(value, nameof(value));
        }
    }
}
