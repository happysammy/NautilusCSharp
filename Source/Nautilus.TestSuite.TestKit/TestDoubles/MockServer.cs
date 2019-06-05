//--------------------------------------------------------------------------------------------------
// <copyright file="MockServer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Messaging.Interfaces;
    using Nautilus.Network;
    using Nautilus.Serialization;

    /// <summary>
    /// Provides a mock server for testing.
    /// </summary>
    public sealed class MockServer : Server<MockMessage>
    {
        private readonly IEndpoint receiver;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockServer"/> class.
        /// </summary>
        /// <param name="receiver">The test receiver.</param>
        /// <param name="container">The container.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="id">The identifier.</param>
        public MockServer(
            IEndpoint receiver,
            IComponentryContainer container,
            NetworkAddress host,
            NetworkPort port,
            Guid id)
            : base(
                container,
                new MockSerializer(),
                new MsgPackResponseSerializer(),
                host,
                port,
                id)
        {
            this.receiver = receiver;

            this.RegisterHandler<ReceivedMessage<MockMessage>>(this.OnMessage);
        }

        private void OnMessage(ReceivedMessage<MockMessage> message)
        {
            this.Log.Debug($"Received {message}.");

            this.receiver.Send(message.Payload);
        }
    }
}
