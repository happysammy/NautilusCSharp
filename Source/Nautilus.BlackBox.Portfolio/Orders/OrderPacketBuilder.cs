//--------------------------------------------------------------------------------------------------
// <copyright file="OrderPacketBuilder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio.Orders
{
    using System.Collections.Generic;
    using Nautilus.Core;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The sealed <see cref="OrderPacketBuilder"/> class.
    /// </summary>
    public sealed class OrderPacketBuilder : ComponentBase
    {
        private readonly Instrument instrument;
        private readonly PositionSizer positionSizer;
        private readonly OrderIdFactory orderIdFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderPacketBuilder"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="instrument">The instrument.</param>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public OrderPacketBuilder(
            BlackBoxContainer container,
            Instrument instrument)
            : base(
            NautilusService.Portfolio,
            LabelFactory.Component(nameof(OrderPacketBuilder), instrument.Symbol),
            container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(instrument, nameof(instrument));

            this.instrument = instrument;
            this.positionSizer = new PositionSizer(container, instrument);
            this.orderIdFactory = new OrderIdFactory(instrument.Symbol);
        }

        /// <summary>
        /// Creates and returns a new <see cref="AtomicOrdersPacket"/> based on the given inputs
        /// (optional value).
        /// </summary>
        /// <param name="signal">The entry signal.</param>
        /// <param name="cashBalance">The cash balance.</param>
        /// <param name="riskPerTrade">The risk per trade.</param>
        /// <param name="hardLimit">The hard limit.</param>
        /// <param name="exchangeRate">The exchange rate.</param>
        /// <returns>A <see cref="AtomicOrdersPacket"/>.</returns>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if the
        /// exchange rate is zero or negative.</exception>
        public Option<AtomicOrdersPacket> Create(
            EntrySignal signal,
            Money cashBalance,
            Percentage riskPerTrade,
            Option<Quantity> hardLimit,
            decimal exchangeRate)
        {
            Validate.NotNull(signal, nameof(signal));
            Validate.NotNull(cashBalance, nameof(cashBalance));
            Validate.NotNull(riskPerTrade, nameof(riskPerTrade));
            Validate.NotNull(hardLimit, nameof(hardLimit));
            Validate.DecimalNotOutOfRange(exchangeRate, nameof(exchangeRate), decimal.Zero, decimal.MaxValue, RangeEndPoints.Exclusive);

            var tickValue = this.instrument.TickValue * exchangeRate;

            var positionSize = this.positionSizer.Calculate(
                signal,
                cashBalance,
                riskPerTrade,
                hardLimit,
                tickValue);

            if (positionSize.Value == 0)
            {
                this.Log.Warning("Calculated unit size exceeds risk model limit (unit size = 0)");

                return Option<AtomicOrdersPacket>.None();
            }

            var timeInForce = signal.ExpireTime.HasValue ? TimeInForce.GTD : TimeInForce.DAY;

            var unit = 0;

            var atomicOrders = new List<AtomicOrder>();

            for (int i = 0; i < signal.TradeProfile.Units; i++)
            {
                unit++;

                var entryOrder = OrderFactory.StopMarket(
                    signal.Symbol,
                    this.orderIdFactory.Create(signal.SignalTimestamp),
                    LabelFactory.EntryOrder(signal.SignalLabel, unit),
                    signal.OrderSide,
                    positionSize,
                    signal.EntryPrice,
                    timeInForce,
                    signal.ExpireTime,
                    signal.SignalTimestamp);

                var stopLossOrder = OrderFactory.StopMarket(
                    signal.Symbol,
                    this.orderIdFactory.Create(signal.SignalTimestamp),
                    LabelFactory.StopLossOrder(signal.SignalLabel, unit),
                    GetOppositeSide(signal.OrderSide),
                    positionSize,
                    signal.StopLossPrice,
                    TimeInForce.GTC,
                    Option<ZonedDateTime?>.None(),
                    signal.SignalTimestamp);

                var profitTargetOrder = Option<Order>.None();

                if (signal.ProfitTargets.Count >= unit)
                {
                    profitTargetOrder = OrderFactory.StopLimit(
                        signal.Symbol,
                        this.orderIdFactory.Create(signal.SignalTimestamp),
                        LabelFactory.ProfitTargetOrder(signal.SignalLabel, unit),
                        GetOppositeSide(signal.OrderSide),
                        positionSize,
                        signal.ProfitTargets[unit],
                        TimeInForce.GTC,
                        Option<ZonedDateTime?>.None(),
                        signal.SignalTimestamp);
                }

                var atomicOrder = new AtomicOrder(
                    signal.TradeType,
                    entryOrder,
                    stopLossOrder,
                    profitTargetOrder);

                atomicOrders.Add(atomicOrder);
            }

            return Option<AtomicOrdersPacket>.Some(new AtomicOrdersPacket(
                signal.Symbol,
                signal.TradeType,
                atomicOrders,
                new OrderPacketId(signal.Id.Value),
                signal.SignalTimestamp));
        }

        // OrderSide cannot be undefined (as it was already checked by the signal).
        private static OrderSide GetOppositeSide(OrderSide orderSide)
        {
            return orderSide == OrderSide.BUY ? OrderSide.SELL : OrderSide.BUY;
        }
    }
}
