//--------------------------------------------------------------------------------------------------
// <copyright file="MessageServerFacade.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Common.Interfaces;
using Nautilus.Core.Types;
using Nautilus.Data.Messages.Requests;
using Nautilus.Network;
using Nautilus.Network.Encryption;
using Nautilus.Network.Nodes;
using Nautilus.Serialization.MessageSerializers;

namespace Nautilus.TestSuite.TestKit.Facades
{
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
                new Label("test-server"),
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
