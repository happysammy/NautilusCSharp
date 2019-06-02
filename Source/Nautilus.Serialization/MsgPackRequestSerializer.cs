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
                { nameof(Request.Type), nameof(Request) },
                { nameof(Request), request.Type.Name },
                { nameof(Request.Id), request.Id.ToString() },
                { nameof(Request.Timestamp), request.Timestamp.ToIsoString() },
            };

            switch (request)
            {
                case TickDataRequest req:
                    package.Add(nameof(req.Symbol), req.Symbol.ToString());
                    package.Add(nameof(req.FromDateTime), req.FromDateTime.ToIsoString());
                    package.Add(nameof(req.ToDateTime), req.ToDateTime.ToIsoString());
                    break;
                case BarDataRequest req:
                    package.Add(nameof(req.Symbol), req.Symbol.ToString());
                    package.Add(nameof(req.BarSpecification), req.BarSpecification.ToString());
                    package.Add(nameof(req.FromDateTime), req.FromDateTime.ToIsoString());
                    package.Add(nameof(req.ToDateTime), req.ToDateTime.ToIsoString());
                    break;
                case InstrumentRequest req:
                    package.Add(nameof(req.Symbol), req.Symbol.ToString());
                    break;
                case InstrumentsRequest req:
                    package.Add(nameof(req.Venue), req.Venue.ToString());
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

            var identifier = new Guid(unpacked[nameof(Request.Id)].ToString());
            var timestamp = unpacked[nameof(Request.Timestamp)].ToString().ToZonedDateTimeFromIso();
            var request = unpacked[nameof(Request)].ToString();

            switch (request)
            {
                case nameof(TickDataRequest):
                    return new TickDataRequest(
                        ObjectExtractor.Symbol(unpacked[nameof(TickDataRequest.Symbol)]),
                        ObjectExtractor.ZonedDateTime(unpacked[nameof(TickDataRequest.FromDateTime)]),
                        ObjectExtractor.ZonedDateTime(unpacked[nameof(TickDataRequest.ToDateTime)]),
                        identifier,
                        timestamp);
                case nameof(BarDataRequest):
                    return new BarDataRequest(
                        ObjectExtractor.Symbol(unpacked[nameof(BarDataRequest.Symbol)]),
                        ObjectExtractor.BarSpecification(unpacked[nameof(BarDataRequest.BarSpecification)]),
                        ObjectExtractor.ZonedDateTime(unpacked[nameof(BarDataRequest.FromDateTime)]),
                        ObjectExtractor.ZonedDateTime(unpacked[nameof(BarDataRequest.ToDateTime)]),
                        identifier,
                        timestamp);
                case nameof(InstrumentRequest):
                    return new InstrumentRequest(
                        ObjectExtractor.Symbol(unpacked[nameof(InstrumentRequest.Symbol)]),
                        identifier,
                        timestamp);
                case nameof(InstrumentsRequest):
                    return new InstrumentsRequest(
                        ObjectExtractor.Enum<Venue>(unpacked[nameof(InstrumentsRequest.Venue)]),
                        identifier,
                        timestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(request, nameof(request));
            }
        }
    }
}
