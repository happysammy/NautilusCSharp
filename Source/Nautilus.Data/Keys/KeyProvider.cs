// -------------------------------------------------------------------------------------------------
// <copyright file="KeyProvider.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Keys
{
    using System;
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
        private const string NautilusData = nameof(Nautilus) + ":" + nameof(Data);
        private const string Ticks = nameof(Ticks);
        private const string Bars = nameof(Bars);
        private const string Instruments = nameof(Instruments);

        private static readonly string TicksNamespace = $"{NautilusData}:{Ticks}";
        private static readonly string BarsNamespace = $"{NautilusData}:{Bars}";
        private static readonly string InstrumentsNamespace = $"{NautilusData}:{Instruments}";

        /// <summary>
        /// Returns an array of <see cref="DateKey"/>s based on the given from and to <see cref="ZonedDateTime"/> range.
        /// </summary>
        /// <param name="fromDateTime">The from date time.</param>
        /// <param name="toDateTime">The to date time.</param>
        /// <returns>An array of <see cref="DateKey"/>.</returns>
        /// <remarks>The given time range should have been previously validated.</remarks>
        public static List<DateKey> GetDateKeys(ZonedDateTime fromDateTime, ZonedDateTime toDateTime)
        {
            Debug.NotDefault(fromDateTime, nameof(fromDateTime));
            Debug.NotDefault(toDateTime, nameof(toDateTime));
            Debug.True(!toDateTime.IsLessThan(fromDateTime), nameof(toDateTime));

            var difference = (toDateTime - fromDateTime) / Duration.FromDays(1);

            if (difference <= 1)
            {
                return new List<DateKey> { new DateKey(fromDateTime) };
            }

            var iterationCount = Convert.ToInt32(Math.Floor(difference));

            var dateKeys = new List<DateKey> { new DateKey(fromDateTime) };
            for (var i = 0; i < iterationCount; i++)
            {
                dateKeys.Add(new DateKey(fromDateTime + Duration.FromDays(i + 1)));
            }

            return dateKeys;
        }

        /// <summary>
        /// Returns a wildcard string from the given symbol bar spec.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string GetTickWildcardKey(Symbol symbol)
        {
            return $"{TicksNamespace}:{symbol.Venue}:{symbol.Code}";
        }

        /// <summary>
        /// Returns the tick data key for the given parameters.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="dateKey">The date key.</param>
        /// <returns>The key string.</returns>
        public static string GetTickKey(Symbol symbol, DateKey dateKey)
        {
            return $"{TicksNamespace}:{symbol.Venue}:{symbol.Code}:{dateKey}";
        }

        /// <summary>
        /// Returns an array of <see cref="DateKey"/>s based on the given from and to
        /// <see cref="ZonedDateTime"/> range.
        /// </summary>
        /// <param name="symbol">The ticks symbol.</param>
        /// <param name="fromDateTime">The ticks from date time.</param>
        /// <param name="toDateTime">The ticks to date time.</param>
        /// <returns>An array of <see cref="DateKey"/>.</returns>
        /// <remarks>The given time range should have been previously validated.</remarks>
        public static IEnumerable<string> GetTickKeys(
            Symbol symbol,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            Debug.NotDefault(fromDateTime, nameof(fromDateTime));
            Debug.NotDefault(toDateTime, nameof(toDateTime));
            Debug.True(!toDateTime.IsLessThan(fromDateTime), nameof(toDateTime));

            return GetDateKeys(fromDateTime, toDateTime).Select(key => GetTickKey(symbol, key));
        }

        /// <summary>
        /// Returns a wildcard key string for all bars.
        /// </summary>
        /// <returns>The key <see cref="string"/>.</returns>
        public static string GetBarWildcardKey()
        {
            return BarsNamespace + "*";
        }

        /// <summary>
        /// Returns a wildcard string from the given symbol bar spec.
        /// </summary>
        /// <param name="barType">The symbol bar spec.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string GetBarWildcardKey(BarType barType)
        {
            return BarsNamespace +
                   $":{barType.Symbol.Venue}" +
                   $":{barType.Symbol.Code}" +
                   $":{barType.Specification.Resolution}" +
                   $":{barType.Specification.QuoteType}" + "*";
        }

        /// <summary>
        /// Returns the bar data key for the given parameters.
        /// </summary>
        /// <param name="barType">The bar type.</param>
        /// <param name="dateKey">The date key.</param>
        /// <returns>The key <see cref="string"/>.</returns>
        public static string GetBarKey(BarType barType, DateKey dateKey)
        {
            return BarsNamespace +
                   $":{barType.Symbol.Venue}" +
                   $":{barType.Symbol.Code}" +
                   $":{barType.Specification.Resolution}" +
                   $":{barType.Specification.QuoteType}" +
                   $":{dateKey}";
        }

        /// <summary>
        /// Returns an array of <see cref="DateKey"/>s based on the given from and to
        /// <see cref="ZonedDateTime"/> range.
        /// </summary>
        /// <param name="barType">The bar specification.</param>
        /// <param name="fromDateTime">The from date time.</param>
        /// <param name="toDateTime">The to date time.</param>
        /// <returns>An array of key <see cref="string"/>(s).</returns>
        /// <remarks>The given time range should have been previously validated.</remarks>
        public static IEnumerable<string> GetBarKeys(
            BarType barType,
            ZonedDateTime fromDateTime,
            ZonedDateTime toDateTime)
        {
            Debug.NotDefault(fromDateTime, nameof(fromDateTime));
            Debug.NotDefault(toDateTime, nameof(toDateTime));
            Debug.True(toDateTime.IsGreaterThanOrEqualTo(fromDateTime), nameof(toDateTime));

            return GetDateKeys(fromDateTime, toDateTime).Select(key => GetBarKey(barType, key));
        }

        /// <summary>
        /// Returns a wildcard key string for all instruments.
        /// </summary>
        /// <returns>The key <see cref="string"/>.</returns>
        public static string GetInstrumentWildcardKey()
        {
            return InstrumentsNamespace + "*";
        }

        /// <summary>
        /// Returns the instruments key.
        /// </summary>
        /// <param name="symbol">The instruments symbol.</param>
        /// <returns>The key <see cref="string"/>.</returns>
        public static string GetInstrumentKey(Symbol symbol)
        {
            return InstrumentsNamespace + ":" + symbol;
        }
    }
}
