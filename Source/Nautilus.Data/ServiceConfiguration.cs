//--------------------------------------------------------------------------------------------------
// <copyright file="ServiceConfiguration.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Data
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.Extensions.Configuration;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix;
    using Nautilus.Network;
    using Nautilus.Network.Configuration;
    using NodaTime;

    /// <summary>
    /// Represents a <see cref="DataService"/> configuration.
    /// </summary>
    public sealed class ServiceConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceConfiguration"/> class.
        /// </summary>
        /// <param name="loggingAdapter">The service logging adapter.</param>
        /// <param name="networkSection">The network configuration section.</param>
        /// <param name="dataSection">The data configuration section.</param>
        /// <param name="messagingConfig">The messaging configuration section.</param>
        /// <param name="fixConfig">The FIX configuration section.</param>
        /// <param name="symbolMap">The service symbol map.</param>
        public ServiceConfiguration(
        ILoggingAdapter loggingAdapter,
        IConfigurationSection networkSection,
        IConfigurationSection dataSection,
        MessagingConfiguration messagingConfig,
        FixConfiguration fixConfig,
        ImmutableDictionary<string, string> symbolMap)
        {
            this.LoggingAdapter = loggingAdapter;

            // Network Configuration
            this.TickRouterPort = new NetworkPort(int.Parse(networkSection["TickRouterPort"]));
            this.TickPublisherPort = new NetworkPort(int.Parse(networkSection["TickPubPort"]));
            this.BarRouterPort = new NetworkPort(int.Parse(networkSection["BarRouterPort"]));
            this.BarPublisherPort = new NetworkPort(int.Parse(networkSection["BarPubPort"]));
            this.InstrumentRouterPort = new NetworkPort(int.Parse(networkSection["InstrumentRouterPort"]));
            this.InstrumentPublisherPort = new NetworkPort(int.Parse(networkSection["InstrumentPubPort"]));

            this.MessagingConfiguration = messagingConfig;
            this.FixConfiguration = fixConfig;

            this.SubscribingSymbols = dataSection.GetSection("Symbols")
                .AsEnumerable()
                .Select(x => x.Value)
                .Where(x => x != null)
                .Distinct()
                .Select(Symbol.FromString)
                .ToImmutableList();

            this.BarSpecifications = dataSection.GetSection("BarSpecifications")
                .AsEnumerable()
                .Select(x => x.Value)
                .Where(x => x != null)
                .Distinct()
                .Select(BarSpecification.FromString)
                .ToImmutableList();

            // Trim Job Ticks
            var trimJobTicks = dataSection.GetSection("TrimJobBars");
            var tickTrimHour = int.Parse(trimJobTicks["Hour"]);
            var tickTrimMinute = int.Parse(trimJobTicks["Minute"]);
            this.TickDataTrimTime = new LocalTime(tickTrimHour, tickTrimMinute);
            this.TickDataTrimWindowDays = int.Parse(trimJobTicks["WindowDays"]);

            // Trim Job Bars
            var trimJobBars = dataSection.GetSection("TrimJobBars");
            var barTrimHour = int.Parse(trimJobBars["Hour"]);
            var barTrimMinute = int.Parse(trimJobBars["Minute"]);
            this.BarDataTrimTime = new LocalTime(barTrimHour, barTrimMinute);
            this.BarDataTrimWindowDays = int.Parse(trimJobBars["WindowDays"]);

            this.SymbolMap = symbolMap;
        }

        /// <summary>
        /// Gets the systems logging adapter.
        /// </summary>
        public ILoggingAdapter LoggingAdapter { get; }

        /// <summary>
        /// Gets the messaging configuration.
        /// </summary>
        public MessagingConfiguration MessagingConfiguration { get; }

        /// <summary>
        /// Gets the network configuration tick request port.
        /// </summary>
        public NetworkPort TickRouterPort { get; }

        /// <summary>
        /// Gets the network configuration tick subscribe port.
        /// </summary>
        public NetworkPort TickPublisherPort { get; }

        /// <summary>
        /// Gets the network configuration bar request port.
        /// </summary>
        public NetworkPort BarRouterPort { get; }

        /// <summary>
        /// Gets the network configuration bar subscribe port.
        /// </summary>
        public NetworkPort BarPublisherPort { get; }

        /// <summary>
        /// Gets the network configuration instrument request port.
        /// </summary>
        public NetworkPort InstrumentRouterPort { get; }

        /// <summary>
        /// Gets the network configuration instrument subscribe port.
        /// </summary>
        public NetworkPort InstrumentPublisherPort { get; }

        /// <summary>
        /// Gets the FIX configuration.
        /// </summary>
        public FixConfiguration FixConfiguration { get; }

        /// <summary>
        /// Gets the symbol conversion index.
        /// </summary>
        public ImmutableDictionary<string, string> SymbolMap { get; }

        /// <summary>
        /// Gets the subscribing symbols.
        /// </summary>
        public IReadOnlyCollection<Symbol> SubscribingSymbols { get; }

        /// <summary>
        /// Gets the configuration bar specifications.
        /// </summary>
        public IReadOnlyCollection<BarSpecification> BarSpecifications { get; }

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
