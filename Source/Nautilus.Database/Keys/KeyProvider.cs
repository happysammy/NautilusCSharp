// -------------------------------------------------------------------------------------------------
// <copyright file="KeyProvider.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Keys
{
    using System.Collections.Generic;
    using System.Linq;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides database keys for market data.
    /// </summary>
    [Immutable]
    public static class KeyProvider
    {
        private const string TicksNamespaceConst = "ticks";
        private const string BarsNamespaceConst = "bars";
        private const string WildcardConst = "*";

        /// <summary>
        /// Gets the ticks namespace <see cref="string"/>.
        /// </summary>
        public static string TicksNamespace => TicksNamespaceConst;

        /// <summary>
        /// Gets the bars namespace <see cref="string"/>.
        /// </summary>
        public static string BarsNamespace => BarsNamespaceConst;

        /// <summary>
        /// Gets the bars namespace wildcard <see cref="string"/>.
        /// </summary>
        public static string TicksNamespaceWildcard => TicksNamespaceConst + WildcardConst;

        /// <summary>
        /// Gets the bars namespace wildcard <see cref="string"/>.
        /// </summary>
        public static string BarsNamespaceWildcard => BarsNamespaceConst + WildcardConst;

        /// <summary>
        /// Returns an array of <see cref="DateKey"/>s based on the given from and to
        /// <see cref="ZonedDateTime"/> range.
        /// </summary>
        /// <param name="symbol">The ticks symbol.</param>
        /// <param name="fromDateTime">The ticks from date time.</param>
        /// <param name="toDateTime">The ticks two date time</param>
        /// <returns>An array of <see cref="DateKey"/>.</returns>
        /// <remarks>The given time range should have been previously validated.</remarks>
        public static IEnumerable<string> GetTicksKeyStrings(
            Symbol symbol,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotDefault(fromDateTime, nameof(fromDateTime));
            Validate.NotDefault(toDateTime, nameof(toDateTime));
            Validate.True(!toDateTime.IsLessThan(fromDateTime), nameof(toDateTime));

            return DateKeyGenerator.GetDateKeys(fromDateTime, toDateTime)
                .Select(key => new TickDataKey(symbol, key).ToString())
                .ToList();
        }

        /// <summary>
        /// Returns a wildcard string from the given symbol bar spec.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string GetTicksWildcardString(Symbol symbol)
        {
            Debug.NotNull(symbol, nameof(symbol));

            return TicksNamespaceConst +
                   $":{symbol.Exchange.ToString().ToLower()}" +
                   $":{symbol.Code.ToLower()}" + WildcardConst;
        }

        /// <summary>
        /// Returns an array of <see cref="DateKey"/>s based on the given from and to
        /// <see cref="ZonedDateTime"/> range.
        /// </summary>
        /// <param name="barType">The bar specification.</param>
        /// <param name="fromDateTime">The from date time.</param>
        /// <param name="toDateTime">The two date time</param>
        /// <returns>An array of <see cref="DateKey"/>.</returns>
        /// <remarks>The given time range should have been previously validated.</remarks>
        public static IEnumerable<string> GetBarsKeyStrings(
            BarType barType,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            Validate.NotNull(barType, nameof(barType));
            Validate.NotDefault(fromDateTime, nameof(fromDateTime));
            Validate.NotDefault(toDateTime, nameof(toDateTime));
            Validate.True(!toDateTime.IsLessThan(fromDateTime), nameof(toDateTime));

            return DateKeyGenerator.GetDateKeys(fromDateTime, toDateTime)
                .Select(key => new BarDataKey(barType, key).ToString())
                .ToList();
        }

        /// <summary>
        /// Returns a wildcard string from the given symbol bar spec.
        /// </summary>
        /// <param name="barType">The symbol bar spec.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string GetBarsWildcardString(BarType barType)
        {
            Debug.NotNull(barType, nameof(barType));

            return BarsNamespaceConst +
                   $":{barType.Symbol.Exchange.ToString().ToLower()}" +
                   $":{barType.Symbol.Code.ToLower()}" +
                   $":{barType.Specification.Resolution.ToString().ToLower()}" +
                   $":{barType.Specification.QuoteType.ToString().ToLower()}" + WildcardConst;
        }
    }
}
