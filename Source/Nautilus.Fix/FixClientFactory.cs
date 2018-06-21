//--------------------------------------------------------------------------------------------------
// <copyright file="FixClientFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using Nautilus.Core.Validation;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Fix.Interfaces;

    /// <summary>
    /// Provides a factory for creating FIX clients.
    /// </summary>
    public class FixClientFactory : IBrokerageClientFactory
    {
        private readonly Broker broker;
        private readonly IFixMessageHandler fixMessageHandler;
        private readonly IFixMessageRouter fixMessageRouter;
        private readonly FixCredentials credentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixClientFactory"/> class.
        /// </summary>
        /// <param name="broker">The FIX client broker.</param>
        /// <param name="fixMessageHandler">The FIX message handler.</param>
        /// <param name="fixMessageRouter">The FIX message router.</param>
        /// <param name="credentials">The FIX credentials.</param>
        public FixClientFactory(
            Broker broker,
            IFixMessageHandler fixMessageHandler,
            IFixMessageRouter fixMessageRouter,
            FixCredentials credentials)
        {
            Validate.NotNull(credentials, nameof(credentials));

            this.broker = broker;
            this.fixMessageHandler = fixMessageHandler;
            this.fixMessageRouter = fixMessageRouter;
            this.credentials = credentials;
        }

        /// <summary>
        /// Creates a new <see cref="IBrokerageClient"/>.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adatper.</param>
        /// <param name="tickDataProcessor">The tick data processor.</param>
        /// <returns></returns>
        public IBrokerageClient Create(
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
                this.fixMessageHandler,
                this.fixMessageRouter,
                this.credentials,
                this.broker);
        }
    }
}
