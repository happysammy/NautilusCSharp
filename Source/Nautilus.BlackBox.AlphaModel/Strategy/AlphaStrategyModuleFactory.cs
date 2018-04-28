// -------------------------------------------------------------------------------------------------
// <copyright file="AlphaStrategyModuleFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.AlphaModel.Strategy
{
    using Akka.Actor;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.AlphaModel.Signal;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Setup;
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
        /// <param name="setupContainer">The module setup container.</param>
        /// <param name="messagingAdapter">The module messaging adapter.</param>
        /// <param name="strategy">The module strategy.</param>
        /// <param name="actorContext">The module actor context.</param>
        /// <returns>A <see cref="IActorRef"/>.</returns>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public static IActorRef Create(
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            IAlphaStrategy strategy,
            IUntypedActorContext actorContext)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(strategy, nameof(strategy));
            Validate.NotNull(actorContext, nameof(actorContext));

            var barStore = new BarStore(strategy.Instrument.Symbol, strategy.TradeProfile.BarSpecification);
            var barStoreDaily = new BarStore(strategy.Instrument.Symbol, new BarSpecification(BarResolution.Day, 1));
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
                    setupContainer,
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
