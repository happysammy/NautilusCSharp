//--------------------------------------------------------------------------------------------------
// <copyright file="ExitSignalProcessor.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio.Processors
{
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Common.Commands;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// The sealed <see cref="ExitSignalProcessor"/> class.
    /// </summary>
    public class ExitSignalProcessor : ComponentBusConnectedBase
    {
        private readonly ITradeBook tradeBook;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitSignalProcessor"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="instrument">The instrument.</param>
        /// <param name="tradeBook">The trade book.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public ExitSignalProcessor(
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter,
            Instrument instrument,
            ITradeBook tradeBook)
            : base(
            BlackBoxService.Portfolio,
            LabelFactory.Component(nameof(ExitSignalProcessor), instrument.Symbol),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(instrument, nameof(instrument));
            Validate.NotNull(tradeBook, nameof(tradeBook));

            this.tradeBook = tradeBook;
        }

        /// <summary>
        /// Processes the given exit signal.
        /// </summary>
        /// <param name="signal">The exit signal.</param>
        /// <exception cref="ValidationException">Throws if the signal is null.</exception>
        public void Process(ExitSignal signal)
        {
            Validate.NotNull(signal, nameof(signal));

            var tradeType = signal.TradeType;
            var trades = this.tradeBook.GetTradesByTradeType(tradeType);

            foreach (var trade in trades) // TODO: refactor this nesting.
            {
                if (this.IsValidSignalForTrade(trade, signal))
                {
                    foreach (var tradeUnit in trade.TradeUnits)
                    {
                        foreach (var forUnit in signal.ForUnit)
                        {
                            if (IsValidSignalForUnit(forUnit, tradeUnit))
                            {
                                var closePosition = new ClosePosition(
                                    tradeUnit.Position,
                                    this.NewGuid(),
                                    this.TimeNow());

                                this.Send(BlackBoxService.Execution, closePosition);
                            }
                        }
                    }
                }
            }
        }

        private static bool IsValidSignalForUnit(int forUnit, TradeUnit tradeUnit)
        {
            Debug.Int32NotOutOfRange(forUnit, nameof(forUnit), 0, int.MaxValue);
            Debug.NotNull(tradeUnit, nameof(tradeUnit));

            return (forUnit == 0 || tradeUnit.Label.ToString() != "U" + forUnit)
                 && tradeUnit.Position.MarketPosition != MarketPosition.Flat;
        }

        private bool IsValidSignalForTrade(Trade trade, ExitSignal signal)
        {
            Debug.NotNull(trade, nameof(trade));
            Debug.NotNull(signal, nameof(signal));

            if (trade.TradeStatus != TradeStatus.Active)
            {
                this.Log.Debug(
                    $"Exit Signal {signal.ForMarketPosition}-{trade.TradeType} ignored... "
                  + $"(no {trade.TradeType} trades active, TradeStatus={trade.TradeStatus}, MarketPosition={trade.MarketPosition})");

                return false;
            }

            if (trade.MarketPosition != signal.ForMarketPosition)
            {
                this.Log.Debug(
                    $"Exit Signal {signal.ForMarketPosition}-{trade.TradeType} ignored... "
                  + $"(signal MarketPosition does not equal trade MarketPosition={trade.MarketPosition})");

                return false;
            }

            if (trade.TradeTimestamp == signal.SignalTimestamp)
            {
                this.Log.Debug(
                    $"Exit Signal {signal.ForMarketPosition}-{trade.TradeType} ignored... "
                  + $"(signal time {signal.SignalTimestamp} coincides with trade timestamp {trade.TradeTimestamp}");

                return false;
            }

            return true;
        }
    }
}
