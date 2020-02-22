// -------------------------------------------------------------------------------------------------
// <copyright file="MockSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Mocks
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using MessagePack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Serialization.MessageSerializers.Internal;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MockSerializer : IMessageSerializer<MockMessage>
    {
        public byte[] Serialize(MockMessage message)
        {
            var package = new Dictionary<string, byte[]>
            {
                { nameof(MockMessage.Type), ObjectSerializer.Serialize(message.Type) },
                { nameof(MockMessage.Payload), ObjectSerializer.Serialize(message.Payload) },
                { nameof(MockMessage.Id), ObjectSerializer.Serialize(message.Id) },
                { nameof(MockMessage.Timestamp), ObjectSerializer.Serialize(message.Timestamp) },
            };

            return MessagePackSerializer.Serialize(package);
        }

        public MockMessage Deserialize(byte[] dataBytes)
        {
            var unpacked = MessagePackSerializer.Deserialize<Dictionary<string, byte[]>>(dataBytes);

            return new MockMessage(
                ObjectDeserializer.AsString(unpacked[nameof(MockMessage.Payload)]),
                ObjectDeserializer.AsGuid(unpacked[nameof(MockMessage.Id)]),
                ObjectDeserializer.AsZonedDateTime(unpacked[nameof(MockMessage.Timestamp)]));
        }
    }
}
