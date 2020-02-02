// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackQuerySerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.MessagePack
{
    using System.Collections.Generic;
    using MsgPack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.Serialization.Internal;

    /// <summary>
    /// Provides a serializer for query objects.
    /// </summary>
    public class MsgPackQuerySerializer : ISerializer<Dictionary<string, string>>
    {
        /// <inheritdoc />
        public byte[] Serialize(Dictionary<string, string> query)
        {
            Debug.NotEmpty(query, nameof(query));

            var package = new MessagePackObjectDictionary();
            foreach (var (key, value) in query)
            {
                package.Add(key, value);
            }

            return MsgPackSerializer.Serialize(package);
        }

        /// <inheritdoc />
        public Dictionary<string, string> Deserialize(byte[] dataBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(dataBytes);

            var query = new Dictionary<string, string>();
            foreach (var (key, value) in unpacked)
            {
                query.Add(key.AsString(), value.AsString());
            }

            return query;
        }
    }
}
