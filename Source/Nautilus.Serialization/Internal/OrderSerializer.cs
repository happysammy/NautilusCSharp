// -------------------------------------------------------------------------------------------------
// <copyright file="OrderSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.Internal
{
    using System.ComponentModel;
    using MsgPack;
    using Nautilus.Common.Componentry;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Provides serialization of <see cref="Order"/> objects to MessagePack specification bytes.
    /// </summary>
    internal class OrderSerializer
    {
        private static readonly byte[] Empty = MsgPackSerializer.Serialize(new MessagePackObjectDictionary());

        private readonly ObjectCache<string, Symbol> symbolCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSerializer"/> class.
        /// </summary>
        public OrderSerializer()
        {
            this.symbolCache = new ObjectCache<string, Symbol>(Symbol.FromString);
        }

        /// <summary>
        /// Returns the given <see cref="Order"/> serialized to MessagePack specification bytes.
        /// </summary>
        /// <param name="order">The order to serialize.</param>
        /// <returns>The serialized bytes.</returns>
        internal byte[] Serialize(Order order)
        {
            return MsgPackSerializer.Serialize(new MessagePackObjectDictionary
            {
                { nameof(Order.Id), order.Id.Value },
                { nameof(Order.Symbol), order.Symbol.Value },
                { nameof(Order.Label), order.Label.Value },
                { nameof(OrderSide), order.Side.ToString() },
                { nameof(OrderType), order.Type.ToString() },
                { nameof(Order.Quantity), order.Quantity.Value },
                { nameof(Order.Price), ObjectPacker.Pack(order.Price) },
                { nameof(Order.TimeInForce), order.TimeInForce.ToString() },
                { nameof(Order.ExpireTime), ObjectPacker.Pack(order.ExpireTime) },
                { nameof(Order.Timestamp), order.Timestamp.ToIsoString() },
                { nameof(Order.InitId), order.InitialEvent.Id.ToString() },
            });
        }

        /// <summary>
        /// Returns the given <see cref="Order"/>? serialized to MessagePack specification bytes.
        /// </summary>
        /// <param name="order">The nullable order to serialize.</param>
        /// <returns>The serialized nullable order bytes.</returns>
        internal byte[] SerializeNullable(Order? order)
        {
            return order == null ? Empty : this.Serialize(order);
        }

        /// <summary>
        /// Returns the given bytes deserialized to an <see cref="Order"/>.
        /// </summary>
        /// <param name="orderBytes">The order bytes.</param>
        /// <returns>The deserialized <see cref="Order"/>.</returns>
        internal Order Deserialize(byte[] orderBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(orderBytes);

            return this.Deserialize(unpacked);
        }

        /// <summary>
        /// Returns the given bytes deserialized to an <see cref="Order"/>?.
        /// </summary>
        /// <param name="orderBytes">The message pack object dictionary.</param>
        /// <returns>The deserialized <see cref="Order"/>?.</returns>
        internal Order? DeserializeNullable(byte[] orderBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(orderBytes);

            return unpacked.Count == 0 ? null : this.Deserialize(unpacked);
        }

        /// <summary>
        /// Returns the given <see cref="MessagePackObjectDictionary"/> deserialized to an <see cref="Order"/>.
        /// </summary>
        /// <param name="unpacked">The unpacked order object dictionary.</param>
        /// <returns>The deserialized <see cref="Order"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">If the order type is unknown.</exception>
        private Order Deserialize(MessagePackObjectDictionary unpacked)
        {
            var type = ObjectExtractor.Enum<OrderType>(unpacked[nameof(OrderType)]);
            var id = ObjectExtractor.OrderId(unpacked, nameof(Order.Id));
            var symbol = this.symbolCache.Get(unpacked[nameof(Symbol)].AsString());
            var label = ObjectExtractor.Label(unpacked);
            var side = ObjectExtractor.Enum<OrderSide>(unpacked[nameof(OrderSide)]);
            var quantity = ObjectExtractor.Quantity(unpacked[nameof(Order.Quantity)]);
            var timestamp = ObjectExtractor.ZonedDateTime(unpacked[nameof(Order.Timestamp)]);
            var initialId = ObjectExtractor.Guid(unpacked[nameof(Order.InitId)]);

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
                        initialId);
                case OrderType.LIMIT:
                    return OrderFactory.Limit(
                        id,
                        symbol,
                        label,
                        side,
                        quantity,
                        ObjectExtractor.Price(unpacked[nameof(Order.Price)]),
                        ObjectExtractor.Enum<TimeInForce>(unpacked[nameof(Order.TimeInForce)]),
                        ObjectExtractor.NullableZonedDateTime(unpacked[nameof(Order.ExpireTime)]),
                        timestamp,
                        initialId);
                case OrderType.STOP_LIMIT:
                    return OrderFactory.StopLimit(
                        id,
                        symbol,
                        label,
                        side,
                        quantity,
                        ObjectExtractor.Price(unpacked[nameof(Order.Price)]),
                        ObjectExtractor.Enum<TimeInForce>(unpacked[nameof(Order.TimeInForce)]),
                        ObjectExtractor.NullableZonedDateTime(unpacked[nameof(Order.ExpireTime)]),
                        timestamp,
                        initialId);
                case OrderType.STOP_MARKET:
                    return OrderFactory.StopMarket(
                        id,
                        symbol,
                        label,
                        side,
                        quantity,
                        ObjectExtractor.Price(unpacked[nameof(Order.Price)]),
                        ObjectExtractor.Enum<TimeInForce>(unpacked[nameof(Order.TimeInForce)]),
                        ObjectExtractor.NullableZonedDateTime(unpacked[nameof(Order.ExpireTime)]),
                        timestamp,
                        initialId);
                case OrderType.MIT:
                    return OrderFactory.MarketIfTouched(
                        id,
                        symbol,
                        label,
                        side,
                        quantity,
                        ObjectExtractor.Price(unpacked[nameof(Order.Price)]),
                        ObjectExtractor.Enum<TimeInForce>(unpacked[nameof(Order.TimeInForce)]),
                        ObjectExtractor.NullableZonedDateTime(unpacked[nameof(Order.ExpireTime)]),
                        timestamp,
                        initialId);
                case OrderType.UNKNOWN:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(type, nameof(type));
            }
        }
    }
}
