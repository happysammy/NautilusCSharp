//--------------------------------------------------------------------------------------------------
// <copyright file="AtomicOrder.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.DomainModel.Aggregates;
using Nautilus.DomainModel.Entities.Base;
using Nautilus.DomainModel.Identifiers;

namespace Nautilus.DomainModel.Entities
{
    /// <summary>
    /// Represents a collection of orders being an entry, stop-loss and optional take-profit to
    /// be managed together.
    /// </summary>
    [Immutable]
    public sealed class AtomicOrder : Entity<AtomicOrderId, AtomicOrder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicOrder"/> class.
        /// </summary>
        /// <param name="entry">The entry order.</param>
        /// <param name="stopLoss">The stop-loss order.</param>
        /// <param name="takeProfit">The take-profit order.</param>
        public AtomicOrder(
            Order entry,
            Order stopLoss,
            Order? takeProfit = null)
            : base(new AtomicOrderId("A" + entry.Id.Value), entry.Timestamp)
        {
            this.Entry = entry;
            this.StopLoss = stopLoss;
            this.TakeProfit = takeProfit;
        }

        /// <summary>
        /// Gets the atomic orders symbol.
        /// </summary>
        public Symbol Symbol => this.Entry.Symbol;

        /// <summary>
        /// Gets the atomic orders entry order.
        /// </summary>
        public Order Entry { get; }

        /// <summary>
        /// Gets the atomic orders stop-loss order.
        /// </summary>
        public Order StopLoss { get; }

        /// <summary>
        /// Gets the atomic orders take-profit order (optional can be null).
        /// </summary>
        public Order? TakeProfit { get; }
    }
}
