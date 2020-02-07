// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackRequestSerializer.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Message;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.Serialization.Internal;

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
            var package = new MessagePackObjectDictionary
            {
                { nameof(Request.Type), request.Type.Name },
                { nameof(Request.Id), request.Id.ToString() },
                { nameof(Request.Timestamp), request.Timestamp.ToIsoString() },
            };

            switch (request)
            {
                case DataRequest req:
                    package.Add(nameof(req.Query), this.querySerializer.Serialize(req.Query));
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(request, nameof(request));
            }

            return MsgPackSerializer.Serialize(package);
        }

        /// <inheritdoc />
        public Request Deserialize(byte[] dataBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(dataBytes);

            var request = unpacked[nameof(Request.Type)].AsString();
            var id = ObjectExtractor.AsGuid(unpacked[nameof(Request.Id)]);
            var timestamp = ObjectExtractor.AsZonedDateTime(unpacked[nameof(Request.Timestamp)]);

            switch (request)
            {
                case nameof(DataRequest):
                    return new DataRequest(
                        this.querySerializer.Deserialize(unpacked[nameof(DataRequest.Query)].AsBinary()),
                        id,
                        timestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(request, nameof(request));
            }
        }
    }
}
