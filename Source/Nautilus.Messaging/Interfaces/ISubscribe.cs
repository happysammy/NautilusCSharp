//--------------------------------------------------------------------------------------------------
// <copyright file="ISubscribe.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Messaging.Interfaces
{
    using System;

    /// <summary>
    /// Represents a generic subscribe message.
    /// </summary>
    public interface ISubscribe
    {
        /// <summary>
        /// Gets the subscriptions name.
        /// </summary>
        string SubscriptionName { get; }

        /// <summary>
        /// Gets the subscriptions type.
        /// </summary>
        Type SubscriptionType { get; }

        /// <summary>
        /// Gets the subscriptions subscriber endpoint.
        /// </summary>
        IEndpoint Subscriber { get; }
    }
}
