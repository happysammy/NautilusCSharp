// -------------------------------------------------------------------------------------------------
// <copyright file="RedisBarPublisher.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Redis
{
    using Nautilus.Core.Validation;
    using Nautilus.Database.Interfaces;
    using Nautilus.DomainModel.Events;
    using ServiceStack.Redis;

    public sealed class RedisBarPublisher : IBarPublisher
    {
        private readonly IRedisClientsManager clientsManager;
        private readonly IRedisClient publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisBarPublisher"/> class.
        /// </summary>
        /// <param name="clientsManager">The redis clients manager.</param>
        public RedisBarPublisher(IRedisClientsManager clientsManager)
        {
            Validate.NotNull(clientsManager, nameof(clientsManager));

            this.clientsManager = clientsManager;
            this.publisher = this.clientsManager.GetClient();
        }

        public void Publish(BarDataEvent barEvent)
        {
            Debug.NotNull(barEvent, nameof(barEvent));

            this.publisher.PublishMessage(
                barEvent.BarType.ToString().ToLower(),
                barEvent.Bar.ToString());
        }
    }
}
