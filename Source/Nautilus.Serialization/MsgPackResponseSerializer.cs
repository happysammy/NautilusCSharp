// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackResponseSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using MsgPack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
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
            var package = new MessagePackObjectDictionary
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
                case TickDataResponse res:
                    package.Add(nameof(res.Symbol), res.Symbol.ToString());
                    package.Add(nameof(res.Ticks), MsgPackSerializer.Serialize(res.Ticks));
                    break;
                case BarDataResponse res:
                    package.Add(nameof(res.Symbol), res.Symbol.ToString());
                    package.Add(nameof(res.BarSpecification), res.BarSpecification.ToString());
                    package.Add(nameof(res.Bars), MsgPackSerializer.Serialize(res.Bars));
                    break;
                case InstrumentResponse res:
                    package.Add(nameof(res.Instruments), MsgPackSerializer.Serialize(res.Instruments));
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(response, nameof(response));
            }

            return MsgPackSerializer.Serialize(package);
        }

        /// <inheritdoc />
        public Response Deserialize(byte[] responseBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(responseBytes);

            var response = unpacked[nameof(Response.Type)].ToString();
            var correlationId = ObjectExtractor.Guid(unpacked[nameof(Response.CorrelationId)]);
            var id = ObjectExtractor.Guid(unpacked[nameof(Response.Id)]);
            var timestamp = ObjectExtractor.ZonedDateTime(unpacked[nameof(Response.Timestamp)]);

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
                case nameof(TickDataResponse):
                    return new TickDataResponse(
                        ObjectExtractor.Symbol(unpacked),
                        MsgPackSerializer.Deserialize<byte[][]>(unpacked[nameof(TickDataResponse.Ticks)].AsBinary()),
                        correlationId,
                        id,
                        timestamp);
                case nameof(BarDataResponse):
                    return new BarDataResponse(
                        ObjectExtractor.Symbol(unpacked),
                        ObjectExtractor.BarSpecification(unpacked),
                        MsgPackSerializer.Deserialize<byte[][]>(unpacked[nameof(BarDataResponse.Bars)].AsBinary()),
                        correlationId,
                        id,
                        timestamp);
                case nameof(InstrumentResponse):
                    return new InstrumentResponse(
                        MsgPackSerializer.Deserialize<byte[][]>(unpacked[nameof(InstrumentResponse.Instruments)].AsBinary()),
                        correlationId,
                        id,
                        timestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(response, nameof(response));
            }
        }
    }
}
