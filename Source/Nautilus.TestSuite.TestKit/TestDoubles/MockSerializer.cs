// -------------------------------------------------------------------------------------------------
// <copyright file="MockSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using MsgPack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Extensions;
    using Nautilus.Serialization.Internal;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class MockSerializer : IMessageSerializer<MockMessage>
    {
        public byte[] Serialize(MockMessage message)
        {
            var package = new MessagePackObjectDictionary
            {
                { nameof(MockMessage.Type), message.Type.Name },
                { nameof(MockMessage.Payload), message.Payload },
                { nameof(MockMessage.Id), message.Id.ToString() },
                { nameof(MockMessage.Timestamp), message.Timestamp.ToIsoString() },
            };

            return MsgPackSerializer.Serialize(package);
        }

        public MockMessage Deserialize(byte[] dataBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(dataBytes);

            return new MockMessage(
                unpacked[nameof(MockMessage.Payload)].ToString(),
                ObjectExtractor.AsGuid(unpacked[nameof(MockMessage.Id)]),
                unpacked[nameof(MockMessage.Timestamp)].ToString().ToZonedDateTimeFromIso());
        }
    }
}
