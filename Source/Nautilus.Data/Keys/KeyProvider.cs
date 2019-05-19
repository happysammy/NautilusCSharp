// -------------------------------------------------------------------------------------------------
// <copyright file="KeyProvider.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Keys
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides database keys for market data.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Allows nameof.")]
    public static class KeyProvider
    {
        private const string Ticks = nameof(Ticks);
        private const string Bars = nameof(Bars);
        private const string Instruments = nameof(Instruments);
        private const string Wildcard = "*";
        private const string Separator = ":";

        /// <summary>
        /// Gets the ticks namespace <see cref="string"/>.
        /// </summary>
        public static string TicksNamespace => Ticks;

        /// <summary>
        /// Gets the bars namespace <see cref="string"/>.
        /// </summary>
        public static string BarsNamespace => Bars;

        /// <summary>
        /// Gets the instruments namespace <see cref="string"/>.
        /// </summary>
        public static string InstrumentsNamespace => Instruments;

        /// <summary>
        /// Gets the bars namespace wildcard <see cref="string"/>.
        /// </summary>
        public static string TicksNamespaceWildcard => Ticks + Wildcard;

        /// <summary>
        /// Gets the bars namespace wildcard <see cref="string"/>.
        /// </summary>
        public static string BarsNamespaceWildcard => Bars + Wildcard;

        /// <summary>
        /// Gets the instruments namespace wildcard <see cref="string"/>.
        /// </summary>
        public static string InstrumentsWildcard => Instruments + Wildcard;

        /// <summary>
        /// Returns an array of <see cref="DateKey"/>s based on the given from and to
        /// <see cref="ZonedDateTime"/> range.
        /// </summary>
        /// <param name="symbol">The ticks symbol.</param>
        /// <param name="fromDateTime">The ticks from date time.</param>
        /// <param name="toDateTime">The ticks to date time.</param>
        /// <returns>An array of <see cref="DateKey"/>.</returns>
        /// <remarks>The given time range should have been previously validated.</remarks>
        public static IEnumerable<string> GetTicksKeyStrings(
            Symbol symbol,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            Debug.NotDefault(fromDateTime, nameof(fromDateTime));
            Debug.NotDefault(toDateTime, nameof(toDateTime));
            Debug.True(!toDateTime.IsLessThan(fromDateTime), nameof(toDateTime));

            return DateKeyGenerator.GetDateKeys(fromDateTime, toDateTime)
                .Select(key => new TickDataKey(symbol, key).ToString());
        }

        /// <summary>
        /// Returns a wildcard string from the given symbol bar spec.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string GetTicksWildcardString(Symbol symbol)
        {
            return Ticks + Separator + symbol.Venue + Separator + symbol.Code + Wildcard;
        }

        /// <summary>
        /// Returns an array of <see cref="DateKey"/>s based on the given from and to
        /// <see cref="ZonedDateTime"/> range.
        /// </summary>
        /// <param name="barType">The bar specification.</param>
        /// <param name="fromDateTime">The from date time.</param>
        /// <param name="toDateTime">The to date time.</param>
        /// <returns>An array of <see cref="DateKey"/>.</returns>
        /// <remarks>The given time range should have been previously validated.</remarks>
        public static IEnumerable<string> GetBarsKeyStrings(
            BarType barType,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            Debug.NotDefault(fromDateTime, nameof(fromDateTime));
            Debug.NotDefault(toDateTime, nameof(toDateTime));
            Debug.True(!toDateTime.IsLessThan(fromDateTime), nameof(toDateTime));

            return DateKeyGenerator.GetDateKeys(fromDateTime, toDateTime)
                .Select(key => new BarDataKey(barType, key).ToString());
        }

        /// <summary>
        /// Returns a wildcard string from the given symbol bar spec.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public static string GetBarsWildcardString() => Bars + Wildcard;

        /// <summary>
        /// Returns a wildcard string from the given symbol bar spec.
        /// </summary>
        /// <param name="barType">The symbol bar spec.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string GetBarTypeWildcardString(BarType barType)
        {
            return Bars +
                   $"{Separator}{barType.Symbol.Venue}" +
                   $"{Separator}{barType.Symbol.Code}" +
                   $"{Separator}{barType.Specification.Resolution}" +
                   $"{Separator}{barType.Specification.QuoteType}" + Wildcard;
        }

        /// <summary>
        /// Returns the instruments key.
        /// </summary>
        /// <param name="symbol">The instruments symbol.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string GetInstrumentKey(Symbol symbol)
        {
            return Instruments + Separator + symbol;
        }
    }
}
