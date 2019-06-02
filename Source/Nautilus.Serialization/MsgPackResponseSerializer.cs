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
    using Nautilus.Common.Messages.Responses;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Messages.Responses;
    using Nautilus.Serialization.Internal;

    /// <summary>
    /// Provides a <see cref="Request"/> message binary serializer for the MessagePack specification.
    /// </summary>
    public sealed class MsgPackResponseSerializer : IResponseSerializer
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
                case BadResponse res:
                    package.Add(nameof(res.Message), res.Message);
                    break;
                case BarDataResponse res:
                    package.Add(nameof(res.Symbol), res.Symbol.ToString());
                    package.Add(nameof(res.BarSpecification), res.BarSpecification.ToString());
                    package.Add(nameof(res.BarsCount), res.BarsCount);
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
                case nameof(BadResponse):
                    return new BadResponse(
                        unpacked[nameof(BadResponse.Message)].ToString(),
                        correlationId,
                        id,
                        timestamp);
                case nameof(BarDataResponse):
                    return new BarDataResponse(
                        ObjectExtractor.Symbol(unpacked[nameof(BarDataResponse.Symbol)]),
                        ObjectExtractor.BarSpecification(unpacked[nameof(BarDataResponse.BarSpecification)]),
                        unpacked[nameof(BarDataResponse.BarsCount)].AsInt32(),
                        correlationId,
                        id,
                        timestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(response, nameof(response));
            }
        }
    }
}
