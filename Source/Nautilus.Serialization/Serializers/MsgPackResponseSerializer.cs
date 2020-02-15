// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackResponseSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.Serializers
{
    using System.Collections.Generic;
    using MessagePack;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Message;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.Network.Messages;
    using Nautilus.Serialization.Internal;

    /// <summary>
    /// Provides a <see cref="Response"/> message binary serializer for the MessagePack specification.
    /// </summary>
    public sealed class MsgPackResponseSerializer : IMessageSerializer<Response>
    {
        /// <inheritdoc />
        public byte[] Serialize(Response response)
        {
            var package = new Dictionary<string, byte[]>
            {
                { nameof(Response.Type), ObjectSerializer.Serialize(response.Type) },
                { nameof(Response.CorrelationId), ObjectSerializer.Serialize(response.CorrelationId) },
                { nameof(Response.Id), ObjectSerializer.Serialize(response.Id) },
                { nameof(Response.Timestamp), ObjectSerializer.Serialize(response.Timestamp) },
            };

            switch (response)
            {
                case MessageReceived res:
                    package.Add(nameof(res.ReceivedType), ObjectSerializer.Serialize(res.ReceivedType));
                    break;
                case MessageRejected res:
                    package.Add(nameof(res.Message), ObjectSerializer.Serialize(res.Message));
                    break;
                case QueryFailure res:
                    package.Add(nameof(res.Message), ObjectSerializer.Serialize(res.Message));
                    break;
                case DataResponse res:
                    package.Add(nameof(res.Data), res.Data);
                    package.Add(nameof(res.DataType), ObjectSerializer.Serialize(res.DataType));
                    package.Add(nameof(res.DataEncoding), ObjectSerializer.Serialize(res.DataEncoding.ToString().ToUpper()));
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(response, nameof(response));
            }

            return MessagePackSerializer.Serialize(package);
        }

        /// <inheritdoc />
        public Response Deserialize(byte[] dataBytes)
        {
            var unpacked = MessagePackSerializer.Deserialize<Dictionary<string, byte[]>>(dataBytes);

            var response = ObjectDeserializer.AsString(unpacked[nameof(Response.Type)]);
            var correlationId = ObjectDeserializer.AsGuid(unpacked[nameof(Response.CorrelationId)]);
            var id = ObjectDeserializer.AsGuid(unpacked[nameof(Response.Id)]);
            var timestamp = ObjectDeserializer.AsZonedDateTime(unpacked[nameof(Response.Timestamp)]);

            switch (response)
            {
                case nameof(MessageReceived):
                    return new MessageReceived(
                        ObjectDeserializer.AsString(unpacked[nameof(MessageReceived.ReceivedType)]),
                        correlationId,
                        id,
                        timestamp);
                case nameof(MessageRejected):
                    return new MessageRejected(
                        ObjectDeserializer.AsString(unpacked[nameof(MessageRejected.Message)]),
                        correlationId,
                        id,
                        timestamp);
                case nameof(QueryFailure):
                    return new QueryFailure(
                        ObjectDeserializer.AsString(unpacked[nameof(MessageRejected.Message)]),
                        correlationId,
                        id,
                        timestamp);
                case nameof(DataResponse):
                    return new DataResponse(
                        unpacked[nameof(DataResponse.Data)],
                        ObjectDeserializer.AsString(unpacked[nameof(DataResponse.DataType)]),
                        ObjectDeserializer.AsString(unpacked[nameof(DataResponse.DataEncoding)]).ToEnum<DataEncoding>(),
                        correlationId,
                        id,
                        timestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(response, nameof(response));
            }
        }
    }
}
