// -------------------------------------------------------------------------------------------------
// <copyright file="MessagingServiceFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Common.Messaging
{
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;

    /// <summary>
    /// Provides a factory to create the systems messaging service.
    /// </summary>
    public static class MessagingServiceFactory
    {
        /// <summary>
        /// Creates and returns a new messaging adapter.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <returns>The messaging adapter.</returns>
        public static MessagingAdapter Create(IComponentryContainer container)
        {
            return new MessagingAdapter(
                new MessageBus<Command>(container),
                new MessageBus<Event>(container),
                new MessageBus<Document>(container));
        }
    }
}
