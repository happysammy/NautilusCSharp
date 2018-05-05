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
    using NautechSystems.CSharp;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Componentry;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Orders;
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
            BlackBoxService.Portfolio,
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
        /// Creates and returns a new <see cref="AtomicOrderPacket"/> based on the given inputs
        /// (optional value).
        /// </summary>
        /// <param name="signal">The entry signal.</param>
        /// <param name="cashBalance">The cash balance.</param>
        /// <param name="riskPerTrade">The risk per trade.</param>
        /// <param name="hardLimit">The hard limit.</param>
        /// <param name="exchangeRate">The exchange rate.</param>
        /// <returns>A <see cref="AtomicOrderPacket"/>.</returns>
        /// <exception cref="ValidationException">Throws if any class argument is null, or if the
        /// exchange rate is zero or negative.</exception>
        public Option<AtomicOrderPacket> Create(
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

                return Option<AtomicOrderPacket>.None();
            }

            var timeInForce = signal.ExpireTime.HasValue ? TimeInForce.GTD : TimeInForce.DAY;

            var unit = 0;

            var atomicOrders = new List<AtomicOrder>();

            for (int i = 0; i < signal.TradeProfile.Units; i++)
            {
                unit++;

                var entryOrder = new StopMarketOrder(
                    signal.Symbol,
                    this.orderIdFactory.Create(signal.SignalTimestamp),
                    LabelFactory.EntryOrder(signal.SignalLabel, unit),
                    signal.OrderSide,
                    positionSize,
                    signal.EntryPrice,
                    timeInForce,
                    signal.ExpireTime,
                    signal.SignalTimestamp);

                var stoplossOrder = new StopMarketOrder(
                    signal.Symbol,
                    this.orderIdFactory.Create(signal.SignalTimestamp),
                    LabelFactory.StopLossOrder(signal.SignalLabel, unit),
                    GetOppositeSide(signal.OrderSide),
                    positionSize,
                    signal.StopLossPrice,
                    TimeInForce.GTC,
                    Option<ZonedDateTime?>.None(),
                    signal.SignalTimestamp);

                var profitTargetOrder = Option<StopOrder>.None();

                if (signal.ProfitTargets.Count >= unit)
                {
                    profitTargetOrder = new StopLimitOrder(
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
                    stoplossOrder,
                    profitTargetOrder);

                atomicOrders.Add(atomicOrder);
            }

            return Option<AtomicOrderPacket>.Some(new AtomicOrderPacket(
                signal.Symbol,
                signal.TradeType,
                atomicOrders,
                signal.SignalId,
                signal.SignalTimestamp));
        }

        // Orderside cannot be Undefined (as it was already checked by the signal).
        private static OrderSide GetOppositeSide(OrderSide orderSide)
        {
            return orderSide == OrderSide.Buy ? OrderSide.Sell : OrderSide.Buy;
        }
    }
}
