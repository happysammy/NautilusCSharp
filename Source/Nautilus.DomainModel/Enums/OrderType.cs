//--------------------------------------------------------------------------------------------------
// <copyright file="OrderType.cs" company="Nautech Systems Pty Ltd">
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
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents an order type.
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// The enumerator value is undefined (invalid).
        /// </summary>
        [InvalidValue]
        Undefined = 0,

        /// <summary>
        /// The market order type. A market order is an order to buy or sell an instrument
        /// immediately. This type of order guarantees that the order will be executed, but does not
        /// guarantee the execution price. A market order generally will execute at or near the
        /// current bid (for a sell order) or ask (for a buy order) price. The last-traded price is
        /// not necessarily the price at which a market order will be executed.
        /// </summary>
        Market = 1,

        /// <summary>
        /// The limit order type. A limit order is an order to buy or sell an instrument at a
        /// specific price. A buy limit order can only be executed at the limit price or lower, and
        /// a sell limit order can only be executed at the limit price or higher.
        /// </summary>
        Limit = 2,

        /// <summary>
        /// The stop order type. A stop order is an instruction to submit a buy or sell  market
        /// order if and when the user-specified stop trigger price is attained or penetrated. A
        /// Stop order is not guaranteed a specific execution price and may execute significantly
        /// away from its stop price.
        /// </summary>
        Stop = 3,

        /// <summary>
        /// The stop-limit order type. A stop-limit order is an order to buy or sell an instrument
        /// that combines the features of a stop order and a limit order.  Once the stop price is
        /// reached, a stop-limit order becomes a limit order that will be executed at a specified
        /// price (or better).  The benefit of a stop-limit order is that the user can control
        /// the price at which the order can be executed.
        /// </summary>
        StopLimit = 4,

        /// <summary>
        /// The market-if-touched order type. A market-if-touched order (MIT) is an order to buy
        /// (or sell) an instrument below (or above) the market. This order is held with the broker
        /// until the trigger price is touched, and is then submitted as a market order. An MIT
        /// order is similar to a stop order, except that an MIT sell order is placed above the
        /// current market price, and a stop sell order is placed below.
        /// </summary>
        [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Correct name")]
        MIT = 5,
    }
}
