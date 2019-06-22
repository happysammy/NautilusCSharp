//--------------------------------------------------------------------------------------------------
// <copyright file="BarSpecificationFactory.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides a factory for creating <see cref="Bar"/>(s).
    /// </summary>
    public static class BarSpecificationFactory
    {
        /// <summary>
        /// Returns a new <see cref="BarSpecification"/> from the given <see cref="string"/>.
        /// </summary>
        /// <param name="barSpecString">The bar specification string.</param>
        /// <returns>The created <see cref="BarSpecification"/>.</returns>
        public static BarSpecification Create(string barSpecString)
        {
            Debug.NotEmptyOrWhiteSpace(barSpecString, nameof(barSpecString));

            var split1 = barSpecString.Split('-');
            var split2 = split1[1].Split('[');
            var period = Convert.ToInt32(split1[0]);
            var resolution = split2[0].ToUpper();
            var quoteType = split2[1].Trim(']').ToUpper();

            return new BarSpecification(
                period,
                resolution.ToEnum<Resolution>(),
                quoteType.ToEnum<QuoteType>());
        }
    }
}
