//--------------------------------------------------------------------------------------------------
// <copyright file="BarData.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.DomainModel.ValueObjects
{
    using System;
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents financial market trade bar data including <see cref="BarType"/>.
    /// </summary>
    [Immutable]
    public readonly struct BarData : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarData"/> struct.
        /// </summary>
        /// <param name="barType">The bar type.</param>
        /// <param name="bar">The bar.</param>
        public BarData(BarType barType, Bar bar)
        {
            this.BarType = barType;
            this.Bar = bar;
        }

        /// <summary>
        /// Gets the data bar type.
        /// </summary>
        public BarType BarType { get; }

        /// <summary>
        /// Gets the bar data.
        /// </summary>
        public Bar Bar { get; }

        /// <summary>
        /// Returns a string representation of the <see cref="BarData"/>.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString() => $"{nameof(BarData)}({this.BarType}, {this.Bar})";

        /// <inheritdoc />
        public object Clone()
        {
            return this;  // Immutable type
        }
    }
}
