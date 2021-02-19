//--------------------------------------------------------------------------------------------------
// <copyright file="OrderState.cs" company="Nautech Systems Pty Ltd">
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
    /// Represents the state of an order at the brokerage.
    /// </summary>
    public enum OrderState
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The initialized order state.
        /// </summary>
        Initialized = 1,

        /// <summary>
        /// The invalid order state.
        /// </summary>
        Invalid = 2,

        /// <summary>
        /// The denied order state.
        /// </summary>
        Denied = 3,

        /// <summary>
        /// The submitted order state.
        /// </summary>
        Submitted = 4,

        /// <summary>
        /// The accepted order state.
        /// </summary>
        Accepted = 5,

        /// <summary>
        /// The rejected order state.
        /// </summary>
        Rejected = 6,

        /// <summary>
        /// The working order state.
        /// </summary>
        Working = 7,

        /// <summary>
        /// The cancelled order state.
        /// </summary>
        Cancelled = 8,

        /// <summary>
        /// Expired order state.
        /// </summary>
        Expired = 9,

        /// <summary>
        /// The partially filled order state.
        /// </summary>
        PartiallyFilled = 10,

        /// <summary>
        /// The completely filled order state.
        /// </summary>
        Filled = 11,
    }
}
