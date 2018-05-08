//--------------------------------------------------------------------------------------------------
// <copyright file="Instrument.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Entities
{
    using System;
    using System.Runtime.Serialization;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Extensions;
    using NautechSystems.CSharp.Validation;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The immutable sealed <see cref="Instrument"/> class. Represents a financial market instrument.
    /// </summary>
    [Immutable]
    [Serializable]
    public sealed class Instrument : Entity<Instrument>, ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Instrument"/> class.
        /// </summary>
        /// <param name="symbol">The instrument symbol.</param>
        /// <param name="instrumentId">The instrument identifier.</param>
        /// <param name="brokerSymbol">The instrument broker symbol.</param>
        /// <param name="quoteCurrency">The instrument quote currency.</param>
        /// <param name="securityType">The instrument security type.</param>
        /// <param name="tickSize">The instrument tick size.</param>
        /// <param name="tickValue">The instrument tick value.</param>
        /// <param name="targetDirectSpread">The instrument target direct spread.</param>
        /// <param name="contractSize">The instrument contract size.</param>
        /// <param name="minStopDistanceEntry">The instrument minimum stop distance for entry.</param>
        /// <param name="minLimitDistanceEntry">The instrument minimum limit distance for entry.</param>
        /// <param name="minStopDistance">The instrument minimum stop distance.</param>
        /// <param name="minLimitDistance">The instrument minimum limit distance.</param>
        /// <param name="minTradeSize">The instrument minimum trade size.</param>
        /// <param name="maxTradeSize">The instrument maximum trade size.</param>
        /// <param name="marginRequirement">The instrument margin requirement.</param>
        /// <param name="rolloverInterestBuy">The instrument rollover interest for long positions.</param>
        /// <param name="rolloverInterestSell">The instrument rollover interest for short positions.</param>
        /// <param name="timestamp"> The instrument timestamp.</param>
        /// <exception cref="ValidationException">Throws if the validation fails (see constructor).</exception>
        public Instrument(
            Symbol symbol,
            EntityId instrumentId,
            EntityId brokerSymbol,
            CurrencyCode quoteCurrency,
            SecurityType securityType,
            decimal tickSize,
            decimal tickValue,
            int targetDirectSpread,
            int contractSize,
            int minStopDistanceEntry,
            int minLimitDistanceEntry,
            int minStopDistance,
            int minLimitDistance,
            int minTradeSize,
            int maxTradeSize,
            decimal marginRequirement,
            decimal rolloverInterestBuy,
            decimal rolloverInterestSell,
            ZonedDateTime timestamp)
            : base(instrumentId, timestamp)
        {
            Validate.NotNull(symbol, nameof(symbol));
            Validate.NotNull(instrumentId, nameof(instrumentId));
            Validate.NotNull(brokerSymbol, nameof(brokerSymbol));
            Validate.DecimalNotOutOfRange(tickSize, nameof(tickSize), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);
            Validate.DecimalNotOutOfRange(tickValue, nameof(tickValue), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);
            Validate.Int32NotOutOfRange(targetDirectSpread, nameof(targetDirectSpread), 0, int.MaxValue, RangeEndPoints.Exclusive);
            Validate.Int32NotOutOfRange(contractSize, nameof(contractSize), 0, int.MaxValue, RangeEndPoints.Exclusive);
            Validate.Int32NotOutOfRange(minStopDistanceEntry, nameof(minStopDistanceEntry), 0, int.MaxValue);
            Validate.Int32NotOutOfRange(minLimitDistanceEntry, nameof(minLimitDistanceEntry), 0, int.MaxValue);
            Validate.Int32NotOutOfRange(minStopDistance, nameof(minStopDistance), 0, int.MaxValue);
            Validate.Int32NotOutOfRange(minLimitDistance, nameof(minLimitDistance), 0, int.MaxValue);
            Validate.DecimalNotOutOfRange(minStopDistanceEntry, nameof(this.MinStopDistanceEntry), decimal.Zero, decimal.MaxValue);
            Validate.NotDefault(timestamp, nameof(timestamp));

            this.Symbol = symbol;
            this.BrokerSymbol = brokerSymbol;
            this.QuoteCurrency = quoteCurrency;
            this.SecurityType = securityType;
            this.TickSize = tickSize;
            this.TickValue = tickValue;
            this.TargetDirectSpread = targetDirectSpread;
            this.ContractSize = contractSize;
            this.MinStopDistanceEntry = minStopDistanceEntry;
            this.MinLimitDistanceEntry = minLimitDistanceEntry;
            this.MinStopDistance = minStopDistance;
            this.MinLimitDistance = minLimitDistance;
            this.MinTradeSize = minTradeSize;
            this.MaxTradeSize = maxTradeSize;
            this.MarginRequirement = marginRequirement;
            this.RolloverInterestBuy = rolloverInterestBuy;
            this.RolloverInterestSell = rolloverInterestSell;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Instrument"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        private Instrument(SerializationInfo info, StreamingContext context)
            : base(
                new EntityId(info.GetString("InstrumentId")),
                (info.GetString("Timestamp").ToZonedDateTimeFromIso()))
        {
            Debug.NotNull(info, nameof(info));

            this.Symbol = new Symbol(info.GetString("SymbolCode"), (Exchange)info.GetValue("SymbolExchange", typeof(Exchange)));
            this.BrokerSymbol = new EntityId(info.GetString("BrokerSymbol"));
            this.QuoteCurrency = (CurrencyCode)info.GetValue(nameof(this.QuoteCurrency), typeof(CurrencyCode));
            this.SecurityType = (SecurityType)info.GetValue(nameof(this.SecurityType), typeof(SecurityType));
            this.TickSize = info.GetDecimal(nameof(this.TickSize));
            this.TickValue = info.GetDecimal(nameof(this.TickValue));
            this.TargetDirectSpread = info.GetInt32(nameof(this.TargetDirectSpread));
            this.ContractSize = info.GetInt32(nameof(this.ContractSize));
            this.MinStopDistanceEntry = info.GetInt32(nameof(this.MinStopDistanceEntry));
            this.MinLimitDistanceEntry = info.GetInt32(nameof(this.MinLimitDistanceEntry));
            this.MinStopDistance = info.GetInt32(nameof(this.MinStopDistance));
            this.MinLimitDistance = info.GetInt32(nameof(this.MinLimitDistance));
            this.MinTradeSize = info.GetInt32(nameof(this.MinTradeSize));
            this.MaxTradeSize = info.GetInt32(nameof(this.MaxTradeSize));
            this.MarginRequirement = info.GetDecimal(nameof(this.MarginRequirement));
            this.RolloverInterestBuy = info.GetDecimal(nameof(this.RolloverInterestBuy));
            this.RolloverInterestSell = info.GetDecimal(nameof(this.RolloverInterestSell));
        }

        /// <summary>
        /// Gets the instruments symbol.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// Gets the instruments identifier.
        /// </summary>
        public EntityId InstrumentId => this.EntityId;

        /// <summary>
        /// Gets the instruments broker symbol.
        /// </summary>
        public EntityId BrokerSymbol { get; }

        /// <summary>
        /// Gets the instruments quote currency.
        /// </summary>
        public CurrencyCode QuoteCurrency { get; }

        /// <summary>
        /// Gets the instruments security type.
        /// </summary>
        public SecurityType SecurityType { get; }

        /// <summary>
        /// Gets the instruments tick size.
        /// </summary>
        public decimal TickSize { get; }

        /// <summary>
        /// Gets the instruments tick value.
        /// </summary>
        public decimal TickValue { get; }

        /// <summary>
        /// Gets the instruments target direct spread.
        /// </summary>
        public int TargetDirectSpread { get; }

        /// <summary>
        /// Gets the instruments contract size.
        /// </summary>
        public int ContractSize { get; }

        /// <summary>
        /// Gets the instruments minimum stop distance for entry.
        /// </summary>
        public int MinStopDistanceEntry { get; }

        /// <summary>
        /// Gets the instruments minimum limit distance for entry.
        /// </summary>
        public int MinLimitDistanceEntry { get; }

        /// <summary>
        /// Gets the instruments minimum stop distance.
        /// </summary>
        public int MinStopDistance { get; }

        /// <summary>
        /// Gets the instruments minimum limit distance.
        /// </summary>
        public int MinLimitDistance { get; }

        /// <summary>
        /// Gets the instruments minimum trade size.
        /// </summary>
        public int MinTradeSize { get; }

        /// <summary>
        /// Gets the instruments maximum trade size.
        /// </summary>
        public int MaxTradeSize { get; }

        /// <summary>
        /// Gets the instruments margin requirement.
        /// </summary>
        public decimal MarginRequirement { get; }

        /// <summary>
        /// Gets the instruments rollover interest for long positions.
        /// </summary>
        public decimal RolloverInterestBuy { get; }

        /// <summary>
        /// Gets the instruments rollover interest for short positions.
        /// </summary>
        public decimal RolloverInterestSell { get; }

        /// <summary>
        /// The instruments creation timestamp.
        /// </summary>
        public ZonedDateTime Timestamp => this.EntityTimestamp;

        /// <summary>
        /// Gets the object data for serialization.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The context.</param>
        /// <exception cref="ValidationException">Throws if the info is null.</exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Validate.NotNull(info, nameof(info));

            info.AddValue("SymbolCode", this.Symbol.Code);
            info.AddValue("SymbolExchange", this.Symbol.Exchange);
            info.AddValue(nameof(this.InstrumentId), this.InstrumentId.Value);
            info.AddValue(nameof(this.BrokerSymbol), this.BrokerSymbol.Value);
            info.AddValue(nameof(this.QuoteCurrency), this.QuoteCurrency);
            info.AddValue(nameof(this.SecurityType), this.SecurityType);
            info.AddValue(nameof(this.TickSize), this.TickSize);
            info.AddValue(nameof(this.TickValue), this.TickValue);
            info.AddValue(nameof(this.TargetDirectSpread), this.TargetDirectSpread);
            info.AddValue(nameof(this.ContractSize), this.ContractSize);
            info.AddValue(nameof(this.MinStopDistanceEntry), this.MinStopDistanceEntry);
            info.AddValue(nameof(this.MinLimitDistanceEntry), this.MinLimitDistanceEntry);
            info.AddValue(nameof(this.MinStopDistance), this.MinStopDistance);
            info.AddValue(nameof(this.MinLimitDistance), this.MinLimitDistance);
            info.AddValue(nameof(this.MinTradeSize), this.MinTradeSize);
            info.AddValue(nameof(this.MaxTradeSize), this.MaxTradeSize);
            info.AddValue(nameof(this.MarginRequirement), this.MarginRequirement);
            info.AddValue(nameof(this.RolloverInterestBuy), this.RolloverInterestBuy);
            info.AddValue(nameof(this.RolloverInterestSell), this.RolloverInterestSell);
            info.AddValue(nameof(this.Timestamp), this.Timestamp.ToIsoString());
        }

        /// <summary>
        /// Returns a value indicating whether this <see cref="Instrument"/> is equal to the
        /// specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>A <see cref="bool"/>.</returns>
        public override bool Equals(object obj) => obj is Instrument otherInstrument && otherInstrument.Symbol.Equals(this.Symbol);

        /// <summary>
        /// Returns the hash code of the <see cref="Instrument"/>.
        /// </summary>
        /// <returns>A <see cref="int"/>.</returns>
        public override int GetHashCode() => this.Symbol.GetHashCode();

        /// <summary>
        /// Returns a string representation of the <see cref="Instrument"/>.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString() => this.Symbol.ToString();
    }
}
