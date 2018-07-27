// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackOrderSerializer.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.MsgPack
{
    using System;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Identifiers;
    using global::MsgPack;

    public class MsgPackOrderSerializer : IOrderSerializer
    {

        /// <summary>
        /// Serialize the given order to Message Pack specification bytes.
        /// </summary>
        /// <param name="order">The order to serialize.</param>
        /// <returns>The serialized order.</returns>
        public byte[] Serialize(Order order)
        {
            return MsgPackSerializer.Serialize(new MessagePackObjectDictionary
            {
                {new MessagePackObject(Key.Symbol), order.Symbol.ToString()},
                {new MessagePackObject(Key.OrderId), order.Id.Value},
                {new MessagePackObject(Key.Label), order.Label.ToString()},
                {new MessagePackObject(Key.OrderSide), order.Side.ToString()},
                {new MessagePackObject(Key.OrderType), order.Type.ToString()},
                {new MessagePackObject(Key.Quantity), order.Quantity.Value},
                {new MessagePackObject(Key.Price), MsgPackSerializationHelper.GetPriceString(order.Price)},
                {new MessagePackObject(Key.TimeInForce), order.TimeInForce.ToString()},
                {new MessagePackObject(Key.ExpireTime), MsgPackSerializationHelper.GetExpireTimeString(order.ExpireTime)},
                {new MessagePackObject(Key.Timestamp), order.Timestamp.ToIsoString()},
            }.Freeze());
        }

        public Order Deserialize(byte[] orderBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(orderBytes);

            var orderType = unpacked[Key.OrderType].ToString().ToEnum<OrderType>();
            var symbol = MsgPackSerializationHelper.GetSymbol(unpacked[Key.Symbol].ToString());
            var id = new OrderId(unpacked[Key.OrderId].ToString());
            var label = new Label(unpacked[Key.Label].ToString());
            var side = unpacked[Key.OrderSide].ToString().ToEnum<OrderSide>();
            var quantity = Quantity.Create(Convert.ToInt32(unpacked[Key.Quantity].ToString()));
            var timestamp = unpacked[Key.Timestamp].ToString().ToZonedDateTimeFromIso();

            switch (orderType)
            {
                case OrderType.UNKNOWN:
                    throw new InvalidOperationException(
                        "Cannot deserialize order (the type is unknown).");

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
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.Price].ToString()),
                        unpacked[Key.TimeInForce].ToString().ToEnum<TimeInForce>(),
                        MsgPackSerializationHelper.GetExpireTime(unpacked[Key.ExpireTime].ToString()),
                        timestamp);

                case OrderType.STOP_LIMIT:
                    return OrderFactory.StopLimit(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.Price].ToString()),
                        unpacked[Key.TimeInForce].ToString().ToEnum<TimeInForce>(),
                        MsgPackSerializationHelper.GetExpireTime(unpacked[Key.ExpireTime].ToString()),
                        timestamp);

                case OrderType.STOP_MARKET:
                    return OrderFactory.StopMarket(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.Price].ToString()),
                        unpacked[Key.TimeInForce].ToString().ToEnum<TimeInForce>(),
                        MsgPackSerializationHelper.GetExpireTime(unpacked[Key.ExpireTime].ToString()),
                        timestamp);

                case OrderType.MIT:
                    return OrderFactory.MarketIfTouched(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.Price].ToString()),
                        unpacked[Key.TimeInForce].ToString().ToEnum<TimeInForce>(),
                        MsgPackSerializationHelper.GetExpireTime(unpacked[Key.ExpireTime].ToString()),
                        timestamp);

                default:
                    throw new InvalidOperationException(
                        "Cannot deserialize order (the order type is not recognized).");
            }
        }
    }
}
