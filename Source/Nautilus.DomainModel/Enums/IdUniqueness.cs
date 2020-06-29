//--------------------------------------------------------------------------------------------------
// <copyright file="IdUniqueness.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.DomainModel.Enums
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// The uniqueness context of an identifier. An identifier is unique if there are no duplicates
    /// of its value within the context.
    /// </summary>
    public enum IdUniqueness
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The identifier must be unique at the strategy level.
        /// </summary>
        Strategy = 1,

        /// <summary>
        /// The identifier must be unique at the trader level.
        /// </summary>
        Trader = 2,

        /// <summary>
        /// The identifier must be unique at the fund/team level.
        /// </summary>
        Fund = 3,

        /// <summary>
        /// The identifier must be unique at the global level.
        /// </summary>
        Global = 4,
    }
}
