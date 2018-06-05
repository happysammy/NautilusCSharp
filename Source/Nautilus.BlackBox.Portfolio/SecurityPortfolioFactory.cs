//--------------------------------------------------------------------------------------------------
// <copyright file="SecurityPortfolioFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio
{
    using Akka.Actor;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Validation;
    using Nautilus.BlackBox.Core.Build;
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
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="instrument">The instrument.</param>
        /// <param name="actorContext">The actor context.</param>
        /// <returns>A <see cref="IActorRef"/>.</returns>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public static IActorRef Create(
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter,
            Instrument instrument,
            IUntypedActorContext actorContext)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(instrument, nameof(instrument));
            Validate.NotNull(actorContext, nameof(actorContext));

            var tradeBook = new TradeBook(container, instrument.Symbol);

            var orderExpiryController = new OrderExpiryController(
                container,
                messagingAdapter,
                instrument.Symbol);

            var entrySignalProcessor = new EntrySignalProcessor(
                container,
                messagingAdapter,
                instrument,
                tradeBook);

            var exitSignalProcessor = new ExitSignalProcessor(
                container,
                messagingAdapter,
                instrument,
                tradeBook);

            var trailingStopSignalProcessor = new TrailingStopSignalProcessor(
                container,
                messagingAdapter,
                instrument,
                tradeBook);

            return actorContext.ActorOf(Props.Create(() => new SecurityPortfolio(
                container,
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
