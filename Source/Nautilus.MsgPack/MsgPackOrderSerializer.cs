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
    using System.Globalization;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Orders;
    using Nautilus.DomainModel.ValueObjects;
    using global::MsgPack;

    public class MsgPackOrderSerializer : IOrderSerializer
    {
        private const string None = "NONE";

        public byte[] Serialize(Order order)
        {
            var package = new MessagePackObjectDictionary
            {
                {new MessagePackObject(Key.Symbol), order.Symbol.ToString()},
                {new MessagePackObject(Key.OrderId), order.Id.Value},
                {new MessagePackObject(Key.Label), order.Label.ToString()},
                {new MessagePackObject(Key.OrderSide), order.Side.ToString()},
                {new MessagePackObject(Key.OrderType), order.Type.ToString()},
                {new MessagePackObject(Key.Quantity), order.Quantity.Value},
                {new MessagePackObject(Key.Timestamp), order.Timestamp.ToIsoString()},
            };

            switch (order)
            {
                case MarketOrder marketOrder:
                    package.Add(new MessagePackObject(Key.Price), None);
                    package.Add(new MessagePackObject(Key.TimeInForce), None);
                    package.Add(new MessagePackObject(Key.ExpireTime), None);
                    break;

                case PricedOrder pricedOrder:
                    package.Add(new MessagePackObject(Key.Price), pricedOrder
                        .Price
                        .Value
                        .ToString(CultureInfo.InvariantCulture));
                    package.Add(new MessagePackObject(Key.TimeInForce), pricedOrder.TimeInForce.ToString());
                    package.Add(new MessagePackObject(Key.ExpireTime),
                        MsgPackSerializationHelper.GetExpireTimeString(pricedOrder.ExpireTime));
                    break;

                default: throw new InvalidOperationException(
                    "Cannot serialize the order (unrecognized order).");
            }

            return MsgPackSerializer.Serialize(package.Freeze());
        }

        public Order Deserialize(byte[] orderBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(orderBytes);

            var orderType = unpacked[Key.OrderType].ToString().ToEnum<OrderType>();
            var symbol = MsgPackSerializationHelper.GetSymbol(unpacked[Key.Symbol].ToString());
            var id = new EntityId(unpacked[Key.OrderId].ToString());
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
                    return new MarketOrder(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        timestamp);

                case OrderType.LIMIT:
                    return new LimitOrder(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.Price].ToString()).Value,
                        unpacked[Key.TimeInForce].ToString().ToEnum<TimeInForce>(),
                        MsgPackSerializationHelper.GetExpireTime(unpacked[Key.ExpireTime].ToString()),
                        timestamp);

                case OrderType.STOP_LIMIT:
                    return new StopLimitOrder(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.Price].ToString()).Value,
                        unpacked[Key.TimeInForce].ToString().ToEnum<TimeInForce>(),
                        MsgPackSerializationHelper.GetExpireTime(unpacked[Key.ExpireTime].ToString()),
                        timestamp);

                case OrderType.STOP_MARKET:
                    return new StopMarketOrder(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.Price].ToString()).Value,
                        unpacked[Key.TimeInForce].ToString().ToEnum<TimeInForce>(),
                        MsgPackSerializationHelper.GetExpireTime(unpacked[Key.ExpireTime].ToString()),
                        timestamp);

                case OrderType.MIT:
                    return new StopLimitOrder(
                        symbol,
                        id,
                        label,
                        side,
                        quantity,
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.Price].ToString()).Value,
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
