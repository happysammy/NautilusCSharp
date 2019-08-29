// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackEventSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System.Globalization;
    using MsgPack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Message;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Serialization.Internal;

    /// <summary>
    /// Provides an <see cref="Event"/> message binary serializer for the MessagePack specification.
    /// </summary>
    public sealed class MsgPackEventSerializer : ISerializer<Event>
    {
        /// <inheritdoc />
        public byte[] Serialize(Event @event)
        {
            var package = new MessagePackObjectDictionary
            {
                { nameof(Event.Type), @event.Type.Name },
                { nameof(Event.Id), @event.Id.ToString() },
                { nameof(Event.Timestamp), @event.Timestamp.ToIsoString() },
            };

            switch (@event)
            {
                case AccountStateEvent evt:
                    package.Add(nameof(evt.AccountId), evt.AccountId.ToString());
                    package.Add(nameof(evt.Currency), evt.Currency.ToString());
                    package.Add(nameof(evt.CashBalance), evt.CashBalance.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(nameof(evt.CashStartDay), evt.CashStartDay.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(nameof(evt.CashActivityDay), evt.CashActivityDay.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(nameof(evt.MarginUsedLiquidation), evt.MarginUsedLiquidation.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(nameof(evt.MarginUsedMaintenance), evt.MarginUsedMaintenance.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(nameof(evt.MarginRatio), evt.MarginRatio.ToString(CultureInfo.InvariantCulture));
                    package.Add(nameof(evt.MarginCallStatus), evt.MarginCallStatus);
                    break;
                case OrderInitialized evt:
                    package.Add(nameof(evt.OrderId), evt.OrderId.ToString());
                    package.Add(nameof(evt.Symbol), evt.Symbol.ToString());
                    package.Add(nameof(evt.Label), evt.Label.ToString());
                    package.Add(nameof(evt.OrderSide), evt.OrderSide.ToString());
                    package.Add(nameof(evt.OrderType), evt.OrderType.ToString());
                    package.Add(nameof(evt.Quantity), evt.Quantity.Value);
                    package.Add(nameof(evt.Price), ObjectPacker.Pack(evt.Price));
                    package.Add(nameof(evt.TimeInForce), evt.TimeInForce.ToString());
                    package.Add(nameof(evt.ExpireTime), ObjectPacker.Pack(evt.ExpireTime));
                    break;
                case OrderSubmitted evt:
                    package.Add(nameof(evt.OrderId), evt.OrderId.ToString());
                    package.Add(nameof(evt.AccountId), evt.AccountId.ToString());
                    package.Add(nameof(evt.SubmittedTime), evt.SubmittedTime.ToIsoString());
                    break;
                case OrderAccepted evt:
                    package.Add(nameof(evt.OrderId), evt.OrderId.ToString());
                    package.Add(nameof(evt.AccountId), evt.AccountId.ToString());
                    package.Add(nameof(evt.AcceptedTime), evt.AcceptedTime.ToIsoString());
                    break;
                case OrderRejected evt:
                    package.Add(nameof(evt.OrderId), evt.OrderId.ToString());
                    package.Add(nameof(evt.AccountId), evt.AccountId.ToString());
                    package.Add(nameof(evt.RejectedTime), evt.RejectedTime.ToIsoString());
                    package.Add(nameof(evt.RejectedReason), evt.RejectedReason);
                    break;
                case OrderWorking evt:
                    package.Add(nameof(evt.OrderId), evt.OrderId.ToString());
                    package.Add(nameof(evt.OrderIdBroker), evt.OrderIdBroker.ToString());
                    package.Add(nameof(evt.AccountId), evt.AccountId.ToString());
                    package.Add(nameof(evt.Symbol), evt.Symbol.ToString());
                    package.Add(nameof(evt.Label), evt.Label.ToString());
                    package.Add(nameof(evt.OrderSide), evt.OrderSide.ToString());
                    package.Add(nameof(evt.OrderType), evt.OrderType.ToString());
                    package.Add(nameof(evt.Quantity), evt.Quantity.Value);
                    package.Add(nameof(evt.Price), evt.Price.ToString());
                    package.Add(nameof(evt.TimeInForce), evt.TimeInForce.ToString());
                    package.Add(nameof(evt.ExpireTime), ObjectPacker.Pack(evt.ExpireTime));
                    package.Add(nameof(evt.WorkingTime), evt.WorkingTime.ToIsoString());
                    break;
                case OrderCancelled evt:
                    package.Add(nameof(evt.OrderId), evt.OrderId.ToString());
                    package.Add(nameof(evt.AccountId), evt.AccountId.ToString());
                    package.Add(nameof(evt.CancelledTime), evt.CancelledTime.ToIsoString());
                    break;
                case OrderCancelReject evt:
                    package.Add(nameof(evt.OrderId), evt.OrderId.ToString());
                    package.Add(nameof(evt.AccountId), evt.AccountId.ToString());
                    package.Add(nameof(evt.RejectedTime), evt.RejectedTime.ToIsoString());
                    package.Add(nameof(evt.RejectedResponseTo), evt.RejectedResponseTo);
                    package.Add(nameof(evt.RejectedReason), evt.RejectedReason);
                    break;
                case OrderModified evt:
                    package.Add(nameof(evt.OrderId), evt.OrderId.ToString());
                    package.Add(nameof(evt.OrderIdBroker), evt.OrderIdBroker.ToString());
                    package.Add(nameof(evt.AccountId), evt.AccountId.ToString());
                    package.Add(nameof(evt.ModifiedPrice), evt.ModifiedPrice.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(nameof(evt.ModifiedTime), evt.ModifiedTime.ToIsoString());
                    break;
                case OrderExpired evt:
                    package.Add(nameof(evt.OrderId), evt.OrderId.ToString());
                    package.Add(nameof(evt.AccountId), evt.AccountId.ToString());
                    package.Add(nameof(evt.ExpiredTime), evt.ExpiredTime.ToIsoString());
                    break;
                case OrderPartiallyFilled evt:
                    package.Add(nameof(evt.OrderId), evt.OrderId.ToString());
                    package.Add(nameof(evt.AccountId), evt.AccountId.ToString());
                    package.Add(nameof(evt.ExecutionId), evt.ExecutionId.ToString());
                    package.Add(nameof(evt.ExecutionTicket), evt.ExecutionTicket.ToString());
                    package.Add(nameof(evt.Symbol), evt.Symbol.ToString());
                    package.Add(nameof(evt.OrderSide), evt.OrderSide.ToString());
                    package.Add(nameof(evt.FilledQuantity), evt.FilledQuantity.Value);
                    package.Add(nameof(evt.LeavesQuantity), evt.LeavesQuantity.Value);
                    package.Add(nameof(evt.AveragePrice), evt.AveragePrice.ToString());
                    package.Add(nameof(evt.ExecutionTime), evt.ExecutionTime.ToIsoString());
                    break;
                case OrderFilled evt:
                    package.Add(nameof(evt.OrderId), evt.OrderId.ToString());
                    package.Add(nameof(evt.AccountId), evt.AccountId.ToString());
                    package.Add(nameof(evt.ExecutionId), evt.ExecutionId.ToString());
                    package.Add(nameof(evt.ExecutionTicket), evt.ExecutionTicket.ToString());
                    package.Add(nameof(evt.Symbol), evt.Symbol.ToString());
                    package.Add(nameof(evt.OrderSide), evt.OrderSide.ToString());
                    package.Add(nameof(evt.FilledQuantity), evt.FilledQuantity.Value);
                    package.Add(nameof(evt.AveragePrice), evt.AveragePrice.ToString());
                    package.Add(nameof(evt.ExecutionTime), evt.ExecutionTime.ToIsoString());
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(@event, nameof(@event));
            }

            return MsgPackSerializer.Serialize(package);
        }

        /// <inheritdoc />
        public Event Deserialize(byte[] serializedEvent)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(serializedEvent);

            var @event = unpacked[nameof(Event.Type)].ToString();
            var id = ObjectExtractor.Guid(unpacked[nameof(Event.Id)]);
            var timestamp = ObjectExtractor.ZonedDateTime(unpacked[nameof(Event.Timestamp)]);

            switch (@event)
            {
                case nameof(AccountStateEvent):
                    var currency = ObjectExtractor.Enum<Currency>(unpacked[nameof(AccountStateEvent.Currency)]);
                    return new AccountStateEvent(
                        AccountId.FromString(unpacked[nameof(AccountStateEvent.AccountId)].AsString()),
                        currency,
                        ObjectExtractor.Money(unpacked[nameof(AccountStateEvent.CashBalance)], currency),
                        ObjectExtractor.Money(unpacked[nameof(AccountStateEvent.CashStartDay)], currency),
                        ObjectExtractor.Money(unpacked[nameof(AccountStateEvent.CashActivityDay)], currency),
                        ObjectExtractor.Money(unpacked[nameof(AccountStateEvent.MarginUsedLiquidation)], currency),
                        ObjectExtractor.Money(unpacked[nameof(AccountStateEvent.MarginUsedMaintenance)], currency),
                        ObjectExtractor.Decimal(unpacked[nameof(AccountStateEvent.MarginRatio)].ToString()),
                        unpacked[nameof(AccountStateEvent.MarginCallStatus)].AsString(),
                        id,
                        timestamp);
                case nameof(OrderInitialized):
                    return new OrderInitialized(
                        ObjectExtractor.OrderId(unpacked[nameof(OrderInitialized.OrderId)]),
                        ObjectExtractor.Symbol(unpacked),
                        ObjectExtractor.Label(unpacked),
                        ObjectExtractor.Enum<OrderSide>(unpacked[nameof(OrderInitialized.OrderSide)]),
                        ObjectExtractor.Enum<OrderType>(unpacked[nameof(OrderInitialized.OrderType)]),
                        ObjectExtractor.Quantity(unpacked[nameof(OrderInitialized.Quantity)]),
                        ObjectExtractor.NullablePrice(unpacked[nameof(OrderInitialized.Price)]),
                        ObjectExtractor.Enum<TimeInForce>(unpacked[nameof(OrderInitialized.TimeInForce)]),
                        ObjectExtractor.NullableZonedDateTime(unpacked[nameof(OrderInitialized.ExpireTime)]),
                        id,
                        timestamp);
                case nameof(OrderSubmitted):
                    return new OrderSubmitted(
                        ObjectExtractor.OrderId(unpacked[nameof(OrderSubmitted.OrderId)]),
                        ObjectExtractor.AccountId(unpacked[nameof(OrderSubmitted.AccountId)]),
                        ObjectExtractor.ZonedDateTime(unpacked[nameof(OrderSubmitted.SubmittedTime)]),
                        id,
                        timestamp);
                case nameof(OrderAccepted):
                    return new OrderAccepted(
                        ObjectExtractor.OrderId(unpacked[nameof(OrderAccepted.OrderId)]),
                        ObjectExtractor.AccountId(unpacked[nameof(OrderAccepted.AccountId)]),
                        ObjectExtractor.ZonedDateTime(unpacked[nameof(OrderAccepted.AcceptedTime)]),
                        id,
                        timestamp);
                case nameof(OrderRejected):
                    return new OrderRejected(
                        ObjectExtractor.OrderId(unpacked[nameof(OrderRejected.OrderId)]),
                        ObjectExtractor.AccountId(unpacked[nameof(OrderRejected.AccountId)]),
                        ObjectExtractor.ZonedDateTime(unpacked[nameof(OrderRejected.RejectedTime)]),
                        unpacked[nameof(OrderRejected.RejectedReason)].ToString(),
                        id,
                        timestamp);
                case nameof(OrderWorking):
                    return new OrderWorking(
                        ObjectExtractor.OrderId(unpacked[nameof(OrderWorking.OrderId)]),
                        ObjectExtractor.OrderId(unpacked[nameof(OrderWorking.OrderIdBroker)]),
                        ObjectExtractor.AccountId(unpacked[nameof(OrderWorking.AccountId)]),
                        ObjectExtractor.Symbol(unpacked),
                        ObjectExtractor.Label(unpacked),
                        ObjectExtractor.Enum<OrderSide>(unpacked[nameof(OrderWorking.OrderSide)]),
                        ObjectExtractor.Enum<OrderType>(unpacked[nameof(OrderWorking.OrderType)]),
                        ObjectExtractor.Quantity(unpacked[nameof(OrderWorking.Quantity)]),
                        ObjectExtractor.Price(unpacked[nameof(OrderWorking.Price)]),
                        ObjectExtractor.Enum<TimeInForce>(unpacked[nameof(OrderWorking.TimeInForce)]),
                        ObjectExtractor.NullableZonedDateTime(unpacked[nameof(OrderWorking.ExpireTime)]),
                        ObjectExtractor.ZonedDateTime(unpacked[nameof(OrderWorking.WorkingTime)]),
                        id,
                        timestamp);
                case nameof(OrderCancelled):
                    return new OrderCancelled(
                        ObjectExtractor.OrderId(unpacked[nameof(OrderCancelled.OrderId)]),
                        ObjectExtractor.AccountId(unpacked[nameof(OrderCancelled.AccountId)]),
                        ObjectExtractor.ZonedDateTime(unpacked[nameof(OrderCancelled.CancelledTime)]),
                        id,
                        timestamp);
                case nameof(OrderCancelReject):
                    return new OrderCancelReject(
                        ObjectExtractor.OrderId(unpacked[nameof(OrderCancelReject.OrderId)]),
                        ObjectExtractor.AccountId(unpacked[nameof(OrderCancelReject.AccountId)]),
                        ObjectExtractor.ZonedDateTime(unpacked[nameof(OrderCancelReject.RejectedTime)]),
                        unpacked[nameof(OrderCancelReject.RejectedResponseTo)].ToString(),
                        unpacked[nameof(OrderCancelReject.RejectedReason)].ToString(),
                        id,
                        timestamp);
                case nameof(OrderModified):
                    return new OrderModified(
                        ObjectExtractor.OrderId(unpacked[nameof(OrderModified.OrderId)]),
                        ObjectExtractor.OrderId(unpacked[nameof(OrderModified.OrderIdBroker)]),
                        ObjectExtractor.AccountId(unpacked[nameof(OrderModified.AccountId)]),
                        ObjectExtractor.Price(unpacked[nameof(OrderModified.ModifiedPrice)]),
                        ObjectExtractor.ZonedDateTime(unpacked[nameof(OrderModified.ModifiedTime)]),
                        id,
                        timestamp);
                case nameof(OrderExpired):
                    return new OrderExpired(
                        ObjectExtractor.OrderId(unpacked[nameof(OrderExpired.OrderId)]),
                        ObjectExtractor.AccountId(unpacked[nameof(OrderExpired.AccountId)]),
                        ObjectExtractor.ZonedDateTime(unpacked[nameof(OrderExpired.ExpiredTime)]),
                        id,
                        timestamp);
                case nameof(OrderPartiallyFilled):
                    return new OrderPartiallyFilled(
                        ObjectExtractor.OrderId(unpacked[nameof(OrderPartiallyFilled.OrderId)]),
                        ObjectExtractor.AccountId(unpacked[nameof(OrderPartiallyFilled.AccountId)]),
                        ObjectExtractor.ExecutionId(unpacked),
                        ObjectExtractor.ExecutionTicket(unpacked),
                        ObjectExtractor.Symbol(unpacked),
                        ObjectExtractor.Enum<OrderSide>(unpacked[nameof(OrderPartiallyFilled.OrderSide)]),
                        ObjectExtractor.Quantity(unpacked[nameof(OrderPartiallyFilled.FilledQuantity)]),
                        ObjectExtractor.Quantity(unpacked[nameof(OrderPartiallyFilled.LeavesQuantity)]),
                        ObjectExtractor.Price(unpacked[nameof(OrderPartiallyFilled.AveragePrice)]),
                        ObjectExtractor.ZonedDateTime(unpacked[nameof(OrderPartiallyFilled.ExecutionTime)]),
                        id,
                        timestamp);
                case nameof(OrderFilled):
                    return new OrderFilled(
                        ObjectExtractor.OrderId(unpacked[nameof(OrderFilled.OrderId)]),
                        ObjectExtractor.AccountId(unpacked[nameof(OrderFilled.AccountId)]),
                        ObjectExtractor.ExecutionId(unpacked),
                        ObjectExtractor.ExecutionTicket(unpacked),
                        ObjectExtractor.Symbol(unpacked),
                        ObjectExtractor.Enum<OrderSide>(unpacked[nameof(OrderFilled.OrderSide)]),
                        ObjectExtractor.Quantity(unpacked[nameof(OrderFilled.FilledQuantity)]),
                        ObjectExtractor.Price(unpacked[nameof(OrderFilled.AveragePrice)]),
                        ObjectExtractor.ZonedDateTime(unpacked[nameof(OrderFilled.ExecutionTime)]),
                        id,
                        timestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(@event, nameof(@event));
            }
        }
    }
}
