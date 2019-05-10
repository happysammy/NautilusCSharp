//--------------------------------------------------------------------------------------------------
// <copyright file="MockConsumer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Messaging;
    using Nautilus.Network;

    /// <summary>
    /// Provides a mock consumer for testing.
    /// </summary>
    public class MockConsumer : Consumer
    {
        private readonly IEndpoint receiver;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockConsumer"/> class.
        /// </summary>
        /// <param name="receiver">The test receiver.</param>
        /// <param name="container">The container.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="id">The identifier.</param>
        public MockConsumer(
            IEndpoint receiver,
            IComponentryContainer container,
            NetworkAddress host,
            Port port,
            Guid id)
            : base(
                container,
                host,
                port,
                id)
        {
            this.receiver = receiver;

            this.RegisterHandler<byte[]>(this.OnMessage);
        }

        private void OnMessage(byte[] message)
        {
            this.Log.Debug($"Received {message}.");

            this.receiver.Send(message);
        }
    }
}
