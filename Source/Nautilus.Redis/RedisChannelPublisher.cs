// -------------------------------------------------------------------------------------------------
// <copyright file="RedisChannelPublisher.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis
{
    using System.Collections.Generic;
    using Nautilus.Core.Validation;
    using Nautilus.Database.Interfaces;
    using ServiceStack.Redis;

    /// <summary>
    /// Provides a publisher to Redis database channels.
    /// </summary>
    public sealed class RedisChannelPublisher : IChannelPublisher
    {
        private readonly IRedisClientsManager clientsManager;
        private readonly RedisPubSubServer server;
        private readonly List<string> channelCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisChannelPublisher"/> class.
        /// </summary>
        /// <param name="clientsManager">The clients manager.</param>
        public RedisChannelPublisher(IRedisClientsManager clientsManager)
        {
            Validate.NotNull(clientsManager, nameof(clientsManager));

            this.clientsManager = clientsManager;
            this.channelCache = new List<string>();
            this.server = new RedisPubSubServer(clientsManager);
            this.server.Start();
        }

        /// <summary>
        /// Publishes the given message to the given channel.
        /// </summary>
        /// <param name="channel">The channel name.</param>
        /// <param name="message">The message content.</param>
        public void Publish(string channel, string message)
        {
            Debug.NotNull(channel, nameof(channel));
            Debug.NotNull(message, nameof(message));

            if (!this.channelCache.Contains(channel))
            {
                this.channelCache.Add(channel);
                this.server.Channels = this.channelCache.ToArray();
            }

            using (var client = this.clientsManager.GetClient())
            {
                client.PublishMessage(channel, message);
            }
        }
    }
}
