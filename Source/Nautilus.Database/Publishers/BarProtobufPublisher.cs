//--------------------------------------------------------------------------------------------------
// <copyright file="BarProtobufPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Database.Publishers
{
    using Nautilus.Database.Interfaces;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.ProtoBuf;

    /// <summary>
    /// Providers a <see cref="ProtoBuf"/> implementation for the <see cref="Bar"/> publisher.
    /// </summary>
    public class BarProtobufPublisher : IBarPublisher
    {
        private readonly ProtoBufServiceClient publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarProtobufPublisher"/> class.
        /// </summary>
        public BarProtobufPublisher()
        {
            this.publisher = new ProtoBufServiceClient("127.0.0.1");
        }
        /// <summary>
        /// Publishes the given bar to subscribers.
        /// </summary>
        /// <param name="barEvent"></param>
        public void Publish(BarDataEvent barEvent)
        {
        }
    }
}
