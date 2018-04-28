//--------------------------------------------------------------
// <copyright file="EntrySignalProcessor.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.BlackBox.Portfolio.Processors
{
    using System;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.BlackBox.Core.Messages.SystemCommands;
    using Nautilus.BlackBox.Core.Setup;
    using Nautilus.BlackBox.Portfolio.Orders;
    using Nautilus.BlackBox.Core;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Messaging;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// The sealed <see cref="EntrySignalProcessor"/> class.
    /// </summary>
    public sealed class EntrySignalProcessor : ComponentBusConnectedBase
    {
        private readonly Instrument instrument;
        private readonly OrderPacketBuilder orderPacketBuilder;
        private readonly IQuoteProvider quoteProvider;
        private readonly ITradeBook tradeBook;
        private readonly IReadOnlyBrokerageAccount account;
        private readonly IReadOnlyRiskModel riskModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntrySignalProcessor"/> class.
        /// </summary>
        /// <param name="setupContainer">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="instrument">The instrument.</param>
        /// <param name="tradeBook">The trade book.</param>
        /// <exception cref="ValidationException">Throws if any argument is null.</exception>
        public EntrySignalProcessor(
            BlackBoxSetupContainer setupContainer,
            IMessagingAdapter messagingAdapter,
            Instrument instrument,
            ITradeBook tradeBook)
            : base(
            BlackBoxService.Portfolio,
            LabelFactory.Component(nameof(EntrySignalProcessor), instrument.Symbol),
            setupContainer,
            messagingAdapter)
        {
            Validate.NotNull(setupContainer, nameof(setupContainer));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(instrument, nameof(instrument));
            Validate.NotNull(tradeBook, nameof(tradeBook));

            this.instrument = instrument;
            this.tradeBook = tradeBook;
            this.orderPacketBuilder = new OrderPacketBuilder(setupContainer, instrument);
            this.quoteProvider = setupContainer.QuoteProvider;
            this.account = setupContainer.Account;
            this.riskModel = setupContainer.RiskModel;
        }

        /// <summary>
        /// Processes the given entry signal, if valid then sends a trade request to the risk service.
        /// </summary>
        /// <param name="signal">The entry signal.</param>
        /// <exception cref="ValidationException">Throws if the signal is null.</exception>
        public void Process(EntrySignal signal)
        {
            Validate.NotNull(signal, nameof(signal));

            var tradeType = signal.TradeProfile.TradeType;

            if (this.IsTradeableSignal(signal))
            {
                Console.WriteLine("is tradable = true");

                var exchangeRate = this.quoteProvider.GetExchangeRate(
                    this.instrument.QuoteCurrency,
                    this.account.Currency);

                if (exchangeRate.HasNoValue)
                {
                    return;
                }

                var orderPacket = this.orderPacketBuilder.Create(
                    signal,
                    this.account.CashBalance,
                    this.riskModel.GetRiskPerTrade(tradeType),
                    this.riskModel.GetHardLimitQuantity(this.instrument.Symbol),
                    exchangeRate.Value.Value);

                if (orderPacket.HasNoValue)
                {
                    return;
                }

                var tradeApproved = new RequestTradeApproval(
                    orderPacket.Value,
                    signal,
                    this.NewGuid(),
                    this.TimeNow());

                this.MessagingAdapter.Send<CommandMessage>(
                    BlackBoxService.Risk,
                    tradeApproved,
                    this.Service);
            }
        }

        private bool IsTradeableSignal(EntrySignal signal)
        {
            Debug.NotNull(signal, nameof(signal));

            var tradeType = signal.TradeType;
            var trades = this.tradeBook.GetTradesByTradeType(tradeType);
            var tradesCount = trades.Count;
            var maxTradesByType = this.riskModel.GetMaxTrades(tradeType);

            if (tradesCount > 0)
            {
                if (tradesCount >= maxTradesByType)
                {
                    this.Log(
                        LogLevel.Debug,
                        $"Entry signal {signal.OrderSide}-{tradeType} ignored... "
                      + $"MaxTrades[{tradeType}]={maxTradesByType}, "
                      + $"TradeCount[{tradeType}]={trades.Count}");

                    return false;
                }

                var currentOrderSide = trades[0].TradeUnits[0].Entry.OrderSide;

                if (currentOrderSide != signal.OrderSide)
                {
                    this.Log(
                        LogLevel.Debug,
                        $"Entry signal {signal.OrderSide}-{tradeType} ignored... "
                      + $"OrderSide for signal does not match currently traded OrderSide="
                      + $"{currentOrderSide}");

                    return false;
                }
            }

            return true;
        }
    }
}
