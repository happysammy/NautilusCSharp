// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackRequestSerializer.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using MessagePack;
using Nautilus.Common.Interfaces;
using Nautilus.Core.Correctness;
using Nautilus.Core.Message;
using Nautilus.Data.Messages.Requests;
using Nautilus.Network.Messages;
using Nautilus.Serialization.MessageSerializers.Internal;

namespace Nautilus.Serialization.MessageSerializers
{
    /// <summary>
    /// Provides a <see cref="Request"/> message binary serializer for the MessagePack specification.
    /// </summary>
    public sealed class MsgPackRequestSerializer : IMessageSerializer<Request>
    {
        private readonly MsgPackDictionarySerializer querySerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsgPackRequestSerializer"/> class.
        /// </summary>
        public MsgPackRequestSerializer()
        {
            this.querySerializer = new MsgPackDictionarySerializer();
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
                    package.Add(nameof(req.Authentication), ObjectSerializer.Serialize(req.Authentication));
                    break;
                case Disconnect req:
                    package.Add(nameof(req.ClientId), ObjectSerializer.Serialize(req.ClientId));
                    package.Add(nameof(req.SessionId), ObjectSerializer.Serialize(req.SessionId));
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
                        ObjectDeserializer.AsClientId(unpacked),
                        ObjectDeserializer.AsString(unpacked[nameof(Connect.Authentication)]),
                        id,
                        timestamp);
                case nameof(Disconnect):
                    return new Disconnect(
                        ObjectDeserializer.AsClientId(unpacked),
                        ObjectDeserializer.AsSessionId(unpacked),
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
