//--------------------------------------------------------------------------------------------------
// <copyright file="AlphaStrategyModule.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Strategy
{
    using System.Collections.Generic;
    using Akka.Util.Internal;
    using Nautilus.Core;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.AlphaModel.Signal;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// The <see cref="AlphaStrategyModule"/> class. Contains the alpha signal processors for market
    /// data events. Any generated signals are sent to the Portfolio service via the messaging
    /// service.
    /// </summary>
    public sealed class AlphaStrategyModule : ActorComponentBusConnectedBase
    {
        private readonly BarStore barStore;
        private readonly BarStore barStoreDaily;
        private readonly ISignalLogic signalLogic;
        private readonly MarketDataProvider marketDataProvider;
        private readonly EntrySignalGenerator entrySignalGenerator;
        private readonly ExitSignalGenerator exitSignalGenerator;
        private readonly TrailingStopSignalGenerator trailingStopSignalGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlphaStrategyModule"/> class.
        /// </summary>
        /// <param name="container">The module setup container.</param>
        /// <param name="messagingAdapter">The module messaging adapter.</param>
        /// <param name="alphaStrategy">The module alpha strategy.</param>
        /// <param name="barStore">The module bar store.</param>
        /// <param name="barStoreDaily">The module daily bar store.</param>
        /// <param name="marketDataProvider">The module market data provider.</param>
        /// <param name="entrySignalGenerator">The module entry signal generator.</param>
        /// <param name="exitSignalGenerator">The module exit signal generator.</param>
        /// <param name="trailingStopSignalGenerator">The module trailing stop signal generator.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public AlphaStrategyModule(
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter,
            IAlphaStrategy alphaStrategy,
            BarStore barStore,
            BarStore barStoreDaily,
            MarketDataProvider marketDataProvider,
            EntrySignalGenerator entrySignalGenerator,
            ExitSignalGenerator exitSignalGenerator,
            TrailingStopSignalGenerator trailingStopSignalGenerator)
            : base(
            BlackBoxService.AlphaModel,
            LabelFactory.Component(
                nameof(AlphaStrategyModule),
                alphaStrategy.Instrument.Symbol,
                alphaStrategy.TradeProfile.TradeType),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(alphaStrategy, nameof(alphaStrategy));
            Validate.NotNull(barStore, nameof(barStore));
            Validate.NotNull(barStoreDaily, nameof(barStoreDaily));
            Validate.NotNull(marketDataProvider, nameof(marketDataProvider));
            Validate.NotNull(entrySignalGenerator, nameof(entrySignalGenerator));
            Validate.NotNull(exitSignalGenerator, nameof(exitSignalGenerator));
            Validate.NotNull(trailingStopSignalGenerator, nameof(trailingStopSignalGenerator));

            this.barStore = barStore;
            this.barStoreDaily = barStoreDaily;
            this.signalLogic = alphaStrategy.SignalLogic;
            this.marketDataProvider = marketDataProvider;
            this.entrySignalGenerator = entrySignalGenerator;
            this.exitSignalGenerator = exitSignalGenerator;
            this.trailingStopSignalGenerator = trailingStopSignalGenerator;

            this.InitializeAlgorithms(alphaStrategy);
            this.SetupEventMessageHandling();
        }

        private void SetupEventMessageHandling()
        {
            this.Receive<BarDataEvent>(msg => this.OnMessage(msg));
        }

        private void InitializeAlgorithms(IAlphaStrategy alphaStrategy)
        {
            Debug.NotNull(alphaStrategy, nameof(alphaStrategy));

            alphaStrategy.EntryStopAlgorithm.Initialize(
                this.barStore,
                this.marketDataProvider);

            alphaStrategy.StopLossAlgorithm.Initialize(
                this.barStore,
                this.marketDataProvider);

            alphaStrategy.ProfitTargetAlgorithm?.Initialize(
                this.barStore,
                this.marketDataProvider);

            alphaStrategy.EntryAlgorithms?.ForEach(a => a.Initialize(
                this.barStore,
                this.marketDataProvider));

            alphaStrategy.TrailingStopAlgorithms?.ForEach(a => a.Initialize(
                this.barStore,
                this.marketDataProvider));

            alphaStrategy.ExitAlgorithms?.ForEach(a => a.Initialize(
                this.barStore,
                this.marketDataProvider));
        }

        private void OnMessage(BarDataEvent message)
        {
            Debug.NotNull(message, nameof(message));

            this.LogReceivedMarketData();

            if (message.BarType.Specification.IsOneDayBar)
            {
                this.UpdateDailyBarStore(message);

                // A daily bar was processed, no need to continue the method.
                return;
            }

            this.UpdateBarStore(message);

            if (message.IsHistorical)
            {
                // Historical bars don't trigger signal processing.
                return;
            }

            this.marketDataProvider.Update(message.LastTick, message.AverageSpread);
            this.ProcessSignals();
        }

        private void LogReceivedMarketData()
        {
            this.Log.Debug("Received MarketDataEvent");
        }

        private void UpdateDailyBarStore(BarDataEvent message)
        {
            Debug.NotNull(message, nameof(message));

            this.barStoreDaily.Update(message.Bar);
            this.Log.Debug("BarStore updated (daily bar)");
        }

        private void UpdateBarStore(BarDataEvent message)
        {
            Debug.NotNull(message, nameof(message));
            Debug.EqualTo(message.BarType.Specification, nameof(message.BarType.Specification), this.barStore.BarSpecification);

            this.barStore.Update(message.Bar);
            this.entrySignalGenerator.Update(message.Bar);
            this.exitSignalGenerator.Update(message.Bar);
            this.trailingStopSignalGenerator.Update(message.Bar);

            this.Log.Debug($"BarStore updated (intraday bar)");
        }

        private void ProcessSignals()
        {
            this.Log.Debug($"Processing signals...");

            var entrySignalsBuy = this.entrySignalGenerator.ProcessBuy();
            var entrySignalsSell = this.entrySignalGenerator.ProcessSell();

            var exitSignalLong = this.exitSignalGenerator.ProcessLong();
            var exitSignalShort = this.exitSignalGenerator.ProcessShort();

            this.ExecuteTrailingStopSignals();

            if (this.signalLogic.IsValidBuySignal(
                entrySignalsBuy,
                entrySignalsSell,
                exitSignalLong))
            {
                this.ExecuteEntrySignals(entrySignalsBuy);
            }

            if (this.signalLogic.IsValidSellSignal(
                entrySignalsBuy,
                entrySignalsSell,
                exitSignalShort))
            {
                this.ExecuteEntrySignals(entrySignalsSell);
            }

            this.ExecuteExitSignals(exitSignalLong, exitSignalShort);
        }

        private void ExecuteTrailingStopSignals()
        {
            var trailingStopSignalLong = this.trailingStopSignalGenerator.ProcessLong();

            if (trailingStopSignalLong.HasValue)
            {
                var signalEvent = new SignalEvent(
                    trailingStopSignalLong.Value,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(BlackBoxService.Portfolio, signalEvent);
            }

            if (trailingStopSignalLong.HasNoValue)
            {
                this.Log.Information("Trailing stop signals LONG count=0...");
            }

            var trailingStopSignalShort = this.trailingStopSignalGenerator.ProcessShort();

            if (trailingStopSignalShort.HasValue)
            {
                var signalEvent = new SignalEvent(
                    trailingStopSignalShort.Value,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(BlackBoxService.Portfolio, signalEvent);
            }

            if (trailingStopSignalShort.HasNoValue)
            {
                this.Log.Information("Trailing stop signals SHORT count=0...");
            }
        }

        private void ExecuteEntrySignals(IReadOnlyCollection<EntrySignal> entrySignals)
        {
            Debug.NotNull(entrySignals, nameof(entrySignals));

            foreach (var entrySignal in entrySignals)
            {
                var signalEvent = new SignalEvent(
                    entrySignal,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(BlackBoxService.Portfolio, signalEvent);
            }
        }

        private void ExecuteExitSignals(
            Option<ExitSignal> exitSignalLong,
            Option<ExitSignal> exitSignalShort)
        {
            Debug.NotDefault(exitSignalLong, nameof(exitSignalLong));
            Debug.NotDefault(exitSignalShort, nameof(exitSignalShort));

            if (exitSignalLong.HasNoValue)
            {
                this.Log.Information($"Exit signals LONG count=0...");
            }

            if (exitSignalLong.HasValue)
            {
                var signalEvent = new SignalEvent(
                    exitSignalLong.Value,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(BlackBoxService.Portfolio, signalEvent);
            }

            if (exitSignalShort.HasNoValue)
            {
                this.Log.Information("Exit signals SHORT count=0...");
            }

            if (exitSignalShort.HasValue)
            {
                var signalEvent = new SignalEvent(
                    exitSignalShort.Value,
                    this.NewGuid(),
                    this.TimeNow());

                this.Send(BlackBoxService.Portfolio, signalEvent);
            }
        }
    }
}
