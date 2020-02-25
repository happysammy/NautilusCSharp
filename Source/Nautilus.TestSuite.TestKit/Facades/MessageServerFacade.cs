//--------------------------------------------------------------------------------------------------
// <copyright file="MessageServerFacade.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Facades
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Interfaces;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Network;
    using Nautilus.Network.Encryption;
    using Nautilus.Serialization.MessageSerializers;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MessageServerFacade : MessageServer
    {
        public MessageServerFacade(
            IComponentryContainer container,
            ICompressor compressor,
            EncryptionSettings encryption,
            ZmqNetworkAddress address)
            : base(
                container,
                new MsgPackRequestSerializer(new MsgPackQuerySerializer()),
                new MsgPackResponseSerializer(),
                compressor,
                encryption,
                address)
        {
            this.ReceivedMessages = new List<DataRequest>();

            this.RegisterHandler<DataRequest>(this.OnMessage);
        }

        public List<DataRequest> ReceivedMessages { get; }

        private void OnMessage(DataRequest message)
        {
            this.ReceivedMessages.Add(message);

            this.SendReceived(message);
        }
    }
}
