//--------------------------------------------------------------------------------------------------
// <copyright file="MockDataPublisher.cs" company="Nautech Systems Pty Ltd">
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

using System.Diagnostics.CodeAnalysis;
using System.Text;
using Nautilus.Common.Interfaces;
using Nautilus.Network;
using Nautilus.Network.Compression;
using Nautilus.Network.Encryption;
using Nautilus.Network.Nodes;

namespace Nautilus.TestSuite.TestKit.Mocks
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MockDataPublisher : PublisherDataBus
    {
        public MockDataPublisher(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter,
            EncryptionSettings encryption,
            ZmqNetworkAddress networkAddress)
            : base(
                container,
                dataBusAdapter,
                new BypassCompressor(),
                encryption,
                networkAddress)
        {
            this.RegisterHandler<(string, string)>(this.OnMessage);
        }

        private void OnMessage((string Topic, string Message) toPublish)
        {
            var (topic, message) = toPublish;
            this.Publish(topic, MockSerializer.Serialize(message));
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class MockSerializer
        {
            public static byte[] Serialize(string dataObject)
            {
                return Encoding.UTF8.GetBytes(dataObject);
            }
        }
    }
}
