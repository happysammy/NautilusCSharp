// -------------------------------------------------------------------------------------------------
// <copyright file="OrderSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.Internal
{
    using System;
    using MsgPack;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// Provides serialization of <see cref="Order"/> objects to Message Pack specification.
    /// </summary>
    internal static class OrderSerializer
    {
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
            });
        }

        /// <summary>
        /// Deserialize the given byte array to an <see cref="Order"/>.
        /// </summary>
        /// <param name="orderBytes">The order bytes.</param>
        /// <param name="initEventGuid">The order initialization event GUID.</param>
        /// <returns>The deserialized order.</returns>
        /// <exception cref="InvalidOperationException">If the order type is unknown.</exception>
        internal static Order Deserialize(byte[] orderBytes, Guid initEventGuid)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(orderBytes);

            var orderType = ObjectExtractor.OrderType(unpacked[Key.OrderType]);
            var symbol = ObjectExtractor.Symbol(unpacked[Key.Symbol]);
            var id = ObjectExtractor.OrderId(unpacked[Key.OrderId]);
            var label = ObjectExtractor.Label(unpacked[Key.Label]);
            var side = ObjectExtractor.OrderSide(unpacked[Key.OrderSide]);
            var quantity = ObjectExtractor.Quantity(unpacked[Key.Quantity]);
            var timestamp = ObjectExtractor.ZonedDateTime(unpacked[Key.Timestamp]);

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

        /// <summary>
        /// Returns the given order as a <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="atomicOrder">The atomic order.</param>
        /// <returns>The <see cref="MessagePackObject"/>.</returns>
        internal static byte[] SerializeEntry(AtomicOrder atomicOrder)
        {
            return Serialize(atomicOrder.Entry);
        }

        /// <summary>
        /// Returns the given order as a <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="atomicOrder">The atomic order.</param>
        /// <returns>The <see cref="MessagePackObject"/>.</returns>
        internal static byte[] SerializeStopLoss(AtomicOrder atomicOrder)
        {
            return Serialize(atomicOrder.StopLoss);
        }

        /// <summary>
        /// Returns the given nullable take profit order as a <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="atomicOrder">The atomic order.</param>
        /// <returns>The <see cref="MessagePackObject"/>.</returns>
        internal static byte[] SerializeTakeProfit(AtomicOrder atomicOrder)
        {
            return atomicOrder.HasTakeProfit
                ? Serialize(atomicOrder.TakeProfit)
                : MsgPackSerializer.Serialize(MessagePackObject.Nil);
        }

        /// <summary>
        /// Returns the nullable take profit <see cref="Order"/>.
        /// </summary>
        /// <param name="unpacked">The message pack object dictionary.</param>
        /// <returns>The nullable <see cref="Order"/>.</returns>
        internal static Order DeserializeEntry(MessagePackObjectDictionary unpacked)
        {
            return Deserialize(unpacked[Key.Entry].AsBinary(), ObjectExtractor.Guid(unpacked[Key.InitEventGuidEntry]));
        }

        /// <summary>
        /// Returns the nullable take profit <see cref="Order"/>.
        /// </summary>
        /// <param name="unpacked">The message pack object dictionary.</param>
        /// <returns>The nullable <see cref="Order"/>.</returns>
        internal static Order DeserializeStopLoss(MessagePackObjectDictionary unpacked)
        {
            return Deserialize(unpacked[Key.StopLoss].AsBinary(), ObjectExtractor.Guid(unpacked[Key.InitEventGuidStopLoss]));
        }

        /// <summary>
        /// Returns the nullable take profit <see cref="Order"/>.
        /// </summary>
        /// <param name="unpacked">The message pack object dictionary.</param>
        /// <returns>The nullable <see cref="Order"/>.</returns>
        internal static Order? DeserializeTakeProfit(MessagePackObjectDictionary unpacked)
        {
            var unpackedOrder = unpacked[Key.TakeProfit].AsBinary();
            var deserialized = MsgPackSerializer.Deserialize<MessagePackObject>(unpackedOrder);

            return deserialized.Equals(MessagePackObject.Nil)
                ? null
                : Deserialize(unpackedOrder, ObjectExtractor.Guid(unpacked[Key.InitEventGuidTakeProfit]));
        }
    }
}
