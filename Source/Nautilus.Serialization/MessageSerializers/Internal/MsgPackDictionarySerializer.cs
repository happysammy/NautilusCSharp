// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackDictionarySerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.MessageSerializers.Internal
{
    using System.Collections.Generic;
    using MessagePack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;

#pragma warning disable CS8604
    /// <summary>
    /// Provides a serializer for query objects.
    /// </summary>
    public sealed class MsgPackDictionarySerializer : ISerializer<Dictionary<string, string>>
    {
        /// <inheritdoc />
        public byte[] Serialize(Dictionary<string, string> query)
        {
            Debug.NotEmpty(query, nameof(query));

            var package = new Dictionary<string, object>();
            foreach (var (key, value) in query)
            {
                package.Add(key, value);
            }

            return MessagePackSerializer.Serialize(package);
        }

        /// <inheritdoc />
        public Dictionary<string, string> Deserialize(byte[] dataBytes)
        {
            var unpacked = MessagePackSerializer.Deserialize<Dictionary<string, object>>(dataBytes);

            var query = new Dictionary<string, string>();
            foreach (var (key, value) in unpacked)
            {
                query.Add(key, value.ToString());
            }

            return query;
        }
    }
}
