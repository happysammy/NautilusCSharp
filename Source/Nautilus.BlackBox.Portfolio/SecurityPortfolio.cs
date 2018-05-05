//--------------------------------------------------------------------------------------------------
// <copyright file="SecurityPortfolio.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio
{
    using System;
    using Akka.Actor;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Messages.TradeCommands;
    using Nautilus.BlackBox.Portfolio.Orders;
    using Nautilus.BlackBox.Portfolio.Processors;
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Enums;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// The sealed <see cref="SecurityPortfolio"/> class.
    /// </summary>
    public sealed class SecurityPortfolio : ActorComponentBusConnectedBase
    {
        private readonly Instrument instrument;
        private readonly TradeBook tradeBook;
        private readonly OrderExpiryController orderExpiryController;
        private readonly EntrySignalProcessor entrySignalProcessor;
        private readonly ExitSignalProcessor exitSignalProcessor;
        private readonly TrailingStopSignalProcessor trailingStopSignalProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityPortfolio"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="instrument">The instrument.</param>
        /// <param name="tradeBook">The trade book.</param>
        /// <param name="orderExpiryController">The order expiry controller.</param>
        /// <param name="entrySignalProcessor">The entry signal processor.</param>
        /// <param name="exitSignalProcessor">The exit signal processor.</param>
        /// <param name="trailingStopSignalProcessor">The trailing stop signal processor.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public SecurityPortfolio(
            ComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            Instrument instrument,
            TradeBook tradeBook,
            OrderExpiryController orderExpiryController,
            EntrySignalProcessor entrySignalProcessor,
            ExitSignalProcessor exitSignalProcessor,
            TrailingStopSignalProcessor trailingStopSignalProcessor)
            : base(
            BlackBoxService.Portfolio,
            LabelFactory.Component(nameof(SecurityPortfolio), instrument.Symbol),
            container,
            messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(instrument, nameof(instrument));
            Validate.NotNull(tradeBook, nameof(tradeBook));
            Validate.NotNull(orderExpiryController, nameof(orderExpiryController));
            Validate.NotNull(entrySignalProcessor, nameof(entrySignalProcessor));
            Validate.NotNull(exitSignalProcessor, nameof(exitSignalProcessor));
            Validate.NotNull(trailingStopSignalProcessor, nameof(trailingStopSignalProcessor));

            this.instrument = instrument;
            this.tradeBook = tradeBook;
            this.orderExpiryController = orderExpiryController;
            this.entrySignalProcessor = entrySignalProcessor;
            this.exitSignalProcessor = exitSignalProcessor;
            this.trailingStopSignalProcessor = trailingStopSignalProcessor;

            this.SetupCommandMessageHandling();
            this.SetupEventMessageHandling();
        }

        /// <summary>
        /// Set up all <see cref="CommandMessage"/> handling methods.
        /// </summary>
        private void SetupCommandMessageHandling()
        {
            this.Receive<TradeApproved>(msg => this.OnMessage(msg));
        }

        /// <summary>
        /// Set up all <see cref="EventMessage"/> handling methods.
        /// </summary>
        private void SetupEventMessageHandling()
        {
            this.Receive<EventMessage>(msg => this.Self.Tell(msg.Event));
            this.Receive<MarketDataEvent>(msg => this.OnMessage(msg));
            this.Receive<SignalEvent>(msg => this.OnMessage(msg));

            this.Receive<OrderRejected>(msg => this.OnMessage(msg));
            this.Receive<OrderWorking>(msg => this.OnMessage(msg));
            this.Receive<OrderCancelled>(msg => this.OnMessage(msg));
            this.Receive<OrderFilled>(msg => this.OnMessage(msg));
            this.Receive<OrderPartiallyFilled>(msg => this.OnMessage(msg));
            this.Receive<OrderExpired>(msg => this.OnMessage(msg));
            this.Receive<OrderModified>(msg => this.OnMessage(msg));
        }

        private void OnMessage(TradeApproved @event)
        {
            Debug.NotNull(@event, nameof(@event));

            this.Execute(() =>
            {
                var newTrade = TradeFactory.Create(@event.OrderPacket);

                this.tradeBook.AddTrade(newTrade);

                this.orderExpiryController.AddCounters(@event.OrderPacket, @event.BarsValid);

                this.Send(
                    BlackBoxService.Execution,
                    new SubmitTrade(
                        @event.OrderPacket,
                        this.instrument.MinStopDistance,
                        this.NewGuid(),
                        this.TimeNow()));

                this.Log.Debug($"Received {@event}");
            });
        }

        private void OnMessage(MarketDataEvent @event)
        {
            Debug.NotNull(@event, nameof(@event));

            this.Execute(() =>
            {
                var activeOrderIds = this.tradeBook.GetAllActiveOrderIds();

                this.orderExpiryController.ProcessCounters(activeOrderIds);

                this.Log.Debug($"Received {@event}");
            });
        }

        private void OnMessage(SignalEvent @event)
        {
            Debug.NotNull(@event, nameof(@event));

            this.Execute(() =>
            {
                switch (@event.Signal)
                {
                    case EntrySignal entrySignal:
                        this.entrySignalProcessor.Process(entrySignal);
                        break;

                    case ExitSignal exitSignal:
                        this.exitSignalProcessor.Process(exitSignal);
                        break;

                    case TrailingStopSignal trailingStopSignal:
                        this.trailingStopSignalProcessor.Process(trailingStopSignal);
                        break;

                    default: throw new InvalidOperationException($"{@event.Signal} signal is not supported by the {nameof(SecurityPortfolio)}.", new InvalidOperationException());
                }
            });
        }

        private void OnMessage(OrderRejected @event)
        {
            Debug.NotNull(@event, nameof(@event));

            this.Execute(() =>
            {
                this.orderExpiryController.RemoveCounter(@event.OrderId);

                var result = this.tradeBook.GetTradeForOrder(@event.OrderId);

                if (result.IsSuccess)
                {
                    var trade = result.Value;

                    trade.Apply(@event);
                    this.tradeBook.Process(result.Value.TradeType);

                    this.Log.Debug($"Applied {@event} to ({trade.TradeId}), TradeStatus={trade.TradeStatus}, MarketPosition={trade.MarketPosition}");
                    this.Log.Warning($"{@event} ({@event.RejectedReason})");
                }
            });
        }

        private void OnMessage(OrderWorking @event)
        {
            this.Execute(() =>
            {
                Debug.NotNull(@event, nameof(@event));

                var result = this.tradeBook.GetTradeForOrder(@event.OrderId);

                if (result.IsSuccess)
                {
                    var trade = result.Value;

                    trade.Apply(@event);
                    this.tradeBook.Process(result.Value.TradeType);

                    this.Log.Debug($"Applied {@event} to ({trade.TradeId}), TradeStatus={trade.TradeStatus}, MarketPosition={trade.MarketPosition}");
                }
            });
        }

        private void OnMessage(OrderCancelled @event)
        {
            Debug.NotNull(@event, nameof(@event));

            this.Execute(() =>
            {
                this.orderExpiryController.RemoveCounter(@event.OrderId);

                var result = this.tradeBook.GetTradeForOrder(@event.OrderId);

                if (result.IsSuccess)
                {
                    var trade = result.Value;

                    trade.Apply(@event);
                    this.tradeBook.Process(result.Value.TradeType);

                    this.Log.Debug($"Applied {@event} to ({trade.TradeId}), TradeStatus={trade.TradeStatus}, MarketPosition={trade.MarketPosition}");
                }
            });
        }

        private void OnMessage(OrderFilled @event)
        {
            Debug.NotNull(@event, nameof(@event));

            this.Execute(() =>
            {
                this.orderExpiryController.RemoveCounter(@event.OrderId);

                var result = this.tradeBook.GetTradeForOrder(@event.OrderId);

                if (result.IsSuccess)
                {
                    var trade = result.Value;

                    trade.Apply(@event);
                    this.tradeBook.Process(result.Value.TradeType);

                    this.Log.Debug($"Applied {@event} to ({trade.TradeId}), TradeStatus={trade.TradeStatus}, MarketPosition={trade.MarketPosition}");
                }
            });
        }

        private void OnMessage(OrderPartiallyFilled @event)
        {
            Debug.NotNull(@event, nameof(@event));

            this.Execute(() =>
            {
                this.orderExpiryController.RemoveCounter(@event.OrderId);

                var result = this.tradeBook.GetTradeForOrder(@event.OrderId);

                if (result.IsSuccess)
                {
                    var trade = result.Value;

                    trade.Apply(@event);
                    this.tradeBook.Process(result.Value.TradeType);

                    this.Log.Debug($"Applied {@event} to ({trade.TradeId}), TradeStatus={trade.TradeStatus}, MarketPosition={trade.MarketPosition}");
                }
            });
        }

        private void OnMessage(OrderExpired @event)
        {
            Debug.NotNull(@event, nameof(@event));

            this.Execute(() =>
            {
                this.orderExpiryController.RemoveCounter(@event.OrderId);

                var result = this.tradeBook.GetTradeForOrder(@event.OrderId);

                if (result.IsSuccess)
                {
                    var trade = result.Value;

                    trade.Apply(@event);
                    this.tradeBook.Process(result.Value.TradeType);

                    this.Log.Debug($"Applied {@event} to ({trade.TradeId}), TradeStatus={trade.TradeStatus}, MarketPosition={trade.MarketPosition}");
                }
            });
        }

        private void OnMessage(OrderModified @event)
        {
            Debug.NotNull(@event, nameof(@event));

            this.Execute(() =>
            {
                var result = this.tradeBook.GetTradeForOrder(@event.OrderId);

                if (result.IsSuccess)
                {
                    var trade = result.Value;

                    trade.Apply(@event);
                    this.tradeBook.Process(result.Value.TradeType);

                    this.Log.Debug($"Applied {@event} to ({trade.TradeId}), TradeStatus={trade.TradeStatus}, MarketPosition={trade.MarketPosition}");
                }
            });
        }
    }
}
