//--------------------------------------------------------------
// <copyright file="SecurityPortfolioFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio
{
    using Akka.Actor;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.BlackBox.Portfolio.Orders;
    using Nautilus.BlackBox.Portfolio.Processors;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Entities;

    /// <summary>
    /// The immutable static <see cref="SecurityPortfolioFactory"/> class.
    /// </summary>
    [Immutable]
    public static class SecurityPortfolioFactory
    {
        /// <summary>
        /// Creates a new <see cref="SecurityPortfolio"/> and returns its <see cref="IActorRef"/>
        /// address.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="instrument">The instrument.</param>
        /// <param name="actorContext">The actor context.</param>
        /// <returns>A <see cref="IActorRef"/>.</returns>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public static IActorRef Create(
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            Instrument instrument,
            IUntypedActorContext actorContext)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(instrument, nameof(instrument));
            Validate.NotNull(actorContext, nameof(actorContext));

            var tradeBook = new TradeBook(setupContainer, instrument.Symbol);

            var orderExpiryController = new OrderExpiryController(
                setupContainer,
                messagingAdapter,
                instrument.Symbol);

            var entrySignalProcessor = new EntrySignalProcessor(
                setupContainer,
                messagingAdapter,
                instrument,
                tradeBook);

            var exitSignalProcessor = new ExitSignalProcessor(
                setupContainer,
                messagingAdapter,
                instrument,
                tradeBook);

            var trailingStopSignalProcessor = new TrailingStopSignalProcessor(
                setupContainer,
                messagingAdapter,
                instrument,
                tradeBook);

            return actorContext.ActorOf(Props.Create(() => new SecurityPortfolio(
                setupContainer,
                messagingAdapter,
                instrument,
                tradeBook,
                orderExpiryController,
                entrySignalProcessor,
                exitSignalProcessor,
                trailingStopSignalProcessor)));
        }
    }
}
