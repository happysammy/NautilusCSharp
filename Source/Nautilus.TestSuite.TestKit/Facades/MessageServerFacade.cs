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
    using Nautilus.Network.Nodes;
    using Nautilus.Serialization.MessageSerializers;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MessageServerFacade : MessageServer
    {
        public MessageServerFacade(
            IComponentryContainer container,
            IMessageBusAdapter messagingAdapter,
            ICompressor compressor,
            EncryptionSettings encryption,
            ZmqNetworkAddress requestAddress,
            ZmqNetworkAddress responseAddress)
            : base(
                container,
                messagingAdapter,
                new MsgPackDictionarySerializer(),
                new MsgPackRequestSerializer(),
                new MsgPackResponseSerializer(),
                compressor,
                encryption,
                requestAddress,
                responseAddress)
        {
            this.ReceivedMessages = new List<DataRequest>();
            this.ReceivedStrings = new List<string>();

            this.RegisterHandler<DataRequest>(this.OnMessage);
        }

        public List<DataRequest> ReceivedMessages { get; }

        public List<string> ReceivedStrings { get; }

        private void OnMessage(DataRequest message)
        {
            this.ReceivedMessages.Add(message);

            this.SendReceived(message);
        }

        private void OnMessage(string message)
        {
            this.ReceivedStrings.Add(message);
        }
    }
}
