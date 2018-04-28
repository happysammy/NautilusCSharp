//--------------------------------------------------------------
// <copyright file="MarketDataPort.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Data.Market
{
    using System.Collections.Generic;
    using Akka.Actor;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.BlackBox.Core;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The immutable sealed <see cref="MarketDataPort"/> class.
    /// </summary>
    [Immutable]
    public sealed class MarketDataPort : ActorComponentBase
    {
        private IDictionary<Symbol, IActorRef> marketDataProcessorsIndex = new Dictionary<Symbol, IActorRef>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDataPort"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.
        /// </param>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public MarketDataPort(
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter)
            : base(
            BlackBoxService.Data,
            new Label(nameof(MarketDataPort)),
            setupContainer,
            messagingAdapter)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            this.SetupEventMessageHandling();
            this.SetupServiceMessageHandling();
        }

        /// <summary>
        /// Sets up all <see cref="EventMessage"/> handling methods.
        /// </summary>
        private void SetupEventMessageHandling()
        {
            this.Receive<Tick>(msg => this.OnReceive(msg));
        }

        /// <summary>
        /// Sets up all <see cref="ServiceMessage"/> handling methods.
        /// </summary>
        private void SetupServiceMessageHandling()
        {
            this.Receive<MarketDataProcessorIndexUpdate>(msg => this.OnMessage(msg));
        }

        private void OnReceive(Tick quote)
        {
            Debug.NotNull(quote, nameof(quote));

            if (this.marketDataProcessorsIndex.ContainsKey(quote.Symbol))
            {
                this.marketDataProcessorsIndex[quote.Symbol].Tell(quote, this.Self);
            }
        }

        private void OnMessage(MarketDataProcessorIndexUpdate message)
        {
            Debug.NotNull(message, nameof(message));

            this.marketDataProcessorsIndex = message.MarketDataProcessorsIndex;

            this.Log(LogLevel.Debug, $"MarketDataProcessorsIndex updated");
        }
    }
}
