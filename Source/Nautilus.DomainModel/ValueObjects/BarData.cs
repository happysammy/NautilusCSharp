//--------------------------------------------------------------------------------------------------
// <copyright file="BarData.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.ValueObjects
{
    using Nautilus.Core.Annotations;

    /// <summary>
    /// Represents financial market trade bar data including <see cref="BarType"/>.
    /// </summary>
    [Immutable]
    public struct BarData
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
    }
}
