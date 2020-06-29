//--------------------------------------------------------------------------------------------------
// <copyright file="BarStructure.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents the granularity of a time resolution.
    /// </summary>
    public enum BarStructure
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The bar structure is based on ticks.
        /// </summary>
        Tick = 1,

        /// <summary>
        /// The bar structure is based on tick imbalance.
        /// </summary>
        TickImbalance = 2,

        /// <summary>
        /// The bar structure is based on volume.
        /// </summary>
        Volume = 3,

        /// <summary>
        /// The bar structure is based on volume imbalance.
        /// </summary>
        VolumeImbalance = 4,

        /// <summary>
        /// The bar structure is based on dollars.
        /// </summary>
        Dollar = 5,

        /// <summary>
        /// The bar structure is based on dollar imbalance.
        /// </summary>
        DollarImbalance = 6,

        /// <summary>
        /// The bar structure is based on seconds.
        /// </summary>
        Second = 7,

        /// <summary>
        /// The bar structure is based on minutes.
        /// </summary>
        Minute = 8,

        /// <summary>
        /// The bar structure is based on hours.
        /// </summary>
        Hour = 9,

        /// <summary>
        /// The bar structure is based on days.
        /// </summary>
        Day = 10,
    }
}
