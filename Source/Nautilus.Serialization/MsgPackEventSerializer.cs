// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackEventSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System.Globalization;
    using MsgPack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.Serialization.Internal;

    /// <summary>
    /// Provides an <see cref="Event"/> message binary serializer for the MessagePack specification.
    /// </summary>
    public class MsgPackEventSerializer : IEventSerializer
    {
        /// <inheritdoc />
        public byte[] Serialize(Event @event)
        {
            var package = new MessagePackObjectDictionary
            {
                { Key.Type, nameof(Event) },
                { Key.Event, @event.Type.Name },
                { Key.EventId, @event.Id.ToString() },
                { Key.EventTimestamp, @event.Timestamp.ToIsoString() },
            };

            switch (@event)
            {
                case AccountEvent accountEvent:
                    package.Add(Key.AccountId, accountEvent.AccountId.ToString());
                    package.Add(Key.Broker, accountEvent.Broker.ToString());
                    package.Add(Key.AccountNumber, accountEvent.AccountNumber);
                    package.Add(Key.Currency, accountEvent.Currency.ToString());
                    package.Add(Key.CashBalance, accountEvent.CashBalance.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(Key.CashStartDay, accountEvent.CashStartDay.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(Key.CashActivityDay, accountEvent.CashActivityDay.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(Key.MarginUsedLiquidation, accountEvent.MarginUsedLiquidation.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(Key.MarginUsedMaintenance, accountEvent.MarginUsedMaintenance.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(Key.MarginRatio, accountEvent.MarginRatio.ToString(CultureInfo.InvariantCulture));
                    package.Add(Key.MarginCallStatus, accountEvent.MarginCallStatus);
                    break;
                case OrderInitialized orderEvent:
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.Label, orderEvent.Label.ToString());
                    package.Add(Key.OrderSide, orderEvent.OrderSide.ToString());
                    package.Add(Key.OrderType, orderEvent.OrderType.ToString());
                    package.Add(Key.Quantity, orderEvent.Quantity.Value);
                    package.Add(Key.Price, ObjectPacker.Pack(orderEvent.Price));
                    package.Add(Key.TimeInForce, orderEvent.TimeInForce.ToString());
                    package.Add(Key.ExpireTime, ObjectPacker.Pack(orderEvent.ExpireTime));
                    break;
                case OrderSubmitted orderEvent:
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.SubmittedTime, orderEvent.SubmittedTime.ToIsoString());
                    break;
                case OrderAccepted orderEvent:
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.AcceptedTime, orderEvent.AcceptedTime.ToIsoString());
                    break;
                case OrderRejected orderEvent:
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.RejectedTime, orderEvent.RejectedTime.ToIsoString());
                    package.Add(Key.RejectedReason, orderEvent.RejectedReason);
                    break;
                case OrderWorking orderEvent:
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.OrderIdBroker, orderEvent.OrderIdBroker.ToString());
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.Label, orderEvent.Label.ToString());
                    package.Add(Key.OrderSide, orderEvent.OrderSide.ToString());
                    package.Add(Key.OrderType, orderEvent.OrderType.ToString());
                    package.Add(Key.Quantity, orderEvent.Quantity.Value);
                    package.Add(Key.Price, orderEvent.Price.ToString());
                    package.Add(Key.TimeInForce, orderEvent.TimeInForce.ToString());
                    package.Add(Key.ExpireTime, ObjectPacker.Pack(orderEvent.ExpireTime));
                    package.Add(Key.WorkingTime, orderEvent.WorkingTime.ToIsoString());
                    break;
                case OrderCancelled orderEvent:
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.CancelledTime, orderEvent.CancelledTime.ToIsoString());
                    break;
                case OrderCancelReject orderEvent:
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.RejectedTime, orderEvent.RejectedTime.ToIsoString());
                    package.Add(Key.RejectedResponse, orderEvent.RejectedResponseTo);
                    package.Add(Key.RejectedReason, orderEvent.RejectedReason);
                    break;
                case OrderModified orderEvent:
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.OrderIdBroker, orderEvent.OrderIdBroker.ToString());
                    package.Add(Key.ModifiedPrice, orderEvent.ModifiedPrice.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(Key.ModifiedTime, orderEvent.ModifiedTime.ToIsoString());
                    break;
                case OrderExpired orderEvent:
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.ExpiredTime, orderEvent.ExpiredTime.ToIsoString());
                    break;
                case OrderPartiallyFilled orderEvent:
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.ExecutionId, orderEvent.ExecutionId.ToString());
                    package.Add(Key.ExecutionTicket, orderEvent.ExecutionTicket.ToString());
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderSide, orderEvent.OrderSide.ToString());
                    package.Add(Key.FilledQuantity, orderEvent.FilledQuantity.Value);
                    package.Add(Key.LeavesQuantity, orderEvent.LeavesQuantity.Value);
                    package.Add(Key.AveragePrice, orderEvent.AveragePrice.ToString());
                    package.Add(Key.ExecutionTime, orderEvent.ExecutionTime.ToIsoString());
                    break;
                case OrderFilled orderEvent:
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.ExecutionId, orderEvent.ExecutionId.ToString());
                    package.Add(Key.ExecutionTicket, orderEvent.ExecutionTicket.ToString());
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderSide, orderEvent.Side.ToString());
                    package.Add(Key.FilledQuantity, orderEvent.FilledQuantity.Value);
                    package.Add(Key.AveragePrice, orderEvent.AveragePrice.ToString());
                    package.Add(Key.ExecutionTime, orderEvent.ExecutionTime.ToIsoString());
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

            var @event = unpacked[Key.Event].ToString();
            var eventId = ObjectExtractor.Guid(unpacked[Key.EventId]);
            var eventTimestamp = ObjectExtractor.ZonedDateTime(unpacked[Key.EventTimestamp]);

            switch (@event)
            {
                case nameof(AccountEvent):
                    var currency = ObjectExtractor.Enum<Currency>(unpacked[Key.Currency]);
                    return new AccountEvent(
                        new AccountId(unpacked[Key.AccountId].ToString()),
                        ObjectExtractor.Enum<Brokerage>(unpacked[Key.Broker]),
                        unpacked[Key.AccountNumber].ToString(),
                        currency,
                        ObjectExtractor.Money(unpacked[Key.CashBalance], currency),
                        ObjectExtractor.Money(unpacked[Key.CashStartDay], currency),
                        ObjectExtractor.Money(unpacked[Key.CashActivityDay], currency),
                        ObjectExtractor.Money(unpacked[Key.MarginUsedLiquidation], currency),
                        ObjectExtractor.Money(unpacked[Key.MarginUsedMaintenance], currency),
                        ObjectExtractor.Decimal(unpacked[Key.MarginRatio].ToString()),
                        unpacked[Key.MarginCallStatus].ToString(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderInitialized):
                    return new OrderInitialized(
                        ObjectExtractor.OrderId(unpacked[Key.OrderId]),
                        ObjectExtractor.Symbol(unpacked[Key.Symbol]),
                        ObjectExtractor.Label(unpacked[Key.Label]),
                        ObjectExtractor.Enum<OrderSide>(unpacked[Key.OrderSide]),
                        ObjectExtractor.Enum<OrderType>(unpacked[Key.OrderType]),
                        ObjectExtractor.Quantity(unpacked[Key.Quantity]),
                        ObjectExtractor.NullablePrice(unpacked[Key.Price]),
                        ObjectExtractor.Enum<TimeInForce>(unpacked[Key.TimeInForce]),
                        ObjectExtractor.NullableZonedDateTime(unpacked[Key.ExpireTime]),
                        eventId,
                        eventTimestamp);
                case nameof(OrderSubmitted):
                    return new OrderSubmitted(
                        ObjectExtractor.OrderId(unpacked[Key.OrderId]),
                        ObjectExtractor.ZonedDateTime(unpacked[Key.SubmittedTime]),
                        eventId,
                        eventTimestamp);
                case nameof(OrderAccepted):
                    return new OrderAccepted(
                        ObjectExtractor.OrderId(unpacked[Key.OrderId]),
                        ObjectExtractor.ZonedDateTime(unpacked[Key.AcceptedTime]),
                        eventId,
                        eventTimestamp);
                case nameof(OrderRejected):
                    return new OrderRejected(
                        ObjectExtractor.OrderId(unpacked[Key.OrderId]),
                        ObjectExtractor.ZonedDateTime(unpacked[Key.RejectedTime]),
                        unpacked[Key.RejectedReason].ToString(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderWorking):
                    return new OrderWorking(
                        ObjectExtractor.OrderId(unpacked[Key.OrderId]),
                        ObjectExtractor.OrderId(unpacked[Key.OrderIdBroker]),
                        ObjectExtractor.Symbol(unpacked[Key.Symbol]),
                        ObjectExtractor.Label(unpacked[Key.Label]),
                        ObjectExtractor.Enum<OrderSide>(unpacked[Key.OrderSide]),
                        ObjectExtractor.Enum<OrderType>(unpacked[Key.OrderType]),
                        ObjectExtractor.Quantity(unpacked[Key.Quantity]),
                        ObjectExtractor.Price(unpacked[Key.Price]),
                        ObjectExtractor.Enum<TimeInForce>(unpacked[Key.TimeInForce]),
                        ObjectExtractor.NullableZonedDateTime(unpacked[Key.ExpireTime]),
                        ObjectExtractor.ZonedDateTime(unpacked[Key.WorkingTime]),
                        eventId,
                        eventTimestamp);
                case nameof(OrderCancelled):
                    return new OrderCancelled(
                        ObjectExtractor.OrderId(unpacked[Key.OrderId]),
                        ObjectExtractor.ZonedDateTime(unpacked[Key.CancelledTime]),
                        eventId,
                        eventTimestamp);
                case nameof(OrderCancelReject):
                    return new OrderCancelReject(
                        ObjectExtractor.OrderId(unpacked[Key.OrderId]),
                        ObjectExtractor.ZonedDateTime(unpacked[Key.RejectedTime]),
                        unpacked[Key.RejectedResponse].ToString(),
                        unpacked[Key.RejectedReason].ToString(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderModified):
                    return new OrderModified(
                        ObjectExtractor.OrderId(unpacked[Key.OrderId]),
                        ObjectExtractor.OrderId(unpacked[Key.OrderId]),
                        ObjectExtractor.Price(unpacked[Key.ModifiedPrice]),
                        ObjectExtractor.ZonedDateTime(unpacked[Key.ModifiedTime]),
                        eventId,
                        eventTimestamp);
                case nameof(OrderExpired):
                    return new OrderExpired(
                        ObjectExtractor.OrderId(unpacked[Key.OrderId]),
                        ObjectExtractor.ZonedDateTime(unpacked[Key.ExpiredTime]),
                        eventId,
                        eventTimestamp);
                case nameof(OrderPartiallyFilled):
                    return new OrderPartiallyFilled(
                        ObjectExtractor.OrderId(unpacked[Key.OrderId]),
                        ObjectExtractor.ExecutionId(unpacked[Key.ExecutionId]),
                        ObjectExtractor.ExecutionTicket(unpacked[Key.ExecutionId]),
                        ObjectExtractor.Symbol(unpacked[Key.Symbol]),
                        ObjectExtractor.Enum<OrderSide>(unpacked[Key.OrderSide]),
                        ObjectExtractor.Quantity(unpacked[Key.FilledQuantity]),
                        ObjectExtractor.Quantity(unpacked[Key.LeavesQuantity]),
                        ObjectExtractor.Price(unpacked[Key.AveragePrice]),
                        ObjectExtractor.ZonedDateTime(unpacked[Key.ExecutionTime]),
                        eventId,
                        eventTimestamp);
                case nameof(OrderFilled):
                    return new OrderFilled(
                        ObjectExtractor.OrderId(unpacked[Key.OrderId]),
                        ObjectExtractor.ExecutionId(unpacked[Key.ExecutionId]),
                        ObjectExtractor.ExecutionTicket(unpacked[Key.ExecutionId]),
                        ObjectExtractor.Symbol(unpacked[Key.Symbol]),
                        ObjectExtractor.Enum<OrderSide>(unpacked[Key.OrderSide]),
                        ObjectExtractor.Quantity(unpacked[Key.FilledQuantity]),
                        ObjectExtractor.Price(unpacked[Key.AveragePrice]),
                        ObjectExtractor.ZonedDateTime(unpacked[Key.ExecutionTime]),
                        eventId,
                        eventTimestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(@event, nameof(@event));
            }
        }
    }
}
