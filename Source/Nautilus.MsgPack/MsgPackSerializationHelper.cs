// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackSerializationHelper.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.MsgPack
{
    using System;
    using Nautilus.Core;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides helper methods for objects to serialize.
    /// </summary>
    internal static class MsgPackSerializationHelper
    {
        private const string None = "NONE";

        /// <summary>
        /// Parses and returns the symbol from the given string.
        /// </summary>
        /// <param name="symbolString">The symbol string.</param>
        /// <returns>The parsed symbol <see cref="string"/>.</returns>
        internal static Symbol GetSymbol(string symbolString)
        {
            Debug.NotNull(symbolString, nameof(symbolString));

            var splitSymbol = symbolString.Split('.');
            return new Symbol(splitSymbol[0], splitSymbol[1].ToEnum<Venue>());
        }

        /// <summary>
        /// Return a <see cref="Price"/> from the given string.
        /// </summary>
        /// <param name="priceString">The price string.</param>
        /// <returns>The optional price.</returns>
        internal static Option<Price> GetPrice(string priceString)
        {
            Debug.NotNull(priceString, nameof(priceString));

            if (priceString == None)
            {
                return Option<Price>.None();
            }

            var priceDecimal = Convert.ToDecimal(priceString);
            var priceDecimalPlaces = priceDecimal.GetDecimalPlaces();

            return Price.Create(priceDecimal, priceDecimalPlaces);
        }

        /// <summary>
        /// Return a string from the given price.
        /// </summary>
        /// <param name="price">The price.</param>
        /// <returns>The optional price.</returns>
        internal static string GetPriceString(Option<Price> price)
        {
            Debug.NotNull(price, nameof(price));

            return price.HasValue
                ? None
                : price.ToString();
        }

        /// <summary>
        /// Parses and returns the expire time from the given string.
        /// </summary>
        /// <param name="expireTimeString">The expire time string.</param>
        /// <returns>The parsed expire time <see cref="string"/>.</returns>
        internal static Option<ZonedDateTime?> GetExpireTime(string expireTimeString)
        {
            Debug.NotNull(expireTimeString, nameof(expireTimeString));

            return expireTimeString == None
                ? Option<ZonedDateTime?>.None()
                : Option<ZonedDateTime?>.Some(expireTimeString.ToZonedDateTimeFromIso());
        }

        /// <summary>
        /// Returns the date time ISO 8601 string from the given expire time.
        /// </summary>
        /// <param name="expireTime">The optional expire time.</param>
        /// <returns>The parsed expire time <see cref="string"/>.</returns>
        internal static string GetExpireTimeString(Option<ZonedDateTime?> expireTime)
        {
            Debug.NotNull(expireTime, nameof(expireTime));

            return expireTime.HasNoValue
                ? None
                : expireTime.Value.ToIsoString();
        }
    }
}
