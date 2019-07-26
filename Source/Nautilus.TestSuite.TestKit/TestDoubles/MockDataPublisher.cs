//--------------------------------------------------------------------------------------------------
// <copyright file="MockDataPublisher.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Network;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MockDataPublisher : DataPublisher<string>
    {
        public MockDataPublisher(
            IComponentryContainer container,
            IDataBusAdapter dataBusAdapter,
            NetworkAddress host,
            NetworkPort port)
            : base(
                container,
                dataBusAdapter,
                new MockSerializer(),
                host,
                port,
                Guid.NewGuid())
        {
            this.RegisterHandler<(string, string)>(this.OnMessage);
        }

        private void OnMessage((string Topic, string Message) toPublish)
        {
            this.Publish(toPublish.Topic, toPublish.Message);
        }

        private sealed class MockSerializer : ISerializer<string>, IDataSerializer<string>
        {
            public DataEncoding DataEncoding => DataEncoding.Unknown;

            public byte[] Serialize(string message)
            {
                return Encoding.UTF8.GetBytes(message);
            }

            public string Deserialize(byte[] bytes)
            {
                return Encoding.UTF8.GetString(bytes);
            }
        }
    }
}
