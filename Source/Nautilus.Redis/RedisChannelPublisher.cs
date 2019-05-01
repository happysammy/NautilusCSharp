// -------------------------------------------------------------------------------------------------
// <copyright file="RedisChannelPublisher.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis
{
    using System.Collections.Generic;
    using Nautilus.Core.Correctness;
    using Nautilus.Data.Interfaces;
    using StackExchange.Redis;

    /// <summary>
    /// Provides a publisher to Redis database channels.
    /// </summary>
    public sealed class RedisChannelPublisher : IChannelPublisher
    {
        private readonly ISubscriber subscriber;
        private readonly List<string> channelCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisChannelPublisher"/> class.
        /// </summary>
        /// <param name="subscriber">The Redis subscriber.</param>
        public RedisChannelPublisher(ISubscriber subscriber)
        {
            this.subscriber = subscriber;
            this.channelCache = new List<string>();
        }

        /// <summary>
        /// Publishes the given message to the given channel.
        /// </summary>
        /// <param name="channel">The channel name.</param>
        /// <param name="message">The message content.</param>
        public void Publish(string channel, string message)
        {
            Debug.NotEmptyOrWhiteSpace(channel, nameof(channel));
            Debug.NotEmptyOrWhiteSpace(message, nameof(message));

            if (!this.channelCache.Contains(channel))
            {
                this.channelCache.Add(channel);
            }

            this.subscriber.Publish(channel, message);
        }
    }
}
