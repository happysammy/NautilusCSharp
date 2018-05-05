//--------------------------------------------------------------------------------------------------
// <copyright file="MarketDataPort.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Data.Market
{
    using System.Collections.Generic;
    using Akka.Actor;
    using NautechSystems.CSharp.Annotations;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The immutable sealed <see cref="MarketDataPort"/> class.
    /// </summary>
    [Immutable]
    public sealed class MarketDataPort : ActorComponentBusConnectedBase
    {
        private IDictionary<Symbol, IActorRef> marketDataProcessorsIndex = new Dictionary<Symbol, IActorRef>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketDataPort"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.
        /// </param>
        /// <exception cref="ValidationException">Throws if either argument is null.</exception>
        public MarketDataPort(
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter)
            : base(
            BlackBoxService.Data,
            new Label(nameof(MarketDataPort)),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
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
        /// Sets up all <see cref="DocumentMessage"/> handling methods.
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

            this.Log.Debug($"MarketDataProcessorsIndex updated");
        }
    }
}
