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
    /// Provides serialization of <see cref="Order"/> objects to MessagePack specification.
    /// </summary>
    internal static class OrderSerializer
    {
        private static readonly byte[] Empty = MsgPackSerializer.Serialize(new MessagePackObjectDictionary());

        /// <summary>
        /// Returns the given <see cref="Order"/> serialized to a MessagePack specification byte array.
        /// </summary>
        /// <param name="order">The order to serialize.</param>
        /// <returns>The serialized byte array.</returns>
        internal static byte[] Serialize(Order order)
        {
            return MsgPackSerializer.Serialize(new MessagePackObjectDictionary
            {
                { Key.Id, order.Id.ToString() },
                { Key.Symbol, order.Symbol.ToString() },
                { Key.Label, order.Label.ToString() },
                { Key.OrderSide, order.Side.ToString() },
                { Key.OrderType, order.Type.ToString() },
                { Key.Quantity, order.Quantity.Value },
                { Key.Price, ObjectPacker.Pack(order.Price) },
                { Key.TimeInForce, order.TimeInForce.ToString() },
                { Key.ExpireTime, ObjectPacker.Pack(order.ExpireTime) },
                { Key.Timestamp, order.Timestamp.ToIsoString() },
                { Key.InitId, order.InitId.ToString() },
            });
        }

        /// <summary>
        /// Returns the given <see cref="Order"/>? serialized to a MessagePack specification byte array.
        /// </summary>
        /// <param name="order">The nullable order to serialize.</param>
        /// <returns>The serialized byte array.</returns>
        internal static byte[] SerializeNullable(Order? order)
        {
            return order == null ? Empty : Serialize(order);
        }

        /// <summary>
        /// Returns the given byte array deserialized to an <see cref="Order"/>.
        /// </summary>
        /// <param name="orderBytes">The order bytes.</param>
        /// <returns>The deserialized <see cref="Order"/>.</returns>
        internal static Order Deserialize(byte[] orderBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(orderBytes);

            return Deserialize(unpacked);
        }

        /// <summary>
        /// Returns the given byte array deserialized to an <see cref="Order"/>?.
        /// </summary>
        /// <param name="orderBytes">The message pack object dictionary.</param>
        /// <returns>The deserialized <see cref="Order"/>?.</returns>
        internal static Order? DeserializeNullable(byte[] orderBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(orderBytes);

            return unpacked.Count == 0 ? null : Deserialize(unpacked);
        }

        /// <summary>
        /// Returns the given <see cref="MessagePackObjectDictionary"/> deserialized to an <see cref="Order"/>.
        /// </summary>
        /// <param name="unpacked">The unpacked order object dictionary.</param>
        /// <returns>The deserialized <see cref="Order"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">If the order type is unknown.</exception>
        private static Order Deserialize(MessagePackObjectDictionary unpacked)
        {
            var type = ObjectExtractor.Enum<OrderType>(unpacked[Key.OrderType]);
            var id = ObjectExtractor.OrderId(unpacked[Key.Id]);
            var symbol = ObjectExtractor.Symbol(unpacked[Key.Symbol]);
            var label = ObjectExtractor.Label(unpacked[Key.Label]);
            var side = ObjectExtractor.Enum<OrderSide>(unpacked[Key.OrderSide]);
            var quantity = ObjectExtractor.Quantity(unpacked[Key.Quantity]);
            var timestamp = ObjectExtractor.ZonedDateTime(unpacked[Key.Timestamp]);
            var initId = ObjectExtractor.Guid(unpacked[Key.InitId]);

            switch (type)
            {
                case OrderType.MARKET:
                    return OrderFactory.Market(
                        id,
                        symbol,
                        label,
                        side,
                        quantity,
                        timestamp,
                        initId);
                case OrderType.LIMIT:
                    return OrderFactory.Limit(
                        id,
                        symbol,
                        label,
                        side,
                        quantity,
                        ObjectExtractor.Price(unpacked[Key.Price]),
                        ObjectExtractor.Enum<TimeInForce>(unpacked[Key.TimeInForce]),
                        ObjectExtractor.NullableZonedDateTime(unpacked[Key.ExpireTime]),
                        timestamp,
                        initId);
                case OrderType.STOP_LIMIT:
                    return OrderFactory.StopLimit(
                        id,
                        symbol,
                        label,
                        side,
                        quantity,
                        ObjectExtractor.Price(unpacked[Key.Price]),
                        ObjectExtractor.Enum<TimeInForce>(unpacked[Key.TimeInForce]),
                        ObjectExtractor.NullableZonedDateTime(unpacked[Key.ExpireTime]),
                        timestamp,
                        initId);
                case OrderType.STOP_MARKET:
                    return OrderFactory.StopMarket(
                        id,
                        symbol,
                        label,
                        side,
                        quantity,
                        ObjectExtractor.Price(unpacked[Key.Price]),
                        ObjectExtractor.Enum<TimeInForce>(unpacked[Key.TimeInForce]),
                        ObjectExtractor.NullableZonedDateTime(unpacked[Key.ExpireTime]),
                        timestamp,
                        initId);
                case OrderType.MIT:
                    return OrderFactory.MarketIfTouched(
                        id,
                        symbol,
                        label,
                        side,
                        quantity,
                        ObjectExtractor.Price(unpacked[Key.Price]),
                        ObjectExtractor.Enum<TimeInForce>(unpacked[Key.TimeInForce]),
                        ObjectExtractor.NullableZonedDateTime(unpacked[Key.ExpireTime]),
                        timestamp,
                        initId);
                case OrderType.UNKNOWN:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(type, nameof(type));
            }
        }
    }
}
