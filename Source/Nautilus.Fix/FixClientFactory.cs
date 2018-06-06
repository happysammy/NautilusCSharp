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
    using Nautilus.BlackBox.Core.Build;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel.Enums;

    /// <summary>
    /// Provides a factory for creating FIX clients.
    /// </summary>
    public class FixClientFactory : IBrokerageClientFactory
    {
        private readonly Broker broker;
        private readonly FixCredentials credentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixClientFactory"/> class.
        /// </summary>
        /// <param name="broker">The FIX client broker.</param>
        /// <param name="credentials">The FIX credentials.</param>
        public FixClientFactory(
            Broker broker,
            FixCredentials credentials)
        {
            this.broker = broker;
            this.credentials = credentials;
        }

        /// <summary>
        /// Creates a new <see cref="IBrokerageClient"/>.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="messagingAdapter">The messaging adatper.</param>
        /// <returns></returns>
        public IBrokerageClient Create(
            BlackBoxContainer container,
            IMessagingAdapter messagingAdapter)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(messagingAdapter, nameof(messagingAdapter));

            return new FixClient(container, this.credentials, this.broker);
        }
    }
}
