// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackOrderSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System;
    using MsgPack;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;

    /// <summary>
    /// Provides serialization of <see cref="Order"/> objects to Message Pack specification.
    /// </summary>
    public static class MsgPackOrderSerializer
    {
        /// <summary>
        /// Serialize the given order to Message Pack specification bytes.
        /// </summary>
        /// <param name="order">The order to serialize.</param>
        /// <returns>The serialized order.</returns>
        public static byte[] Serialize(Order order)
        {
            return MsgPackSerializer.Serialize(new MessagePackObjectDictionary
            {
                { Key.Symbol, order.Symbol.ToString() },
                { Key.OrderId, order.Id.ToString() },
                { Key.Label, order.Label.ToString() },
                { Key.OrderSide, order.Side.ToString() },
                { Key.OrderType, order.Type.ToString() },
                { Key.Quantity, order.Quantity.Value },
                { Key.Price, MsgPackObjectConverter.FromNullablePrice(order.Price) },
                { Key.TimeInForce, order.TimeInForce.ToString() },
                { Key.ExpireTime, MsgPackObjectConverter.ToExpireTime(order.ExpireTime) },
                { Key.Timestamp, order.Timestamp.ToIsoString() },
            });
        }

        /// <summary>
        /// Deserialize the given byte array to an <see cref="Order"/>.
        /// </summary>
        /// <param name="orderBytes">The order bytes.</param>
        /// <returns>The deserialized order.</returns>
        /// <exception cref="InvalidOperationException">If the order type is unknown.</exception>
        public static Order Deserialize(byte[] orderBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(orderBytes);

            var orderType = MsgPackObjectConverter.ToOrderType(unpacked[Key.OrderType]);
            var symbol = MsgPackObjectConverter.ToSymbol(unpacked[Key.Symbol]);
            var id = MsgPackObjectConverter.ToOrderId(unpacked[Key.OrderId]);
            var label = MsgPackObjectConverter.ToLabel(unpacked[Key.Label]);
            var side = MsgPackObjectConverter.ToOrderSide(unpacked[Key.OrderSide]);
            var quantity = MsgPackObjectConverter.ToQuantity(unpacked[Key.Quantity]);
            var timestamp = MsgPackObjectConverter.ToZonedDateTime(unpacked[Key.Timestamp]);

            switch (orderType)
            {
                case OrderType.MARKET:
                    return OrderFactory.Market(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        timestamp);
                case OrderType.LIMIT:
                    return OrderFactory.Limit(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        MsgPackObjectConverter.ToPrice(unpacked[Key.Price]),
                        MsgPackObjectConverter.ToTimeInForce(unpacked[Key.TimeInForce]),
                        MsgPackObjectConverter.ToExpireTime(unpacked[Key.ExpireTime]),
                        timestamp);
                case OrderType.STOP_LIMIT:
                    return OrderFactory.StopLimit(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        MsgPackObjectConverter.ToPrice(unpacked[Key.Price]),
                        MsgPackObjectConverter.ToTimeInForce(unpacked[Key.TimeInForce]),
                        MsgPackObjectConverter.ToExpireTime(unpacked[Key.ExpireTime]),
                        timestamp);
                case OrderType.STOP_MARKET:
                    return OrderFactory.StopMarket(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        MsgPackObjectConverter.ToPrice(unpacked[Key.Price]),
                        MsgPackObjectConverter.ToTimeInForce(unpacked[Key.TimeInForce]),
                        MsgPackObjectConverter.ToExpireTime(unpacked[Key.ExpireTime]),
                        timestamp);
                case OrderType.MIT:
                    return OrderFactory.MarketIfTouched(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        MsgPackObjectConverter.ToPrice(unpacked[Key.Price]),
                        MsgPackObjectConverter.ToTimeInForce(unpacked[Key.TimeInForce]),
                        MsgPackObjectConverter.ToExpireTime(unpacked[Key.ExpireTime]),
                        timestamp);
                case OrderType.UNKNOWN:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(orderType, nameof(orderType));
            }
        }

        /// <summary>
        /// Returns the given nullable take profit order as a <see cref="MessagePackObject"/>.
        /// </summary>
        /// <param name="takeProfit">The take profit order.</param>
        /// <returns>The <see cref="MessagePackObject"/>.</returns>
        internal static MessagePackObject SerializeTakeProfit(Order? takeProfit)
        {
            return takeProfit != null
                ? Serialize(takeProfit)
                : MessagePackObject.Nil;
        }

        /// <summary>
        /// Returns the nullable take profit <see cref="Order"/>.
        /// </summary>
        /// <param name="takeProfit">The take profit order bytes.</param>
        /// <param name="hasTakeProfit">The has take profit flag.</param>
        /// <returns>The nullable <see cref="Order"/>.</returns>
        internal static Order? DeserializeTakeProfit(byte[] takeProfit, bool hasTakeProfit)
        {
            return hasTakeProfit
                ? Deserialize(takeProfit)
                : null;
        }
    }
}
