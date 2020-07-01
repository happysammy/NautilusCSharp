//--------------------------------------------------------------------------------------------------
// <copyright file="BarDataFrame.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.DomainModel.ValueObjects;
using NodaTime;

namespace Nautilus.DomainModel.Frames
{
    /// <summary>
    /// A container for <see cref="Bars"/> of a certain <see cref="BarType"/>.
    /// </summary>
    [Immutable]
    public sealed class BarDataFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarDataFrame"/> class.
        /// </summary>
        /// <param name="barType">The symbol bar data.</param>
        /// <param name="bars">The bars dictionary.</param>
        public BarDataFrame(BarType barType, Bar[] bars)
        {
            this.BarType = barType;
            this.Bars = bars;
        }

        /// <summary>
        /// Gets the data frames symbol.
        /// </summary>
        public BarType BarType { get; }

        /// <summary>
        /// Gets the data frames bars.
        /// </summary>
        public Bar[] Bars { get; }

        /// <summary>
        /// Gets the data frames count of bars held.
        /// </summary>
        public int Count => this.Bars.Length;

        /// <summary>
        /// Gets the data frames start time.
        /// </summary>
        public ZonedDateTime StartDateTime => this.Bars[0].Timestamp;

        /// <summary>
        /// Gets the data frames end time.
        /// </summary>
        public ZonedDateTime EndDateTime => this.Bars[^1].Timestamp;
    }
}
