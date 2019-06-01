// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackRequestSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System;
    using MsgPack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Data.Messages.Requests;
    using Nautilus.DomainModel.Enums;
    using Nautilus.Serialization.Internal;

    /// <summary>
    /// Provides a <see cref="Request"/> message binary serializer for the MessagePack specification.
    /// </summary>
    public sealed class MsgPackRequestSerializer : IRequestSerializer
    {
        /// <inheritdoc />
        public byte[] Serialize(Request request)
        {
            var package = new MessagePackObjectDictionary
            {
                { Key.Type, nameof(Command) },
                { Key.Request, request.Type.Name },
                { Key.Id, request.Id.ToString() },
                { Key.Timestamp, request.Timestamp.ToIsoString() },
            };

            switch (request)
            {
                case TickDataRequest req:
                    package.Add(Key.Symbol, req.Symbol.ToString());
                    package.Add(Key.FromDateTime, req.FromDateTime.ToIsoString());
                    package.Add(Key.ToDateTime, req.ToDateTime.ToIsoString());
                    break;
                case BarDataRequest req:
                    package.Add(Key.Symbol, req.Symbol.ToString());
                    package.Add(Key.BarSpecification, req.BarSpecification.ToString());
                    package.Add(Key.FromDateTime, req.FromDateTime.ToIsoString());
                    package.Add(Key.ToDateTime, req.ToDateTime.ToIsoString());
                    break;
                case InstrumentRequest req:
                    package.Add(Key.Symbol, req.Symbol.ToString());
                    break;
                case InstrumentsRequest req:
                    package.Add(Key.Venue, req.Venue.ToString());
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(request, nameof(request));
            }

            return MsgPackSerializer.Serialize(package);
        }

        /// <inheritdoc />
        public Request Deserialize(byte[] commandBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(commandBytes);

            var identifier = new Guid(unpacked[Key.Id].ToString());
            var timestamp = unpacked[Key.Timestamp].ToString().ToZonedDateTimeFromIso();
            var request = unpacked[Key.Request].ToString();

            switch (request)
            {
                case nameof(TickDataRequest):
                    return new TickDataRequest(
                        ObjectExtractor.Symbol(unpacked[Key.Symbol]),
                        ObjectExtractor.ZonedDateTime(unpacked[Key.FromDateTime]),
                        ObjectExtractor.ZonedDateTime(unpacked[Key.ToDateTime]),
                        identifier,
                        timestamp);
                case nameof(BarDataRequest):
                    return new BarDataRequest(
                        ObjectExtractor.Symbol(unpacked[Key.Symbol]),
                        ObjectExtractor.BarSpecification(unpacked[Key.BarSpecification]),
                        ObjectExtractor.ZonedDateTime(unpacked[Key.FromDateTime]),
                        ObjectExtractor.ZonedDateTime(unpacked[Key.ToDateTime]),
                        identifier,
                        timestamp);
                case nameof(InstrumentRequest):
                    return new InstrumentRequest(
                        ObjectExtractor.Symbol(unpacked[Key.Symbol]),
                        identifier,
                        timestamp);
                case nameof(InstrumentsRequest):
                    return new InstrumentsRequest(
                        ObjectExtractor.Enum<Venue>(unpacked[Key.Venue]),
                        identifier,
                        timestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(request, nameof(request));
            }
        }
    }
}
