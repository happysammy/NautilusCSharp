//--------------------------------------------------------------------------------------------------
// <copyright file="RiskModel.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.DomainModel.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Entities.Base;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The sealed <see cref="RiskModel"/> class. Represents a systems model for quantifying
    /// financial market risk exposure.
    /// </summary>
    public sealed class RiskModel : Entity<RiskModel>
    {
        private readonly IDictionary<Symbol, Quantity> positionSizeHardLimitsIndex = new Dictionary<Symbol, Quantity>();
        private readonly IDictionary<TradeType, Percentage> maxRiskPerTradeTypeIndex = new Dictionary<TradeType, Percentage>();
        private readonly IDictionary<TradeType, Quantity> maxTradesPerSymbolType = new Dictionary<TradeType, Quantity>();
        private readonly IList<Tuple<ZonedDateTime, string>> eventLog = new List<Tuple<ZonedDateTime, string>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RiskModel"/> class.
        /// </summary>
        /// <param name="riskModelId">The risk model identifier.</param>
        /// <param name="globalMaxRiskExposure">The global max risk exposure.</param>
        /// <param name="globalMaxRiskPerTrade">The global max risk per trade.</param>
        /// <param name="globalMaxTradesPerType">The global max trades per type.</param>
        /// <param name="positionSizeHardLimits">The position size hard limits.</param>
        /// <param name="timestamp">The risk model creation timestamp.</param>
        public RiskModel(
            RiskModelId riskModelId,
            Percentage globalMaxRiskExposure,
            Percentage globalMaxRiskPerTrade,
            Quantity globalMaxTradesPerType,
            bool positionSizeHardLimits,
            ZonedDateTime timestamp)
            : base(riskModelId, timestamp)
        {
            this.GlobalMaxRiskExposure = globalMaxRiskExposure;
            this.GlobalMaxRiskPerTrade = globalMaxRiskPerTrade;
            this.GlobalMaxTradesPerType = globalMaxTradesPerType;
            this.PositionSizeHardLimits = positionSizeHardLimits;

            this.eventLog.Add(Tuple.Create(timestamp, "Risk model created."));
        }

        /// <summary>
        /// Gets the risk models maximum risk exposure.
        /// </summary>
        public Percentage GlobalMaxRiskExposure { get; }

        /// <summary>
        /// Gets the risk models risk per trade percent.
        /// </summary>
        public Percentage GlobalMaxRiskPerTrade { get; private set; }

        /// <summary>
        /// Gets the risk models global max trades per type.
        /// </summary>
        public Quantity GlobalMaxTradesPerType { get; }

        /// <summary>
        /// Gets a value indicating whether position size hard limits.
        /// </summary>
        public bool PositionSizeHardLimits { get; }

        /// <summary>
        /// Gets the risk models last event time.
        /// </summary>
        public ZonedDateTime LastEventTime => this.eventLog[this.EventCount - 1].Item1;

        /// <summary>
        /// Gets the risk models event count.
        /// </summary>
        public int EventCount => this.eventLog.Count;

        /// <summary>
        /// Updates the risk models global max risk per trade.
        /// </summary>
        /// <param name="maxRiskPerTrade">The max risk per trade.</param>
        /// <param name="timestamp">The update timestamp.</param>
        public void UpdateGlobalMaxRiskPerTrade(Percentage maxRiskPerTrade, ZonedDateTime timestamp)
        {
            this.GlobalMaxRiskPerTrade = maxRiskPerTrade;
            this.eventLog.Add(Tuple.Create(timestamp, $"Global max risk per trade updated ({maxRiskPerTrade})."));
        }

        /// <summary>
        /// Updates the risk models position size hard limit.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="hardLimit">The hard limits.</param>
        /// <param name="timestamp">The update timestamp.</param>
        public void UpdatePositionSizeHardLimit(Symbol symbol, Quantity hardLimit, ZonedDateTime timestamp)
        {
            Debug.NotDefault(timestamp, nameof(timestamp));

            if (this.positionSizeHardLimitsIndex.ContainsKey(symbol))
            {
                this.positionSizeHardLimitsIndex[symbol] = hardLimit;
                this.eventLog.Add(Tuple.Create(timestamp, $"Position size hard limits for {symbol} updated ({hardLimit})."));

                return;
            }

            this.positionSizeHardLimitsIndex.Add(symbol, hardLimit);
            this.eventLog.Add(Tuple.Create(timestamp, $"Position size hard limits for {symbol} added ({hardLimit})."));
        }

        /// <summary>
        /// Updates the risk models max risk per trade type.
        /// </summary>
        /// <param name="tradeType">The trade type.</param>
        /// <param name="riskPerTrade">The risk per trade.</param>
        /// <param name="timestamp">The update timestamp.</param>
        public void UpdateMaxRiskPerTradeType(TradeType tradeType, Percentage riskPerTrade, ZonedDateTime timestamp)
        {
            Debug.NotDefault(timestamp, nameof(timestamp));

            if (this.maxRiskPerTradeTypeIndex.ContainsKey(tradeType))
            {
                this.maxRiskPerTradeTypeIndex[tradeType] = riskPerTrade;
                this.eventLog.Add(Tuple.Create(timestamp, $"Max risk per trade type for {tradeType} trades updated ({riskPerTrade})."));

                return;
            }

            this.maxRiskPerTradeTypeIndex.Add(tradeType, riskPerTrade);
            this.eventLog.Add(Tuple.Create(timestamp, $"Max risk per trade type for {tradeType} trades added ({riskPerTrade})."));
        }

        /// <summary>
        /// Updates the risk models maximum trades per type.
        /// </summary>
        /// <param name="tradeType">The trade type.</param>
        /// <param name="maxTrades">The max trades.</param>
        /// <param name="timestamp">The update timestamp.</param>
        public void UpdateMaxTradesPerSymbolType(TradeType tradeType, Quantity maxTrades, ZonedDateTime timestamp)
        {
            Debug.NotDefault(timestamp, nameof(timestamp));

            if (this.maxTradesPerSymbolType.ContainsKey(tradeType))
            {
                this.maxTradesPerSymbolType[tradeType] = maxTrades;
                this.eventLog.Add(Tuple.Create(timestamp, $"Maximum {tradeType} trades per symbol updated ({maxTrades})."));

                return;
            }

            this.maxTradesPerSymbolType.Add(tradeType, maxTrades);
            this.eventLog.Add(Tuple.Create(timestamp, $"Maximum {tradeType} trades per symbol added ({maxTrades})."));
        }

        /// <summary>
        /// Returns the risk models hard limit quantity for the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns>A <see cref="OptionRef{T}"/>.</returns>
        public OptionRef<Quantity> GetHardLimitQuantity(Symbol symbol)
        {
            return this.positionSizeHardLimitsIndex.ContainsKey(symbol)
                ? this.positionSizeHardLimitsIndex[symbol]
                : OptionRef<Quantity>.None();
        }

        /// <summary>
        /// Returns the risk models risk per trade for the given trade type.
        /// </summary>
        /// <param name="tradeType">The trade type.</param>
        /// <returns>A <see cref="Percentage"/>.</returns>
        public Percentage GetRiskPerTrade(TradeType tradeType)
        {
            return this.maxRiskPerTradeTypeIndex.ContainsKey(tradeType)
                ? this.maxRiskPerTradeTypeIndex[tradeType]
                : this.GlobalMaxRiskPerTrade;
        }

        /// <summary>
        /// Returns the maximum trades for the given trade type.
        /// </summary>
        /// <param name="tradeType">The trade type.</param>
        /// <returns>A <see cref="Quantity"/>.</returns>
        public Quantity GetMaxTrades(TradeType tradeType)
        {
            return this.maxTradesPerSymbolType.ContainsKey(tradeType)
                 ? this.maxTradesPerSymbolType[tradeType]
                 : this.GlobalMaxTradesPerType;
        }

        /// <summary>
        /// Returns the event log.
        /// </summary>
        /// <returns>A read only dictionary.</returns>
        public IReadOnlyList<Tuple<ZonedDateTime, string>> GetEventLog() =>
            this.eventLog.ToImmutableList();
    }
}
