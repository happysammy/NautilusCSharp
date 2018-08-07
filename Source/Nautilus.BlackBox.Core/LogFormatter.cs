//--------------------------------------------------------------------------------------------------
// <copyright file="LogFormatter.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Provides strings formatted for logging based on the given inputs.
    /// </summary>
    [Stateless]
    public static class LogFormatter
    {
        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="service">The black box service context.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(Enum service)
        {
            const int logStringLength = 10;

            if (service.ToString().Length >= logStringLength)
            {
                return service.ToString();
            }

            var lengthDifference = logStringLength - service.ToString().Length;

            var underscoreAppend = string.Empty;
            var builder = new System.Text.StringBuilder();
            builder.Append(underscoreAppend);

            for (var i = 0; i < lengthDifference; i++)
            {
                builder.Append("_");
            }

            underscoreAppend = builder.ToString();

            return service + underscoreAppend;
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="signal">The entry signal.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(EntrySignal signal)
        {
            Debug.NotNull(signal, nameof(signal));

            var expireTime = signal.ExpireTime.HasValue
                ? signal.ExpireTime.Value.ToIsoString()
                : string.Empty;

            return $"{signal.Symbol} {signal}: EntrySignal {signal.OrderSide}, "
                 + $"EntryPrice={signal.EntryPrice}, "
                 + $"StopLossPrice={signal.StopLossPrice}, "
                 + $"ProfitTargets({signal.ProfitTargets.Count})={GetPrint(signal.ProfitTargets)}, "
                 + $"SignalTimestamp={signal.SignalTimestamp.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} UTC, "
                 + $"ExpireTime={expireTime}";
        }

        private static string GetPrint(IReadOnlyDictionary<int, Price> profitTargets)
        {
            Debug.NotNull(profitTargets, nameof(profitTargets));

            if (profitTargets.Count == 0)
            {
                return "0";
            }

            return profitTargets.Values.Aggregate(string.Empty, (current, profitTarget) => ProfitTargetsPrintMaker(profitTarget, current));
        }

        private static string ProfitTargetsPrintMaker(Price profitTarget, string profitTargetsPrint)
        {
            Debug.NotNull(profitTarget, nameof(profitTarget));
            Debug.NotNull(profitTargetsPrint, nameof(profitTargetsPrint));

            if (profitTargetsPrint == string.Empty)
            {
                return profitTarget.ToString();
            }

            return profitTargetsPrint + ", " + profitTarget;
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="signal">The exit signal.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(ExitSignal signal)
        {
            Debug.NotNull(signal, nameof(signal));

            return $"{signal.Symbol} "
                 + $"{signal}: ExitSignal for {signal.ForMarketPosition} {signal.TradeType} trades, "
                 + $"Unit(s)={GetForUnitPrint(signal.ForUnit)}";
        }

        private static string GetForUnitPrint(IReadOnlyList<int> forUnit)
        {
            Debug.NotNull(forUnit, nameof(forUnit));

            return forUnit.Aggregate(string.Empty, (current, unit) => ForUnitPrintMaker(unit, current));
        }

        private static string ForUnitPrintMaker(int unit, string forUnitPrint)
        {
            Debug.NotNull(forUnitPrint, nameof(forUnitPrint));

            if (forUnitPrint == string.Empty)
            {
                if (unit == 0)
                {
                    return "(ALL)";
                }

                return unit.ToString();
            }

            return forUnitPrint + ", " + unit;
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="signal">The trailing stop signal.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(TrailingStopSignal signal)
        {
            Debug.NotNull(signal, nameof(signal));

            return $"{signal.Symbol} "
                 + $"{signal}: TrailingStopSignal for {signal.ForMarketPosition}-{signal.TradeType} trades, "
                 + $"NewStopLoss={GetForUnitStopLossPrint(signal)}";
        }

        private static string GetForUnitStopLossPrint(TrailingStopSignal signal)
        {
            Debug.NotNull(signal, nameof(signal));

            if (signal.ForUnitStopLossPrices.Count == 1)
            {
                if (signal.ForUnitStopLossPrices.ContainsKey(0))
                {
                    return $"(ALL){signal.ForUnitStopLossPrices[0]}";
                }

                foreach (var kvp in signal.ForUnitStopLossPrices)
                {
                    return $"(U{kvp.Key}){kvp.Value}";
                }
            }

            return signal.ForUnitStopLossPrices.Aggregate(string.Empty, (current, unitStopLoss) => ForUnitStopLossPrintMaker(unitStopLoss, current));
        }

        private static string ForUnitStopLossPrintMaker(KeyValuePair<int, Price> unitStopLoss, string forUnitStopLossPrint)
        {
            Debug.NotNull(forUnitStopLossPrint, nameof(forUnitStopLossPrint));

            if (forUnitStopLossPrint == string.Empty)
            {
                return $"(U{unitStopLoss.Key}){unitStopLoss.Value}";
            }

            return $"{forUnitStopLossPrint}, (U{unitStopLoss.Key}){unitStopLoss.Value}";
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="bar">The bar.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(Bar bar)
        {
            Debug.NotNull(bar, nameof(bar));

            return $"Open={bar.Open}, "
                 + $"High={bar.High}, "
                 + $"Low={bar.Low}, "
                 + $"Close={bar.Close}, "
                 + $"Volume={bar.Volume}, "
                 + $"Timestamp={bar.Timestamp}";
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="tick">The tick.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(Tick tick)
        {
            Debug.NotNull(tick, nameof(tick));

            return $"{tick}: "
                 + $"Bid={tick.Bid}, "
                 + $"Ask={tick.Ask}, "
                 + $"Timestamp={tick.Timestamp.ToIsoString()}";
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="instrument">The instrument.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(Instrument instrument)
        {
            Debug.NotNull(instrument, nameof(instrument));

            return $"Instrument: {instrument.Symbol}, " +
                   $"BrokerSymbol={instrument.BrokerSymbol}, " +
                   $"QuoteCurrency={instrument.QuoteCurrency}, " +
                   $"SecurityType={instrument.SecurityType}, " +
                   $"TickDecimals={instrument.TickDecimals}, " +
                   $"TickSize={instrument.TickSize}, " +
                   $"TickValue={instrument.TickValue}, " +
                   $"MinStopDistance={instrument.MinStopDistance}, " +
                   $"MinLimitDistance={instrument.MinLimitDistance}, " +
                   $"MinStopDistanceEntry={instrument.MinStopDistanceEntry}, " +
                   $"MinLimitDistanceEntry={instrument.MinLimitDistanceEntry}, " +
                   $"MinQuantity={instrument.MinTradeSize}, " +
                   $"MaxQuantity={instrument.MaxTradeSize}, " +
                   $"InterestBuy={instrument.RolloverInterestBuy}, " +
                   $"InterestSell={instrument.RolloverInterestSell}, ";
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="order">The stop order.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(Order order)
        {
            Debug.NotNull(order, nameof(order));

            return $"{order}: {order.Side}-{order.Type} {order.Quantity.Value:N0} at {order.Price}, {order.Label}";
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="trade">The trade.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(Trade trade)
        {
            Debug.NotNull(trade, nameof(trade));

            return $"{trade}: Units={trade.TradeUnits.Count}, "
                 + $"TotalQuantity={trade.TotalQuantity.Value:N0}, "
                 + $"(TradeStatus={trade.TradeStatus}, "
                 + $"MarketPosition={trade.MarketPosition})";
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="model">The risk model.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(IRiskModel model)
        {
            Debug.NotNull(model, nameof(model));

            return $"RiskModel: GlobalMaxRiskExposure={model.GlobalMaxRiskExposure}, "
                 + $"GlobalMaxRiskPerTrade={model.GlobalMaxRiskPerTrade}, "
                 + $"PositionSizeHardLimits={model.PositionSizeHardLimits}, "
                 + $"EventCount={model.EventCount}, "
                 + $"LastEventTime={model.LastEventTime.ToIsoString()}";
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="strategy">The alpha strategy.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(IAlphaStrategy strategy)
        {
            Debug.NotNull(strategy, nameof(strategy));

            return $"Symbol={strategy.Instrument.Symbol}, "
                 + $"TradeProfile={strategy.TradeProfile}, "
                 + $"barType={strategy.TradeProfile.BarSpecification}, "
                 + $"TradePeriod={strategy.TradeProfile.TradePeriod}, "
                 + $"EntryAlgorithms={strategy.EntryAlgorithms.Count}, "
                 + $"TrailingStopLossAlgorithms={strategy.TrailingStopAlgorithms.Count}, "
                 + $"ExitAlgorithms={strategy.ExitAlgorithms.Count}";
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="event">The account event.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(AccountEvent @event)
        {
            Debug.NotNull(@event, nameof(@event));

            return $"{@event}: ({@event.Broker}-{@event.AccountNumber}) "
                 + $"CashBalance={@event.CashBalance}, "
                 + $"MarginUsedMaintenance={@event.MarginUsedMaintenance}, "
                 + $"MarginRatio={@event.MarginRatio}, "
                 + $"SignalTimestamp={@event.Timestamp}";
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="event">The market data event.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(BarDataEvent @event)
        {
            Debug.NotNull(@event, nameof(@event));

            return $"{@event.BarType.Symbol} MarketData: {@event.Bar}, "
                 + $"AverageSpread={@event.AverageSpread}, "
                 + $"IsHistoricalData={@event.IsHistorical}";
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="event">The order cancelled event.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(OrderCancelled @event)
        {
            Debug.NotNull(@event, nameof(@event));

            return $"{@event.Symbol} OrderCancelled: {@event.OrderId}, "
                 + $"CancelledTime={@event.CancelledTime}";
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="event">The order expired event.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(OrderExpired @event)
        {
            Debug.NotNull(@event, nameof(@event));

            return $"{@event.Symbol} OrderExpired: {@event.OrderId}, "
                 + $"ExpiredTime={@event.ExpiredTime}";
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="event">The order filled event.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(OrderFilled @event)
        {
            Debug.NotNull(@event, nameof(@event));

            return $"{@event.Symbol} OrderFilled: {@event.OrderId}, "
                 + $"ExecutionId={@event.ExecutionId}, "
                 + $"FilledQuantity={@event.FilledQuantity}, "
                 + $"AveragePrice={@event.AveragePrice}, "
                 + $"ExecutionTime={@event.ExecutionTime}";
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="event">The order modified event.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(OrderModified @event)
        {
            Debug.NotNull(@event, nameof(@event));

            return $"{@event.Symbol} OrderModified: {@event.OrderId}, "
                 + $"AcceptedTime={@event.ModifiedTime}";
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="event">The order partially filled event.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(OrderPartiallyFilled @event)
        {
            Debug.NotNull(@event, nameof(@event));

            return $"{@event.Symbol} OrderPartiallyFilled: {@event.OrderId}, "
                 + $"ExecutionId={@event.ExecutionId}, "
                 + $"FilledQuantity={@event.FilledQuantity}, "
                 + $"LeavesQuantity={@event.LeavesQuantity}, "
                 + $"AveragePrice={@event.AveragePrice}, "
                 + $"ExecutionTime={@event.ExecutionTime}";
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="event">The order rejected event.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(OrderRejected @event)
        {
            Debug.NotNull(@event, nameof(@event));

            return $"{@event.Symbol} OrderRejected: {@event.OrderId}, "
                 + $"RejectedTime={@event.RejectedTime}, "
                 + $"RejectedReason='{@event.RejectedReason}'";
        }

        /// <summary>
        /// Returns a formatted output string from the given input.
        /// </summary>
        /// <param name="event">The order working event.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string ToOutput(OrderWorking @event)
        {
            Debug.NotNull(@event, nameof(@event));

            var expireTimePrint = string.Empty;

            if (@event.ExpireTime.HasValue)
            {
                var expireTime = @event.ExpireTime.Value;
                expireTimePrint = $", ExpireTime={expireTime}";
            }

            return $"{@event.Symbol} OrderWorking: {@event.OrderId}, "
                 + $"AcceptedTime={@event.WorkingTime}{expireTimePrint}";
        }
    }
}
