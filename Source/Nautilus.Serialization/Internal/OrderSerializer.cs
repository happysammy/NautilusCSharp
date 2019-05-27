// -------------------------------------------------------------------------------------------------
// <copyright file="OrderSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.Internal
{
    using System.ComponentModel;
    using MsgPack;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// Provides serialization of <see cref="Order"/> objects to Message Pack specification.
    /// </summary>
    internal static class OrderSerializer
    {
        private static readonly byte[] Empty = MsgPackSerializer.Serialize(new MessagePackObjectDictionary());

        /// <summary>
        /// Serialize the given order to Message Pack specification bytes.
        /// </summary>
        /// <param name="order">The order to serialize.</param>
        /// <returns>The serialized order.</returns>
        internal static byte[] Serialize(Order order)
        {
            return MsgPackSerializer.Serialize(new MessagePackObjectDictionary
            {
                { Key.Symbol, order.Symbol.ToString() },
                { Key.OrderId, order.Id.ToString() },
                { Key.Label, order.Label.ToString() },
                { Key.OrderSide, order.Side.ToString() },
                { Key.OrderType, order.Type.ToString() },
                { Key.Quantity, order.Quantity.Value },
                { Key.Price, ObjectPacker.NullablePrice(order.Price) },
                { Key.TimeInForce, order.TimeInForce.ToString() },
                { Key.ExpireTime, ObjectPacker.NullableZonedDateTime(order.ExpireTime) },
                { Key.Timestamp, order.Timestamp.ToIsoString() },
                { Key.InitEventGuid, order.InitEventId.ToString() },
            });
        }

        /// <summary>
        /// Serialize the given nullable order to Message Pack specification bytes.
        /// </summary>
        /// <param name="order">The nullable order to serialize.</param>
        /// <returns>The serialized order.</returns>
        internal static byte[] SerializeNullable(Order? order)
        {
            return order == null ? Empty : Serialize(order);
        }

        /// <summary>
        /// Deserialize the given byte array to an <see cref="Order"/>.
        /// </summary>
        /// <param name="orderBytes">The order bytes.</param>
        /// <returns>The deserialized order.</returns>
        internal static Order Deserialize(byte[] orderBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(orderBytes);

            return Deserialize(unpacked);
        }

        /// <summary>
        /// Returns the nullable take profit <see cref="Order"/>.
        /// </summary>
        /// <param name="orderBytes">The message pack object dictionary.</param>
        /// <returns>The nullable <see cref="Order"/>.</returns>
        internal static Order? DeserializeNullable(byte[] orderBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(orderBytes);

            return unpacked.Count == 0 ? null : Deserialize(unpacked);
        }

        /// <summary>
        /// Deserialize the given byte array to an <see cref="Order"/>.
        /// </summary>
        /// <param name="unpacked">The unpacked order object dictionary.</param>
        /// <returns>The deserialized order.</returns>
        /// <exception cref="InvalidEnumArgumentException">If the order type is unknown.</exception>
        private static Order Deserialize(MessagePackObjectDictionary unpacked)
        {
            var orderType = ObjectExtractor.OrderType(unpacked[Key.OrderType]);
            var symbol = ObjectExtractor.Symbol(unpacked[Key.Symbol]);
            var id = ObjectExtractor.OrderId(unpacked[Key.OrderId]);
            var label = ObjectExtractor.Label(unpacked[Key.Label]);
            var side = ObjectExtractor.OrderSide(unpacked[Key.OrderSide]);
            var quantity = ObjectExtractor.Quantity(unpacked[Key.Quantity]);
            var timestamp = ObjectExtractor.ZonedDateTime(unpacked[Key.Timestamp]);
            var initEventGuid = ObjectExtractor.Guid(unpacked[Key.InitEventGuid]);

            switch (orderType)
            {
                case OrderType.MARKET:
                    return OrderFactory.Market(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        timestamp,
                        initEventGuid);
                case OrderType.LIMIT:
                    return OrderFactory.Limit(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        ObjectExtractor.Price(unpacked[Key.Price]),
                        ObjectExtractor.TimeInForce(unpacked[Key.TimeInForce]),
                        ObjectExtractor.NullableZonedDateTime(unpacked[Key.ExpireTime]),
                        timestamp,
                        initEventGuid);
                case OrderType.STOP_LIMIT:
                    return OrderFactory.StopLimit(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        ObjectExtractor.Price(unpacked[Key.Price]),
                        ObjectExtractor.TimeInForce(unpacked[Key.TimeInForce]),
                        ObjectExtractor.NullableZonedDateTime(unpacked[Key.ExpireTime]),
                        timestamp,
                        initEventGuid);
                case OrderType.STOP_MARKET:
                    return OrderFactory.StopMarket(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        ObjectExtractor.Price(unpacked[Key.Price]),
                        ObjectExtractor.TimeInForce(unpacked[Key.TimeInForce]),
                        ObjectExtractor.NullableZonedDateTime(unpacked[Key.ExpireTime]),
                        timestamp,
                        initEventGuid);
                case OrderType.MIT:
                    return OrderFactory.MarketIfTouched(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        ObjectExtractor.Price(unpacked[Key.Price]),
                        ObjectExtractor.TimeInForce(unpacked[Key.TimeInForce]),
                        ObjectExtractor.NullableZonedDateTime(unpacked[Key.ExpireTime]),
                        timestamp,
                        initEventGuid);
                case OrderType.UNKNOWN:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(orderType, nameof(orderType));
            }
        }
    }
}
