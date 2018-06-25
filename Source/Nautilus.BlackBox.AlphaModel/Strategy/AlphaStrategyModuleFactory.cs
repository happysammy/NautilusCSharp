// -------------------------------------------------------------------------------------------------
// <copyright file="AlphaStrategyModuleFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Strategy
{
    using Akka.Actor;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.AlphaModel.Signal;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The immutable static <see cref="AlphaStrategyModuleFactory"/> class. Creates modules for the
    /// <see cref="BlackBox"/> system based on the given inputs.
    /// </summary>
    [Immutable]
    public static class AlphaStrategyModuleFactory
    {
        /// <summary>
        /// Creates a new <see cref="AlphaStrategyModule"/> and returns its <see cref="IActorRef"/>
        /// address.
        /// </summary>
        /// <param name="container">The module setup container.</param>
        /// <param name="messagingAdapter">The module messaging adapter.</param>
        /// <param name="strategy">The module strategy.</param>
        /// <param name="actorContext">The module actor context.</param>
        /// <returns>A <see cref="IActorRef"/>.</returns>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public static IActorRef Create(
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter,
            IAlphaStrategy strategy,
            IUntypedActorContext actorContext)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(strategy, nameof(strategy));
            Validate.NotNull(actorContext, nameof(actorContext));

            var barStore = new BarStore(strategy.Instrument.Symbol, strategy.TradeProfile.BarSpecification);
            var barStoreDaily = new BarStore(strategy.Instrument.Symbol, new BarSpecification(QuoteType.Bid, Resolution.Day, 1));
            var marketDataProvider = new MarketDataProvider(strategy.Instrument.Symbol);

            var entrySignalGenerator = new EntrySignalGenerator(
                strategy.Instrument,
                strategy.TradeProfile,
                strategy.EntryAlgorithms,
                strategy.StopLossAlgorithm,
                strategy.ProfitTargetAlgorithm);

            var exitSignalGenerator = new ExitSignalGenerator(
                strategy.Instrument,
                strategy.TradeProfile,
                strategy.ExitAlgorithms);

            var trailingStopSignalGenerator = new TrailingStopSignalGenerator(
                strategy.Instrument,
                strategy.TradeProfile,
                strategy.TrailingStopAlgorithms);

            return actorContext.ActorOf(Props.Create(
                () => new AlphaStrategyModule(
                    container,
                    messagingAdapter,
                    strategy,
                    barStore,
                    barStoreDaily,
                    marketDataProvider,
                    entrySignalGenerator,
                    exitSignalGenerator,
                    trailingStopSignalGenerator)));
        }
    }
}
