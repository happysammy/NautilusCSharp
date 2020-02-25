// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackRequestSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.MessageSerializers
{
    using System.Collections.Generic;
    using MessagePack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Network.Messages;
    using Nautilus.Serialization.MessageSerializers.Internal;

    /// <summary>
    /// Provides a <see cref="Request"/> message binary serializer for the MessagePack specification.
    /// </summary>
    public sealed class MsgPackRequestSerializer : IMessageSerializer<Request>
    {
        private readonly ISerializer<Dictionary<string, string>> querySerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsgPackRequestSerializer"/> class.
        /// </summary>
        /// <param name="querySerializer">The query serializer.</param>
        public MsgPackRequestSerializer(ISerializer<Dictionary<string, string>> querySerializer)
        {
            this.querySerializer = querySerializer;
        }

        /// <inheritdoc />
        public byte[] Serialize(Request request)
        {
            var package = new Dictionary<string, byte[]>
            {
                { nameof(Request.Type), ObjectSerializer.Serialize(request.Type) },
                { nameof(Request.Id), ObjectSerializer.Serialize(request.Id) },
                { nameof(Request.Timestamp), ObjectSerializer.Serialize(request.Timestamp) },
            };

            switch (request)
            {
                case Connect req:
                    package.Add(nameof(req.ClientId), ObjectSerializer.Serialize(req.ClientId));
                    break;
                case Disconnect req:
                    package.Add(nameof(req.ClientId), ObjectSerializer.Serialize(req.ClientId));
                    break;
                case DataRequest req:
                    package.Add(nameof(req.Query), this.querySerializer.Serialize(req.Query));
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(request, nameof(request));
            }

            return MessagePackSerializer.Serialize(package);
        }

        /// <inheritdoc />
        public Request Deserialize(byte[] dataBytes)
        {
            var unpacked = MessagePackSerializer.Deserialize<Dictionary<string, byte[]>>(dataBytes);

            var request = ObjectDeserializer.AsString(unpacked[nameof(Request.Type)]);
            var id = ObjectDeserializer.AsGuid(unpacked[nameof(Request.Id)]);
            var timestamp = ObjectDeserializer.AsZonedDateTime(unpacked[nameof(Request.Timestamp)]);

            switch (request)
            {
                case nameof(Connect):
                    return new Connect(
                        ObjectDeserializer.AsString(unpacked[nameof(Connect.ClientId)]),
                        id,
                        timestamp);
                case nameof(Disconnect):
                    return new Disconnect(
                        ObjectDeserializer.AsString(unpacked[nameof(Disconnect.ClientId)]),
                        id,
                        timestamp);
                case nameof(DataRequest):
                    return new DataRequest(
                        this.querySerializer.Deserialize(unpacked[nameof(DataRequest.Query)]),
                        id,
                        timestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(request, nameof(request));
            }
        }
    }
}
