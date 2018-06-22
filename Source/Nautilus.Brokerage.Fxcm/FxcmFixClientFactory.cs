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
        /// <param name="tickDataProcessor">The tick data processor.</param>
        /// <returns></returns>
        public IDataClient DataClient(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            ITickDataProcessor tickDataProcessor)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(tickDataProcessor, nameof(tickDataProcessor));

            return new FixClient(
                container,
                tickDataProcessor,
                new FxcmFixMessageHandler(tickDataProcessor),
                new FxcmFixMessageRouter(),
                this.credentials,
                Broker.FXCM);
        }

        /// <summary>
        /// Creates a new <see cref="ITradingClient"/>.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adatper.</param>
        /// <param name="tickDataProcessor">The tick data processor.</param>
        /// <returns></returns>
        public ITradingClient TradingClient(
            IComponentryContainer container,
            IMessagingAdapter messagingAdapter,
            ITickDataProcessor tickDataProcessor)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));
            Validate.NotNull(tickDataProcessor, nameof(tickDataProcessor));

            return new FixClient(
                container,
                tickDataProcessor,
                new FxcmFixMessageHandler(tickDataProcessor),
                new FxcmFixMessageRouter(),
                this.credentials,
                Broker.FXCM);
        }

        public IReadOnlyDictionary<string, int> GetTickValueIndex() =>
            FxcmTickSizeProvider.GetIndex();
    }
}
