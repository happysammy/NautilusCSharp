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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Nautilus.Common.Enums;
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
        private readonly IDataSerializer<string> serializer;

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
            this.serializer = new MockSerializer();

            this.RegisterHandler<(string, string)>(this.OnMessage);
        }

        private void OnMessage((string Topic, string Message) toPublish)
        {
            this.Publish(toPublish.Topic, this.serializer.Serialize(toPublish.Message));
        }

        private sealed class MockSerializer : IDataSerializer<string>
        {
            public DataEncoding BlobEncoding => DataEncoding.Undefined;

            public DataEncoding ObjectEncoding => DataEncoding.Undefined;

            byte[] ISerializer<string>.Serialize(string dataObject)
            {
                return Encoding.UTF8.GetBytes(dataObject);
            }

            public byte[][] Serialize(string[] dataObjects)
            {
                throw new InvalidOperationException("Not implemented.");
            }

            public byte[] SerializeBlob(byte[][] dataObjectsArray, Dictionary<string, string> metadata)
            {
                throw new InvalidOperationException("Not implemented.");
            }

            public string[] Deserialize(byte[][] dataBytesArray, object? metadata = null)
            {
                throw new InvalidOperationException("Not implemented.");
            }

            public string[] DeserializeBlob(byte[] dataBytes)
            {
                throw new InvalidOperationException("Not implemented.");
            }

            public byte[] Serialize(string message)
            {
                return Encoding.UTF8.GetBytes(message);
            }

            public string Deserialize(byte[] dataBytes)
            {
                return Encoding.UTF8.GetString(dataBytes);
            }
        }
    }
}
