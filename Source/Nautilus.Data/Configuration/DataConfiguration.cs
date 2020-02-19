//--------------------------------------------------------------------------------------------------
// <copyright file="DataConfiguration.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data.Configuration
{
    using System.Collections.Immutable;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides the data configuration for a <see cref="DataService"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1611", Justification = "TODO")]
    public sealed class DataConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataConfiguration"/> class.
        /// </summary>
        public DataConfiguration(
            ImmutableDictionary<string, string> symbolMap,
            ImmutableList<Symbol> subscribingSymbols,
            ImmutableList<BarSpecification> barSpecifications,
            LocalTime tickDataTrimTime,
            LocalTime barDataTrimTime,
            int tickDataTrimWindowDays,
            int barDataTrimWindowDays)
        {
            this.SymbolMap = symbolMap;
            this.SubscribingSymbols = subscribingSymbols;
            this.BarSpecifications = barSpecifications;
            this.TickDataTrimTime = tickDataTrimTime;
            this.BarDataTrimTime = barDataTrimTime;
            this.TickDataTrimWindowDays = tickDataTrimWindowDays;
            this.BarDataTrimWindowDays = barDataTrimWindowDays;
        }

        /// <summary>
        /// Gets the symbol conversion index.
        /// </summary>
        public ImmutableDictionary<string, string> SymbolMap { get; }

        /// <summary>
        /// Gets the subscribing symbols.
        /// </summary>
        public ImmutableList<Symbol> SubscribingSymbols { get; }

        /// <summary>
        /// Gets the configuration bar specifications.
        /// </summary>
        public ImmutableList<BarSpecification> BarSpecifications { get; }

        /// <summary>
        /// Gets the time to trim the tick data.
        /// </summary>
        public LocalTime TickDataTrimTime { get; }

        /// <summary>
        /// Gets the time to trim the bar data.
        /// </summary>
        public LocalTime BarDataTrimTime { get; }

        /// <summary>
        /// Gets the tick data rolling trim window in days.
        /// </summary>
        public int TickDataTrimWindowDays { get; }

        /// <summary>
        /// Gets the bar data rolling trim window in days.
        /// </summary>
        public int BarDataTrimWindowDays { get; }
    }
}
