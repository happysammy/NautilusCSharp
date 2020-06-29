// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackOrderSerializer.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.Serialization.MessageSerializers.Internal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using MessagePack;
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
    internal sealed class MsgPackOrderSerializer
    {
        private static readonly Func<string, byte[]> Encode = Encoding.UTF8.GetBytes;
        private static readonly Func<byte[], string> Decode = Encoding.UTF8.GetString;
        private static readonly byte[] EmptyDictBytes = MessagePackSerializer.Serialize(new Dictionary<string, byte[]>());

        private readonly ObjectCache<string, Symbol> symbolCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsgPackOrderSerializer"/> class.
        /// </summary>
        public MsgPackOrderSerializer()
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
            return MessagePackSerializer.Serialize(new Dictionary<string, byte[]>
            {
                { nameof(Order.Id), Encode(order.Id.Value) },
                { nameof(Order.Symbol), Encode(order.Symbol.Value) },
                { nameof(Order.Label), Encode(order.Label.Value) },
                { nameof(OrderSide), Encode(order.OrderSide.ToString()) },
                { nameof(OrderType), Encode(order.OrderType.ToString()) },
                { nameof(OrderPurpose), Encode(order.OrderPurpose.ToString()) },
                { nameof(Order.Quantity), Encode(order.Quantity.ToString()) },
                { nameof(Order.Price), ObjectSerializer.Serialize(order.Price) },
                { nameof(Order.TimeInForce), Encode(order.TimeInForce.ToString()) },
                { nameof(Order.ExpireTime), ObjectSerializer.Serialize(order.ExpireTime) },
                { nameof(Order.Timestamp), Encode(order.Timestamp.ToIso8601String()) },
                { nameof(Order.InitId), Encode(order.InitialEvent.Id.ToString()) },
            });
        }

        /// <summary>
        /// Returns the given <see cref="Order"/>? serialized to MessagePack specification bytes.
        /// </summary>
        /// <param name="order">The nullable order to serialize.</param>
        /// <returns>The serialized nullable order bytes.</returns>
        internal byte[] SerializeNullable(Order? order)
        {
            return order == null ? EmptyDictBytes : this.Serialize(order);
        }

        /// <summary>
        /// Returns the given bytes deserialized to an <see cref="Order"/>.
        /// </summary>
        /// <param name="packed">The packed order.</param>
        /// <returns>The deserialized <see cref="Order"/>.</returns>
        internal Order Deserialize(byte[] packed)
        {
            return this.Deserialize(MessagePackSerializer.Deserialize<Dictionary<string, byte[]>>(packed));
        }

        /// <summary>
        /// Returns the given bytes deserialized to an <see cref="Order"/>?.
        /// </summary>
        /// <param name="packed">The message pack object dictionary.</param>
        /// <returns>The deserialized <see cref="Order"/>?.</returns>
        internal Order? DeserializeNullable(byte[] packed)
        {
            var deserialized = MessagePackSerializer.Deserialize<Dictionary<string, byte[]>>(packed);
            return deserialized.Count == 0 ? null : this.Deserialize(deserialized);
        }

        /// <summary>
        /// Returns an <see cref="Order"/> from the given deserialized dictionary.
        /// </summary>
        /// <param name="unpacked">The unpacked order object dictionary.</param>
        /// <returns>The deserialized <see cref="Order"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">If the order type is unknown.</exception>
        private Order Deserialize(Dictionary<string, byte[]> unpacked)
        {
            var type = ObjectDeserializer.AsEnum<OrderType>(unpacked[nameof(OrderType)]);
            var id = ObjectDeserializer.AsOrderId(unpacked, nameof(Order.Id));
            var symbol = this.symbolCache.Get(ObjectDeserializer.AsString(unpacked[nameof(Symbol)]));
            var label = ObjectDeserializer.AsLabel(unpacked);
            var side = ObjectDeserializer.AsEnum<OrderSide>(unpacked[nameof(OrderSide)]);
            var purpose = ObjectDeserializer.AsEnum<OrderPurpose>(unpacked[nameof(OrderPurpose)]);
            var quantity = ObjectDeserializer.AsQuantity(unpacked[nameof(Order.Quantity)]);
            var timestamp = ObjectDeserializer.AsZonedDateTime(unpacked[nameof(Order.Timestamp)]);
            var initialId = ObjectDeserializer.AsGuid(unpacked[nameof(Order.InitId)]);

            switch (type)
            {
                case OrderType.Market:
                    return OrderFactory.Market(
                        id,
                        symbol,
                        label,
                        side,
                        purpose,
                        quantity,
                        timestamp,
                        initialId);
                case OrderType.Limit:
                    return OrderFactory.Limit(
                        id,
                        symbol,
                        label,
                        side,
                        purpose,
                        quantity,
                        ObjectDeserializer.AsPrice(unpacked[nameof(Order.Price)]),
                        ObjectDeserializer.AsEnum<TimeInForce>(unpacked[nameof(Order.TimeInForce)]),
                        ObjectDeserializer.AsNullableZonedDateTime(unpacked[nameof(Order.ExpireTime)]),
                        timestamp,
                        initialId);
                case OrderType.StopLimit:
                    return OrderFactory.StopLimit(
                        id,
                        symbol,
                        label,
                        side,
                        purpose,
                        quantity,
                        ObjectDeserializer.AsPrice(unpacked[nameof(Order.Price)]),
                        ObjectDeserializer.AsEnum<TimeInForce>(unpacked[nameof(Order.TimeInForce)]),
                        ObjectDeserializer.AsNullableZonedDateTime(unpacked[nameof(Order.ExpireTime)]),
                        timestamp,
                        initialId);
                case OrderType.Stop:
                    return OrderFactory.StopMarket(
                        id,
                        symbol,
                        label,
                        side,
                        purpose,
                        quantity,
                        ObjectDeserializer.AsPrice(unpacked[nameof(Order.Price)]),
                        ObjectDeserializer.AsEnum<TimeInForce>(unpacked[nameof(Order.TimeInForce)]),
                        ObjectDeserializer.AsNullableZonedDateTime(unpacked[nameof(Order.ExpireTime)]),
                        timestamp,
                        initialId);
                case OrderType.MIT:
                    return OrderFactory.MarketIfTouched(
                        id,
                        symbol,
                        label,
                        side,
                        purpose,
                        quantity,
                        ObjectDeserializer.AsPrice(unpacked[nameof(Order.Price)]),
                        ObjectDeserializer.AsEnum<TimeInForce>(unpacked[nameof(Order.TimeInForce)]),
                        ObjectDeserializer.AsNullableZonedDateTime(unpacked[nameof(Order.ExpireTime)]),
                        timestamp,
                        initialId);
                case OrderType.Undefined:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(type, nameof(type));
            }
        }
    }
}
