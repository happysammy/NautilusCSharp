//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentBuilder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Redis.Data.Builders
{
    using System.Collections.Generic;
    using System.Globalization;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using NodaTime;

    /// <summary>
    /// Provides a builder for creating <see cref="Instrument"/> objects.
    /// </summary>
    public sealed class InstrumentBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentBuilder"/> class.
        /// </summary>
        /// <param name="startingInstrument">
        /// The starting instrument.
        /// </param>
        public InstrumentBuilder(Instrument startingInstrument)
        {
            this.Symbol = startingInstrument.Symbol;
            this.BrokerSymbol = startingInstrument.BrokerSymbol;
            this.QuoteCurrency = startingInstrument.QuoteCurrency;
            this.SecurityType = startingInstrument.SecurityType;
            this.TickPrecision = startingInstrument.TickPrecision;
            this.TickSize = startingInstrument.TickSize;
            this.RoundLotSize = startingInstrument.RoundLotSize;
            this.MinStopDistanceEntry = startingInstrument.MinStopDistanceEntry;
            this.MinLimitDistanceEntry = startingInstrument.MinLimitDistanceEntry;
            this.MinStopDistance = startingInstrument.MinStopDistance;
            this.MinLimitDistance = startingInstrument.MinLimitDistance;
            this.MinTradeSize = startingInstrument.MinTradeSize;
            this.MaxTradeSize = startingInstrument.MaxTradeSize;
            this.RolloverInterestBuy = startingInstrument.RolloverInterestBuy;
            this.RolloverInterestSell = startingInstrument.RolloverInterestSell;
        }

        /// <summary>
        /// Gets the list of changes to this instrument.
        /// </summary>
        public IList<string> Changes { get; } = new List<string>();

        private Symbol Symbol { get; }

        private BrokerSymbol BrokerSymbol { get; }

        private Currency QuoteCurrency { get; }

        private SecurityType SecurityType { get; }

        private int TickPrecision { get; }

        private decimal TickSize { get; }

        private int RoundLotSize { get; }

        private int MinStopDistanceEntry { get; set; }

        private int MinLimitDistanceEntry { get; set; }

        private int MinStopDistance { get; set; }

        private int MinLimitDistance { get; set; }

        private int MinTradeSize { get; set; }

        private int MaxTradeSize { get; set; }

        private decimal RolloverInterestBuy { get; set; }

        private decimal RolloverInterestSell { get; set; }

        /// <summary>
        /// Creates and returns a new <see cref="InstrumentBuilder"/> updated from the given
        /// <see cref="Instrument"/>.
        /// </summary>
        /// <param name="updateInstrument">The updated instrument.</param>
        /// <returns>A <see cref="InstrumentBuilder"/>.</returns>
        public InstrumentBuilder Update(Instrument updateInstrument)
        {
            if (this.MinStopDistanceEntry != updateInstrument.MinStopDistanceEntry)
            {
                this.AddChange(
                    nameof(this.MinStopDistanceEntry),
                    this.MinStopDistanceEntry.ToString(CultureInfo.InvariantCulture),
                    updateInstrument.MinStopDistanceEntry.ToString(CultureInfo.InvariantCulture));

                this.MinStopDistanceEntry = updateInstrument.MinStopDistanceEntry;
            }

            if (this.MinLimitDistanceEntry != updateInstrument.MinLimitDistanceEntry)
            {
                this.AddChange(
                    nameof(this.MinLimitDistanceEntry),
                    this.MinLimitDistanceEntry.ToString(CultureInfo.InvariantCulture),
                    updateInstrument.MinLimitDistanceEntry.ToString(CultureInfo.InvariantCulture));

                this.MinLimitDistanceEntry = updateInstrument.MinLimitDistanceEntry;
            }

            if (this.MinStopDistance != updateInstrument.MinStopDistance)
            {
                this.AddChange(
                    nameof(this.MinStopDistance),
                    this.MinStopDistance.ToString(CultureInfo.InvariantCulture),
                    updateInstrument.MinStopDistance.ToString(CultureInfo.InvariantCulture));

                this.MinStopDistance = updateInstrument.MinStopDistance;
            }

            if (this.MinLimitDistance != updateInstrument.MinLimitDistance)
            {
                this.AddChange(
                    nameof(this.MinLimitDistance),
                    this.MinLimitDistance.ToString(CultureInfo.InvariantCulture),
                    updateInstrument.MinLimitDistance.ToString(CultureInfo.InvariantCulture));

                this.MinLimitDistance = updateInstrument.MinLimitDistance;
            }

            if (this.MinTradeSize != updateInstrument.MinTradeSize)
            {
                this.AddChange(
                    nameof(this.MinTradeSize),
                    this.MinTradeSize.ToString(CultureInfo.InvariantCulture),
                    updateInstrument.MinTradeSize.ToString(CultureInfo.InvariantCulture));

                this.MinTradeSize = updateInstrument.MinTradeSize;
            }

            if (this.MaxTradeSize != updateInstrument.MaxTradeSize)
            {
                this.AddChange(
                    nameof(this.MaxTradeSize),
                    this.MaxTradeSize.ToString(CultureInfo.InvariantCulture),
                    updateInstrument.MaxTradeSize.ToString(CultureInfo.InvariantCulture));

                this.MaxTradeSize = updateInstrument.MaxTradeSize;
            }

            if (this.RolloverInterestBuy != updateInstrument.RolloverInterestBuy)
            {
                this.AddChange(
                    nameof(this.RolloverInterestBuy),
                    this.RolloverInterestBuy.ToString(CultureInfo.InvariantCulture),
                    updateInstrument.RolloverInterestBuy.ToString(CultureInfo.InvariantCulture));

                this.RolloverInterestBuy = updateInstrument.RolloverInterestBuy;
            }

            if (this.RolloverInterestSell != updateInstrument.RolloverInterestSell)
            {
                this.AddChange(
                    nameof(this.RolloverInterestSell),
                    this.RolloverInterestSell.ToString(CultureInfo.InvariantCulture),
                    updateInstrument.RolloverInterestSell.ToString(CultureInfo.InvariantCulture));

                this.RolloverInterestSell = updateInstrument.RolloverInterestSell;
            }

            return this;
        }

        /// <summary>
        /// Creates and returns a new <see cref="Instrument"/> with the values held by the
        /// <see cref="InstrumentBuilder"/>.
        /// </summary>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>A <see cref="Instrument"/>.</returns>
        public Instrument Build(ZonedDateTime timestamp)
        {
            return new Instrument(
                this.Symbol,
                this.BrokerSymbol,
                this.QuoteCurrency,
                this.SecurityType,
                this.TickPrecision,
                this.TickSize,
                this.RoundLotSize,
                this.MinStopDistanceEntry,
                this.MinLimitDistanceEntry,
                this.MinStopDistance,
                this.MinLimitDistance,
                this.MinTradeSize,
                this.MaxTradeSize,
                this.RolloverInterestBuy,
                this.RolloverInterestSell,
                timestamp);
        }

        private void AddChange(
            string property,
            string oldValue,
            string newValue)
        {
            Debug.NotEmptyOrWhiteSpace(property, nameof(property));
            Debug.NotEmptyOrWhiteSpace(oldValue, nameof(oldValue));
            Debug.NotEmptyOrWhiteSpace(newValue, nameof(newValue));

            this.Changes.Add($", {property} updated from {oldValue} to {newValue}");
        }
    }
}
