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

            // 1-Minute[Bid]
            var period = Convert.ToInt32(barSpecString.Split('-')[0]);
            var resolution = barSpecString.Split('-')[1].Split('[')[0].ToUpper();
            var quoteType = barSpecString.Split('[')[1].Trim(']').ToUpper();

            return new BarSpecification(
                period,
                resolution.ToEnum<Resolution>(),
                quoteType.ToEnum<QuoteType>());
        }
    }
}
