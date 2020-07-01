//--------------------------------------------------------------------------------------------------
// <copyright file="MarketPosition.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.DomainModel.Enums
{
    /// <summary>
    /// Represents relative market position.
    /// </summary>
    public enum MarketPosition
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The market position is flat.
        /// </summary>
        Flat = 1,

        /// <summary>
        /// The market position is long.
        /// </summary>
        Long = 2,

        /// <summary>
        /// The market position is short.
        /// </summary>
        Short = 3,
    }
}
