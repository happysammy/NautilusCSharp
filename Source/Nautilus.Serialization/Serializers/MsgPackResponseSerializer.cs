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

#pragma warning disable CS8604
    /// <summary>
    /// Provides a <see cref="Response"/> message binary serializer for the MessagePack specification.
    /// </summary>
    public sealed class MsgPackResponseSerializer : IMessageSerializer<Response>
    {
        /// <inheritdoc />
        public byte[] Serialize(Response response)
        {
            var package = new Dictionary<string, object>
            {
                { nameof(Response.Type), response.Type.Name },
                { nameof(Response.CorrelationId), response.CorrelationId.ToString() },
                { nameof(Response.Id), response.Id.ToString() },
                { nameof(Response.Timestamp), response.Timestamp.ToIsoString() },
            };

            switch (response)
            {
                case MessageReceived res:
                    package.Add(nameof(res.ReceivedType), res.ReceivedType);
                    break;
                case MessageRejected res:
                    package.Add(nameof(res.Message), res.Message);
                    break;
                case QueryFailure res:
                    package.Add(nameof(res.Message), res.Message);
                    break;
                case DataResponse res:
                    package.Add(nameof(res.Data), res.Data);
                    package.Add(nameof(res.DataType), res.DataType);
                    package.Add(nameof(res.DataEncoding), res.DataEncoding.ToString().ToUpper());
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(response, nameof(response));
            }

            return MessagePackSerializer.Serialize(package);
        }

        /// <inheritdoc />
        public Response Deserialize(byte[] dataBytes)
        {
            var unpacked = MessagePackSerializer.Deserialize<Dictionary<string, object>>(dataBytes);

            var response = unpacked[nameof(Response.Type)].ToString();
            var correlationId = ObjectExtractor.AsGuid(unpacked[nameof(Response.CorrelationId)]);
            var id = ObjectExtractor.AsGuid(unpacked[nameof(Response.Id)]);
            var timestamp = ObjectExtractor.AsZonedDateTime(unpacked[nameof(Response.Timestamp)]);

            switch (response)
            {
                case nameof(MessageReceived):
                    return new MessageReceived(
                        unpacked[nameof(MessageReceived.ReceivedType)].ToString(),
                        correlationId,
                        id,
                        timestamp);
                case nameof(MessageRejected):
                    return new MessageRejected(
                        unpacked[nameof(MessageRejected.Message)].ToString(),
                        correlationId,
                        id,
                        timestamp);
                case nameof(QueryFailure):
                    return new QueryFailure(
                        unpacked[nameof(MessageRejected.Message)].ToString(),
                        correlationId,
                        id,
                        timestamp);
                case nameof(DataResponse):
                    return new DataResponse(
                        (byte[])unpacked[nameof(DataResponse.Data)],
                        unpacked[nameof(DataResponse.DataType)].ToString(),
                        unpacked[nameof(DataResponse.DataEncoding)].ToString().ToEnum<DataEncoding>(),
                        correlationId,
                        id,
                        timestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(response, nameof(response));
            }
        }
    }
}
