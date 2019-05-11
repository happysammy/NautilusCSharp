//--------------------------------------------------------------------------------------------------
// <copyright file="BarFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Factories
{
    using System;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a factory for creating <see cref="Bar"/>(s).
    /// </summary>
    public static class BarFactory
    {
        /// <summary>
        /// Returns a new <see cref="Bar"/> from the given <see cref="string"/>.
        /// </summary>
        /// <param name="barString">The bar string.</param>
        /// <returns>The created <see cref="Bar"/>.</returns>
        public static Bar Create(string barString)
        {
            Debug.NotEmptyOrWhiteSpace(barString, nameof(barString));

            var values = barString.Split(',');

            return new Bar(
                Price.Create(Convert.ToDecimal(values[0].ToString())),
                Price.Create(Convert.ToDecimal(values[1])),
                Price.Create(Convert.ToDecimal(values[2])),
                Price.Create(Convert.ToDecimal(values[3])),
                Quantity.Create(Convert.ToInt32(Convert.ToDecimal(values[4]))),
                values[5].ToZonedDateTimeFromIso());
        }
    }
}
