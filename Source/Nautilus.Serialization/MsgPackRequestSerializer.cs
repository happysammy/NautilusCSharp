// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackRequestSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System.Collections.Generic;
    using MsgPack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
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
        public Request Deserialize(byte[] requestBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(requestBytes);

            var request = unpacked[nameof(Request.Type)].ToString();
            var id = ObjectExtractor.Guid(unpacked[nameof(Request.Id)]);
            var timestamp = ObjectExtractor.ZonedDateTime(unpacked[nameof(Request.Timestamp)]);

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
