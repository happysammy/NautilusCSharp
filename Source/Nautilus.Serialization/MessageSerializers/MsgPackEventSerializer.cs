// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackEventSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization.MessageSerializers
{
    using System.Collections.Generic;
    using MessagePack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.Serialization.MessageSerializers.Internal;

    /// <summary>
    /// Provides an <see cref="Event"/> message binary serializer for the MessagePack specification.
    /// </summary>
    public sealed class MsgPackEventSerializer : ISerializer<Event>
    {
        private readonly IdentifierCache identifierCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsgPackEventSerializer"/> class.
        /// </summary>
        public MsgPackEventSerializer()
        {
            this.identifierCache = new IdentifierCache();
        }

                /// <inheritdoc />
        public byte[] Serialize(Event @event)
        {
            var package = new Dictionary<string, byte[]>
            {
                { nameof(Event.Type), ObjectSerializer.Serialize(@event.Type) },
                { nameof(Event.Id), ObjectSerializer.Serialize(@event.Id) },
                { nameof(Event.Timestamp), ObjectSerializer.Serialize(@event.Timestamp) },
            };

            switch (@event)
            {
                case AccountStateEvent evt:
                    package.Add(nameof(evt.AccountId), ObjectSerializer.Serialize(evt.AccountId));
                    package.Add(nameof(evt.Currency), ObjectSerializer.Serialize(evt.Currency));
                    package.Add(nameof(evt.CashBalance), ObjectSerializer.Serialize(evt.CashBalance));
                    package.Add(nameof(evt.CashStartDay), ObjectSerializer.Serialize(evt.CashStartDay));
                    package.Add(nameof(evt.CashActivityDay), ObjectSerializer.Serialize(evt.CashActivityDay));
                    package.Add(nameof(evt.MarginUsedLiquidation), ObjectSerializer.Serialize(evt.MarginUsedLiquidation));
                    package.Add(nameof(evt.MarginUsedMaintenance), ObjectSerializer.Serialize(evt.MarginUsedMaintenance));
                    package.Add(nameof(evt.MarginRatio), ObjectSerializer.Serialize(evt.MarginRatio));
                    package.Add(nameof(evt.MarginCallStatus), ObjectSerializer.Serialize(evt.MarginCallStatus));
                    break;
                case OrderInitialized evt:
                    package.Add(nameof(evt.OrderId), ObjectSerializer.Serialize(evt.OrderId));
                    package.Add(nameof(evt.Symbol), ObjectSerializer.Serialize(evt.Symbol));
                    package.Add(nameof(evt.Label), ObjectSerializer.Serialize(evt.Label));
                    package.Add(nameof(evt.OrderSide), ObjectSerializer.Serialize(evt.OrderSide));
                    package.Add(nameof(evt.OrderType), ObjectSerializer.Serialize(evt.OrderType));
                    package.Add(nameof(evt.OrderPurpose), ObjectSerializer.Serialize(evt.OrderPurpose));
                    package.Add(nameof(evt.Quantity), ObjectSerializer.Serialize(evt.Quantity));
                    package.Add(nameof(evt.Price), ObjectSerializer.Serialize(evt.Price));
                    package.Add(nameof(evt.TimeInForce), ObjectSerializer.Serialize(evt.TimeInForce));
                    package.Add(nameof(evt.ExpireTime), ObjectSerializer.Serialize(evt.ExpireTime));
                    break;
                case OrderInvalid evt:
                    package.Add(nameof(evt.OrderId), ObjectSerializer.Serialize(evt.OrderId));
                    package.Add(nameof(evt.InvalidReason), ObjectSerializer.Serialize(evt.InvalidReason));
                    break;
                case OrderDenied evt:
                    package.Add(nameof(evt.OrderId), ObjectSerializer.Serialize(evt.OrderId));
                    package.Add(nameof(evt.DeniedReason), ObjectSerializer.Serialize(evt.DeniedReason));
                    break;
                case OrderSubmitted evt:
                    package.Add(nameof(evt.AccountId), ObjectSerializer.Serialize(evt.AccountId));
                    package.Add(nameof(evt.OrderId), ObjectSerializer.Serialize(evt.OrderId));
                    package.Add(nameof(evt.SubmittedTime), ObjectSerializer.Serialize(evt.SubmittedTime));
                    break;
                case OrderAccepted evt:
                    package.Add(nameof(evt.AccountId), ObjectSerializer.Serialize(evt.AccountId));
                    package.Add(nameof(evt.OrderId), ObjectSerializer.Serialize(evt.OrderId));
                    package.Add(nameof(evt.OrderIdBroker), ObjectSerializer.Serialize(evt.OrderIdBroker));
                    package.Add(nameof(evt.Label), ObjectSerializer.Serialize(evt.Label));
                    package.Add(nameof(evt.AcceptedTime), ObjectSerializer.Serialize(evt.AcceptedTime));
                    break;
                case OrderRejected evt:
                    package.Add(nameof(evt.AccountId), ObjectSerializer.Serialize(evt.AccountId));
                    package.Add(nameof(evt.OrderId), ObjectSerializer.Serialize(evt.OrderId));
                    package.Add(nameof(evt.RejectedTime), ObjectSerializer.Serialize(evt.RejectedTime));
                    package.Add(nameof(evt.RejectedReason), ObjectSerializer.Serialize(evt.RejectedReason));
                    break;
                case OrderWorking evt:
                    package.Add(nameof(evt.AccountId), ObjectSerializer.Serialize(evt.AccountId));
                    package.Add(nameof(evt.OrderId), ObjectSerializer.Serialize(evt.OrderId));
                    package.Add(nameof(evt.OrderIdBroker), ObjectSerializer.Serialize(evt.OrderIdBroker));
                    package.Add(nameof(evt.Symbol), ObjectSerializer.Serialize(evt.Symbol));
                    package.Add(nameof(evt.Label), ObjectSerializer.Serialize(evt.Label));
                    package.Add(nameof(evt.OrderSide), ObjectSerializer.Serialize(evt.OrderSide));
                    package.Add(nameof(evt.OrderType), ObjectSerializer.Serialize(evt.OrderType));
                    package.Add(nameof(evt.Quantity), ObjectSerializer.Serialize(evt.Quantity));
                    package.Add(nameof(evt.Price), ObjectSerializer.Serialize(evt.Price));
                    package.Add(nameof(evt.TimeInForce), ObjectSerializer.Serialize(evt.TimeInForce));
                    package.Add(nameof(evt.ExpireTime), ObjectSerializer.Serialize(evt.ExpireTime));
                    package.Add(nameof(evt.WorkingTime), ObjectSerializer.Serialize(evt.WorkingTime));
                    break;
                case OrderCancelled evt:
                    package.Add(nameof(evt.AccountId), ObjectSerializer.Serialize(evt.AccountId));
                    package.Add(nameof(evt.OrderId), ObjectSerializer.Serialize(evt.OrderId));
                    package.Add(nameof(evt.CancelledTime), ObjectSerializer.Serialize(evt.CancelledTime));
                    break;
                case OrderCancelReject evt:
                    package.Add(nameof(evt.AccountId), ObjectSerializer.Serialize(evt.AccountId));
                    package.Add(nameof(evt.OrderId), ObjectSerializer.Serialize(evt.OrderId));
                    package.Add(nameof(evt.RejectedTime), ObjectSerializer.Serialize(evt.RejectedTime));
                    package.Add(nameof(evt.RejectedResponseTo), ObjectSerializer.Serialize(evt.RejectedResponseTo));
                    package.Add(nameof(evt.RejectedReason), ObjectSerializer.Serialize(evt.RejectedReason));
                    break;
                case OrderModified evt:
                    package.Add(nameof(evt.AccountId), ObjectSerializer.Serialize(evt.AccountId));
                    package.Add(nameof(evt.OrderId), ObjectSerializer.Serialize(evt.OrderId));
                    package.Add(nameof(evt.OrderIdBroker), ObjectSerializer.Serialize(evt.OrderIdBroker));
                    package.Add(nameof(evt.ModifiedQuantity), ObjectSerializer.Serialize(evt.ModifiedQuantity));
                    package.Add(nameof(evt.ModifiedPrice), ObjectSerializer.Serialize(evt.ModifiedPrice));
                    package.Add(nameof(evt.ModifiedTime), ObjectSerializer.Serialize(evt.ModifiedTime));
                    break;
                case OrderExpired evt:
                    package.Add(nameof(evt.AccountId), ObjectSerializer.Serialize(evt.AccountId));
                    package.Add(nameof(evt.OrderId), ObjectSerializer.Serialize(evt.OrderId));
                    package.Add(nameof(evt.ExpiredTime), ObjectSerializer.Serialize(evt.ExpiredTime));
                    break;
                case OrderPartiallyFilled evt:
                    package.Add(nameof(evt.AccountId), ObjectSerializer.Serialize(evt.AccountId));
                    package.Add(nameof(evt.OrderId), ObjectSerializer.Serialize(evt.OrderId));
                    package.Add(nameof(evt.ExecutionId), ObjectSerializer.Serialize(evt.ExecutionId));
                    package.Add(nameof(evt.PositionIdBroker), ObjectSerializer.Serialize(evt.PositionIdBroker));
                    package.Add(nameof(evt.Symbol), ObjectSerializer.Serialize(evt.Symbol));
                    package.Add(nameof(evt.OrderSide), ObjectSerializer.Serialize(evt.OrderSide));
                    package.Add(nameof(evt.FilledQuantity), ObjectSerializer.Serialize(evt.FilledQuantity));
                    package.Add(nameof(evt.LeavesQuantity), ObjectSerializer.Serialize(evt.LeavesQuantity));
                    package.Add(nameof(evt.AveragePrice), ObjectSerializer.Serialize(evt.AveragePrice));
                    package.Add(nameof(evt.Currency), ObjectSerializer.Serialize(evt.Currency));
                    package.Add(nameof(evt.ExecutionTime), ObjectSerializer.Serialize(evt.ExecutionTime));
                    break;
                case OrderFilled evt:
                    package.Add(nameof(evt.AccountId), ObjectSerializer.Serialize(evt.AccountId));
                    package.Add(nameof(evt.OrderId), ObjectSerializer.Serialize(evt.OrderId));
                    package.Add(nameof(evt.ExecutionId), ObjectSerializer.Serialize(evt.ExecutionId));
                    package.Add(nameof(evt.PositionIdBroker), ObjectSerializer.Serialize(evt.PositionIdBroker));
                    package.Add(nameof(evt.Symbol), ObjectSerializer.Serialize(evt.Symbol));
                    package.Add(nameof(evt.OrderSide), ObjectSerializer.Serialize(evt.OrderSide));
                    package.Add(nameof(evt.FilledQuantity), ObjectSerializer.Serialize(evt.FilledQuantity));
                    package.Add(nameof(evt.AveragePrice), ObjectSerializer.Serialize(evt.AveragePrice));
                    package.Add(nameof(evt.Currency), ObjectSerializer.Serialize(evt.Currency));
                    package.Add(nameof(evt.ExecutionTime), ObjectSerializer.Serialize(evt.ExecutionTime));
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(@event, nameof(@event));
            }

            return MessagePackSerializer.Serialize(package);
        }

        /// <inheritdoc />
        public Event Deserialize(byte[] dataBytes)
        {
            var unpacked = MessagePackSerializer.Deserialize<Dictionary<string, byte[]>>(dataBytes);

            var @event = ObjectDeserializer.AsString(unpacked[nameof(Event.Type)]);
            var id = ObjectDeserializer.AsGuid(unpacked[nameof(Event.Id)]);
            var timestamp = ObjectDeserializer.AsZonedDateTime(unpacked[nameof(Event.Timestamp)]);

            switch (@event)
            {
                case nameof(AccountStateEvent):
                    var currency = ObjectDeserializer.AsEnum<Currency>(unpacked[nameof(AccountStateEvent.Currency)]);
                    return new AccountStateEvent(
                        this.identifierCache.AccountId(unpacked),
                        currency,
                        ObjectDeserializer.AsMoney(unpacked[nameof(AccountStateEvent.CashBalance)], currency),
                        ObjectDeserializer.AsMoney(unpacked[nameof(AccountStateEvent.CashStartDay)], currency),
                        ObjectDeserializer.AsMoney(unpacked[nameof(AccountStateEvent.CashActivityDay)], currency),
                        ObjectDeserializer.AsMoney(unpacked[nameof(AccountStateEvent.MarginUsedLiquidation)], currency),
                        ObjectDeserializer.AsMoney(unpacked[nameof(AccountStateEvent.MarginUsedMaintenance)], currency),
                        ObjectDeserializer.AsDecimal(unpacked[nameof(AccountStateEvent.MarginRatio)]),
                        ObjectDeserializer.AsString(unpacked[nameof(AccountStateEvent.MarginCallStatus)]),
                        id,
                        timestamp);
                case nameof(OrderInitialized):
                    return new OrderInitialized(
                        ObjectDeserializer.AsOrderId(unpacked),
                        this.identifierCache.Symbol(unpacked),
                        ObjectDeserializer.AsLabel(unpacked),
                        ObjectDeserializer.AsEnum<OrderSide>(unpacked[nameof(OrderInitialized.OrderSide)]),
                        ObjectDeserializer.AsEnum<OrderType>(unpacked[nameof(OrderInitialized.OrderType)]),
                        ObjectDeserializer.AsEnum<OrderPurpose>(unpacked[nameof(OrderInitialized.OrderPurpose)]),
                        ObjectDeserializer.AsQuantity(unpacked[nameof(OrderInitialized.Quantity)]),
                        ObjectDeserializer.AsNullablePrice(unpacked[nameof(OrderInitialized.Price)]),
                        ObjectDeserializer.AsEnum<TimeInForce>(unpacked[nameof(OrderInitialized.TimeInForce)]),
                        ObjectDeserializer.AsNullableZonedDateTime(unpacked[nameof(OrderInitialized.ExpireTime)]),
                        id,
                        timestamp);
                case nameof(OrderInvalid):
                    return new OrderInvalid(
                        ObjectDeserializer.AsOrderId(unpacked),
                        ObjectDeserializer.AsString(unpacked[nameof(OrderInvalid.InvalidReason)]),
                        id,
                        timestamp);
                case nameof(OrderDenied):
                    return new OrderDenied(
                        ObjectDeserializer.AsOrderId(unpacked),
                        ObjectDeserializer.AsString(unpacked[nameof(OrderDenied.DeniedReason)]),
                        id,
                        timestamp);
                case nameof(OrderSubmitted):
                    return new OrderSubmitted(
                        this.identifierCache.AccountId(unpacked),
                        ObjectDeserializer.AsOrderId(unpacked),
                        ObjectDeserializer.AsZonedDateTime(unpacked[nameof(OrderSubmitted.SubmittedTime)]),
                        id,
                        timestamp);
                case nameof(OrderAccepted):
                    return new OrderAccepted(
                        this.identifierCache.AccountId(unpacked),
                        ObjectDeserializer.AsOrderId(unpacked),
                        ObjectDeserializer.AsOrderIdBroker(unpacked),
                        ObjectDeserializer.AsLabel(unpacked),
                        ObjectDeserializer.AsZonedDateTime(unpacked[nameof(OrderAccepted.AcceptedTime)]),
                        id,
                        timestamp);
                case nameof(OrderRejected):
                    return new OrderRejected(
                        this.identifierCache.AccountId(unpacked),
                        ObjectDeserializer.AsOrderId(unpacked),
                        ObjectDeserializer.AsZonedDateTime(unpacked[nameof(OrderRejected.RejectedTime)]),
                        ObjectDeserializer.AsString(unpacked[nameof(OrderRejected.RejectedReason)]),
                        id,
                        timestamp);
                case nameof(OrderWorking):
                    return new OrderWorking(
                        this.identifierCache.AccountId(unpacked),
                        ObjectDeserializer.AsOrderId(unpacked),
                        ObjectDeserializer.AsOrderIdBroker(unpacked),
                        this.identifierCache.Symbol(unpacked),
                        ObjectDeserializer.AsLabel(unpacked),
                        ObjectDeserializer.AsEnum<OrderSide>(unpacked[nameof(OrderWorking.OrderSide)]),
                        ObjectDeserializer.AsEnum<OrderType>(unpacked[nameof(OrderWorking.OrderType)]),
                        ObjectDeserializer.AsQuantity(unpacked[nameof(OrderWorking.Quantity)]),
                        ObjectDeserializer.AsPrice(unpacked[nameof(OrderWorking.Price)]),
                        ObjectDeserializer.AsEnum<TimeInForce>(unpacked[nameof(OrderWorking.TimeInForce)]),
                        ObjectDeserializer.AsNullableZonedDateTime(unpacked[nameof(OrderWorking.ExpireTime)]),
                        ObjectDeserializer.AsZonedDateTime(unpacked[nameof(OrderWorking.WorkingTime)]),
                        id,
                        timestamp);
                case nameof(OrderCancelled):
                    return new OrderCancelled(
                        this.identifierCache.AccountId(unpacked),
                        ObjectDeserializer.AsOrderId(unpacked),
                        ObjectDeserializer.AsZonedDateTime(unpacked[nameof(OrderCancelled.CancelledTime)]),
                        id,
                        timestamp);
                case nameof(OrderCancelReject):
                    return new OrderCancelReject(
                        this.identifierCache.AccountId(unpacked),
                        ObjectDeserializer.AsOrderId(unpacked),
                        ObjectDeserializer.AsZonedDateTime(unpacked[nameof(OrderCancelReject.RejectedTime)]),
                        ObjectDeserializer.AsString(unpacked[nameof(OrderCancelReject.RejectedResponseTo)]),
                        ObjectDeserializer.AsString(unpacked[nameof(OrderCancelReject.RejectedReason)]),
                        id,
                        timestamp);
                case nameof(OrderModified):
                    return new OrderModified(
                        this.identifierCache.AccountId(unpacked),
                        ObjectDeserializer.AsOrderId(unpacked),
                        ObjectDeserializer.AsOrderIdBroker(unpacked),
                        ObjectDeserializer.AsQuantity(unpacked[nameof(OrderModified.ModifiedQuantity)]),
                        ObjectDeserializer.AsPrice(unpacked[nameof(OrderModified.ModifiedPrice)]),
                        ObjectDeserializer.AsZonedDateTime(unpacked[nameof(OrderModified.ModifiedTime)]),
                        id,
                        timestamp);
                case nameof(OrderExpired):
                    return new OrderExpired(
                        this.identifierCache.AccountId(unpacked),
                        ObjectDeserializer.AsOrderId(unpacked),
                        ObjectDeserializer.AsZonedDateTime(unpacked[nameof(OrderExpired.ExpiredTime)]),
                        id,
                        timestamp);
                case nameof(OrderPartiallyFilled):
                    return new OrderPartiallyFilled(
                        this.identifierCache.AccountId(unpacked),
                        ObjectDeserializer.AsOrderId(unpacked),
                        ObjectDeserializer.AsExecutionId(unpacked),
                        ObjectDeserializer.AsPositionIdBroker(unpacked),
                        this.identifierCache.Symbol(unpacked),
                        ObjectDeserializer.AsEnum<OrderSide>(unpacked[nameof(OrderPartiallyFilled.OrderSide)]),
                        ObjectDeserializer.AsQuantity(unpacked[nameof(OrderPartiallyFilled.FilledQuantity)]),
                        ObjectDeserializer.AsQuantity(unpacked[nameof(OrderPartiallyFilled.LeavesQuantity)]),
                        ObjectDeserializer.AsPrice(unpacked[nameof(OrderPartiallyFilled.AveragePrice)]),
                        ObjectDeserializer.AsEnum<Currency>(unpacked[nameof(OrderPartiallyFilled.Currency)]),
                        ObjectDeserializer.AsZonedDateTime(unpacked[nameof(OrderPartiallyFilled.ExecutionTime)]),
                        id,
                        timestamp);
                case nameof(OrderFilled):
                    return new OrderFilled(
                        this.identifierCache.AccountId(unpacked),
                        ObjectDeserializer.AsOrderId(unpacked),
                        ObjectDeserializer.AsExecutionId(unpacked),
                        ObjectDeserializer.AsPositionIdBroker(unpacked),
                        this.identifierCache.Symbol(unpacked),
                        ObjectDeserializer.AsEnum<OrderSide>(unpacked[nameof(OrderFilled.OrderSide)]),
                        ObjectDeserializer.AsQuantity(unpacked[nameof(OrderFilled.FilledQuantity)]),
                        ObjectDeserializer.AsPrice(unpacked[nameof(OrderFilled.AveragePrice)]),
                        ObjectDeserializer.AsEnum<Currency>(unpacked[nameof(OrderFilled.Currency)]),
                        ObjectDeserializer.AsZonedDateTime(unpacked[nameof(OrderFilled.ExecutionTime)]),
                        id,
                        timestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(@event, nameof(@event));
            }
        }
    }
}
