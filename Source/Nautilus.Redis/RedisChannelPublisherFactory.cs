// -------------------------------------------------------------------------------------------------
// <copyright file="RedisChannelPublisherFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis
{
    using Nautilus.Core.Validation;
    using Nautilus.Database.Interfaces;
    using ServiceStack.Redis;

    /// <summary>
    /// Provides Redis implementations of the <see cref="IChannelPublisher"/>.
    /// </summary>
    public sealed class RedisChannelPublisherFactory : IChannelPublisherFactory
    {
        private readonly IRedisClientsManager clientsManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisChannelPublisherFactory"/> class.
        /// </summary>
        /// <param name="clientsManager"></param>
        public RedisChannelPublisherFactory(IRedisClientsManager clientsManager)
        {
            Validate.NotNull(clientsManager, nameof(clientsManager));

            this.clientsManager = clientsManager;
        }

        /// <summary>
        /// Creates and returns a new instance of the <see cref="RedisChannelPublisher"/>.
        /// </summary>
        /// <returns>The channel publisher.</returns>
        public IChannelPublisher Create()
        {
            return new RedisChannelPublisher(this.clientsManager);
        }
    }
}
