//--------------------------------------------------------------
// <copyright file="KeyProvider.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NautechSystems.CSharp.Annotations;
using NautechSystems.CSharp.Validation;
using NautilusDB.Core.Extensions;
using NodaTime;

namespace Nautilus.Database.Core.Keys
{
    [Immutable]
    public static class KeyProvider
    {
        private const string MarketDataNamespace = "market_data";
        private const string Wildcard = "*";

        /// <summary>
        /// Gets the Market Data namespace <see cref="string"/>.
        /// </summary>
        public static string Namespace => MarketDataNamespace;

        /// <summary>
        /// Gets the Market Data namespace wildcard <see cref="string"/>.
        /// </summary>
        public static string NamespaceWildcard => MarketDataNamespace + Wildcard;

        /// <summary>
        /// Returns an array of <see cref="DateKey"/>s based on the given from and to
        /// <see cref="ZonedDateTime"/> range.
        /// </summary>
        /// <param name="barSpec">The bar specification.</param>
        /// <param name="fromDateTime">The from date time.</param>
        /// <param name="toDateTime">The two date time</param>
        /// <returns>An array of <see cref="DateKey"/>.</returns>
        /// <remarks>The given time range should have been previously validated.</remarks>
        public static IEnumerable<string> GetKeyStrings(
            BarSpecification barSpec,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            Validate.NotNull(barSpec, nameof(barSpec));
            Validate.NotDefault(fromDateTime, nameof(fromDateTime));
            Validate.NotDefault(toDateTime, nameof(toDateTime));
            Validate.True(!toDateTime.IsLessThan(fromDateTime), nameof(toDateTime));

            return DateKeyGenerator.GetDateKeys(fromDateTime, toDateTime)
                .Select(key => new MarketDataKey(barSpec, key).ToString())
                .ToList();
        }

        public static string GetWildcardString(BarSpecification barSpec)
        {
            Debug.NotDefault(barSpec, nameof(barSpec));

            return MarketDataNamespace +
                   $":{barSpec.Exchange.ToString().ToLower()}" +
                   $":{barSpec.Symbol.ToLower()}" +
                   $":{barSpec.Resolution.ToString().ToLower()}" +
                   $":{barSpec.QuoteType.ToString().ToLower()}" + Wildcard;
        }
    }
}