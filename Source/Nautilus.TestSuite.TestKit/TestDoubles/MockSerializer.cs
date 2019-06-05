// -------------------------------------------------------------------------------------------------
// <copyright file="MockSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using MsgPack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Extensions;
    using Nautilus.Serialization.Internal;

    /// <summary>
    /// Provides a <see cref="Command"/> message binary serializer for the MessagePack specification.
    /// </summary>
    public sealed class MockSerializer : IMessageSerializer<MockMessage>
    {
        /// <inheritdoc />
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

        /// <inheritdoc />
        public MockMessage Deserialize(byte[] commandBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(commandBytes);

            return new MockMessage(
                unpacked[nameof(MockMessage.Payload)].ToString(),
                ObjectExtractor.Guid(unpacked[nameof(MockMessage.Id)]),
                unpacked[nameof(MockMessage.Timestamp)].ToString().ToZonedDateTimeFromIso());
        }
    }
}
