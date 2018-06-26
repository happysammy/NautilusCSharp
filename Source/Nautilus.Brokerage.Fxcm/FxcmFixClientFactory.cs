//--------------------------------------------------------------------------------------------------
// <copyright file="FixClientFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.FXCM
{
    using System.Collections.Generic;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix;

    /// <summary>
    /// Provides a factory for creating FIX clients.
    /// </summary>
    public class FxcmFixClientFactory : IFixClientFactory
    {
        private readonly FixCredentials credentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="FxcmFixClientFactory"/> class.
        /// </summary>
        /// <param name="username">The FIX account username.</param>
        /// <param name="password">The FIX account password.</param>
        /// <param name="accountNumber">The FIX account number.</param>
        public FxcmFixClientFactory(
            string username,
            string password,
            string accountNumber)
        {
            Validate.NotNull(username, nameof(username));
            Validate.NotNull(password, nameof(password));
            Validate.NotNull(accountNumber, nameof(accountNumber));

            this.credentials = new FixCredentials(
                username,
                password,
                accountNumber);
        }

        /// <summary>
        /// Creates a new <see cref="IDataClient"/>.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adatper.</param>
        /// <param name="tickProcessor">The tick data processor.</param>
        /// <returns></returns>
        public IDataClient DataClient(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            ITickProcessor tickProcessor)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(tickProcessor, nameof(tickProcessor));

            return new FixClient(
                container,
                tickProcessor,
                new FxcmFixMessageHandler(container, tickProcessor),
                new FxcmFixMessageRouter(container),
                this.credentials,
                Broker.FXCM);
        }

        /// <summary>
        /// Creates a new <see cref="ITradeClient"/>.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adatper.</param>
        /// <param name="tickProcessor">The tick data processor.</param>
        /// <returns></returns>
        public ITradeClient TradeClient(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            ITickProcessor tickProcessor)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(tickProcessor, nameof(tickProcessor));

            return new FixClient(
                container,
                tickProcessor,
                new FxcmFixMessageHandler(container, tickProcessor),
                new FxcmFixMessageRouter(container),
                this.credentials,
                Broker.FXCM);
        }

        /// <summary>
        /// Returns a read-only list of all symbol <see cref="string"/>(s) provided by the FIX client.
        /// </summary>
        /// <returns>The list of symbols.</returns>
        public IReadOnlyList<string> GetAllBrokerSymbols() => FxcmSymbolProvider.GetAllBrokerSymbols();

        /// <summary>
        /// Returns a read-only list of all <see cref="Symbol"/>(s) provided by the FIX client.
        /// </summary>
        /// <returns>The list of symbols.</returns>
        public IReadOnlyList<Symbol> GetAllSymbols() => FxcmSymbolProvider.GetAllSymbols();

        /// <summary>
        /// Returns a read-only list of all tick values provided by the FIX client.
        /// </summary>
        /// <returns>The list of symbols</returns>
        public IReadOnlyDictionary<string, int> GetTickValueIndex() => FxcmTickSizeProvider.GetIndex();
    }
}
