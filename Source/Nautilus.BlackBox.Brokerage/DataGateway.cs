//--------------------------------------------------------------------------------------------------
// <copyright file="BrokerageGateway.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.BlackBox.Brokerage
{
    using System.Collections.Generic;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;
    using Symbol = Nautilus.DomainModel.ValueObjects.Symbol;

    /// <summary>
    /// The <see cref="BlackBox"/> boundary for the brokerage implementation.
    /// </summary>
    public sealed class DataGateway : ComponentBusConnectedBase, IDataGateway
    {
        private readonly IInstrumentRepository instrumentRepository;
        private readonly IDataClient dataClient;
        private readonly CurrencyCode accountCurrency;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeGateway"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adapter.</param>
        /// <param name="dataClient">The data client.</param>
        /// <param name="instrumentRepository">The instrument repository.</param>
        public DataGateway(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            IDataClient dataClient,
            IInstrumentRepository instrumentRepository,
            CurrencyCode accountCurrency)
            : base(
                ServiceContext.FIX,
                new Label(nameof(DataGateway)),
                container,
                messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(dataClient, nameof(dataClient));
            Validate.NotNull(instrumentRepository, nameof(instrumentRepository));

            this.dataClient = dataClient;
            this.instrumentRepository = instrumentRepository;
            this.accountCurrency = accountCurrency;
        }

        /// <summary>
        /// Gets the brokerage gateways broker name.
        /// </summary>
        public Broker Broker => this.dataClient.Broker;

        /// <summary>
        /// Gets a value indicating whether the brokerage gateways broker client is connected.
        /// </summary>
        public bool IsConnected => this.dataClient.IsConnected;

        /// <summary>
        /// Returns the current time of the <see cref="BlackBox"/> system clock.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/>.</returns>
        public ZonedDateTime GetTimeNow()
        {
            return this.TimeNow();
        }

        /// <summary>
        /// Connects the brokerage client.
        /// </summary>
        public void Connect()
        {
            this.dataClient.Connect();
        }

        /// <summary>
        /// Disconnects the brokerage client.
        /// </summary>
        public void Disconnect()
        {
            this.dataClient.Disconnect();
        }

        /// <summary>
        /// Requests market data for the given symbol from the brokerage.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <exception cref="ValidationException">Throws if the symbol is null.</exception>
        public void RequestMarketDataSubscribe(Symbol symbol)
        {
            Validate.NotNull(symbol, nameof(symbol));

            this.dataClient.RequestMarketDataSubscribe(symbol);
        }

        /// <summary>
        /// Requests market data for all symbols from the brokerage.
        /// </summary>
        public void RequestMarketDataSubscribeAll()
        {
            this.dataClient.RequestMarketDataSubscribeAll();
        }

        /// <summary>
        /// Request an update on the instrument corresponding to the given symbol from the brokerage,
        /// and subscribe to updates.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <exception cref="ValidationException">Throws if the symbol is null.</exception>
        public void UpdateInstrumentSubscribe(Symbol symbol)
        {
            Validate.NotNull(symbol, nameof(symbol));

            this.dataClient.UpdateInstrumentSubscribe(symbol);
        }

        /// <summary>
        /// Requests an update on all instruments from the brokerage.
        /// </summary>
        public void UpdateInstrumentsSubscribeAll()
        {
            this.dataClient.UpdateInstrumentsSubscribeAll();
        }

        /// <summary>
        /// Event handler for receiving FIX Position Reports.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <exception cref="ValidationException">Throws if the argument is null.</exception>
        public void OnPositionReport(string account)
        {
            Validate.NotNull(account, nameof(account));

            this.Log.Debug($"PositionReport: ({account})");
        }

        /// <summary>
        /// Event handler for receiving FIX Collateral Inquiry Acknowledgements.
        /// </summary>
        /// <param name="inquiryId">The inquiry identifier.</param>
        /// <param name="accountNumber">The account number.</param>
        /// <exception cref="ValidationException">Throws if the either argument is null.</exception>
        public void OnCollateralInquiryAck(string inquiryId, string accountNumber)
        {
            Validate.NotNull(inquiryId, nameof(inquiryId));
            Validate.NotNull(accountNumber, nameof(accountNumber));

            this.Log.Debug($"CollateralInquiryAck: ({this.Broker}-{accountNumber}, InquiryId={inquiryId})");
        }

        /// <summary>
        /// Event handler for receiving FIX business messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnBusinessMessage(string message)
        {
            this.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));

                this.Log.Debug($"BusinessMessageReject: {message}");
            });
        }

        /// <summary>
        /// Updates the given instruments in the <see cref="IInstrumentRepository"/>.
        /// </summary>
        /// <param name="instruments">The instruments collection.</param>
        /// <param name="responseId">The response identifier.</param>
        /// <param name="result">The result.</param>
        public void OnInstrumentsUpdate(
            IReadOnlyCollection<Instrument> instruments,
            string responseId,
            string result)
        {
            this.Execute(() =>
            {
                Validate.NotNull(instruments, nameof(instruments));
                Validate.NotNull(responseId, nameof(responseId));
                Validate.NotNull(result, nameof(result));

                this.instrumentRepository.UpdateInstruments(instruments);

                this.Log.Debug($"SecurityListReceived: (SecurityResponseId={responseId}) result={result}");
            });
        }

        /// <summary>
        /// Event handler for acknowledgement of a request for positions.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <param name="positionRequestId">The position request identifier.</param>
        public void OnRequestForPositionsAck(string accountNumber, string positionRequestId)
        {
            Validate.NotNull(accountNumber, nameof(accountNumber));
            Validate.NotNull(positionRequestId, nameof(positionRequestId));

            this.Log.Debug($"RequestForPositionsAck Received ({accountNumber}-{positionRequestId})");
        }

        private static Symbol ConvertStringToSymbol(string symbolString, Exchange exchange)
        {
            Debug.NotNull(symbolString, nameof(symbolString));

            return symbolString != "NONE"
                       ? new Symbol(symbolString, exchange)
                       : new Symbol("AUDUSD", Exchange.FXCM);
        }

        private Money GetMoneyType(decimal amount)
        {
            Debug.DecimalNotOutOfRange(amount, nameof(amount), decimal.Zero, decimal.MaxValue);

            return amount > 0
                ? Money.Create(amount, this.accountCurrency)
                : Money.Zero(this.accountCurrency);
        }
    }
}
