// -------------------------------------------------------------------------------------------------
// <copyright file="DomainObjectParser.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel
{
    using System;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides a parser for creating domain objects from <see cref="string"/>(s).
    /// </summary>
    public static class DomainObjectParser
    {
        /// <summary>
        /// Returns a new <see cref="Symbol"/> from the given <see cref="string"/>.
        /// </summary>
        /// <param name="symbolString">The symbol string.</param>
        /// <returns>The created <see cref="Symbol"/>.</returns>
        public static Symbol ParseSymbol(string symbolString)
        {
            Debug.NotEmptyOrWhiteSpace(symbolString, nameof(symbolString));

            var symbolSplit = symbolString.Split('.');

            return new Symbol(symbolSplit[0], symbolSplit[1].ToEnum<Venue>());
        }

        /// <summary>
        /// Returns a new <see cref="Tick"/> from the given <see cref="string"/>.
        /// </summary>
        /// <param name="tickString">The string containing tick values.</param>
        /// <returns>The created <see cref="Tick"/>.</returns>
        public static (Price, Price, ZonedDateTime) ParseTickValues(string tickString)
        {
            Debug.NotEmptyOrWhiteSpace(tickString, nameof(tickString));

            var values = tickString.Split(',');

            return new ValueTuple<Price, Price, ZonedDateTime>(
                Price.Create(Convert.ToDecimal(values[0])),
                Price.Create(Convert.ToDecimal(values[1])),
                values[2].ToZonedDateTimeFromIso());
        }

        /// <summary>
        /// Returns a new <see cref="Tick"/> from the given <see cref="string"/>.
        /// </summary>
        /// <param name="symbol">The symbol to create the tick with.</param>
        /// <param name="tickString">The string containing tick values.</param>
        /// <returns>The created <see cref="Tick"/>.</returns>
        public static Tick ParseTick(Symbol symbol, string tickString)
        {
            Debug.NotEmptyOrWhiteSpace(tickString, nameof(tickString));

            var values = tickString.Split(',');

            return new Tick(
                symbol,
                Convert.ToDecimal(values[0]),
                Convert.ToDecimal(values[1]),
                values[2].ToZonedDateTimeFromIso());
        }

        /// <summary>
        /// Returns a new <see cref="Bar"/> from the given <see cref="string"/>.
        /// </summary>
        /// <param name="barString">The bar string.</param>
        /// <returns>The created <see cref="Bar"/>.</returns>
        public static Bar ParseBar(string barString)
        {
            Debug.NotEmptyOrWhiteSpace(barString, nameof(barString));

            var values = barString.Split(',');

            return new Bar(
                Price.Create(Convert.ToDecimal(values[0])),
                Price.Create(Convert.ToDecimal(values[1])),
                Price.Create(Convert.ToDecimal(values[2])),
                Price.Create(Convert.ToDecimal(values[3])),
                Quantity.Create(Convert.ToInt32(Convert.ToDecimal(values[4]))),
                values[5].ToZonedDateTimeFromIso());
        }

        /// <summary>
        /// Returns a new <see cref="BarSpecification"/> from the given <see cref="string"/>.
        /// </summary>
        /// <param name="barSpecString">The bar specification string.</param>
        /// <returns>The created <see cref="BarSpecification"/>.</returns>
        public static BarSpecification ParseBarSpecification(string barSpecString)
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
