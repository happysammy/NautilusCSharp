// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackEventSerializer.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    using System;
    using System.Globalization;
    using MsgPack;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides an events binary serializer for the Message Pack specification.
    /// </summary>
    public class MsgPackEventSerializer : MsgPackSerializer, IEventSerializer
    {
        /// <summary>
        /// Serialize the given event to Message Pack specification bytes.
        /// </summary>
        /// <param name="event">The order to serialize.</param>
        /// <returns>The serialized event.</returns>
        /// <exception cref="InvalidOperationException">Throws if the event cannot be serialized.</exception>
        public byte[] Serialize(Event @event)
        {
            switch (@event)
            {
                case OrderEvent orderEvent:
                    return this.SerializeOrderEvent(orderEvent);
                case AccountEvent accountEvent:
                    return this.SerializeToMsgPack(new MessagePackObjectDictionary
                    {
                        { Key.Event, nameof(AccountEvent) },
                        { Key.AccountId, accountEvent.AccountId.ToString() },
                        { Key.Broker, accountEvent.Broker.ToString() },
                        { Key.AccountNumber, accountEvent.AccountNumber },
                        { Key.Currency, accountEvent.Currency.ToString() },
                        { Key.CashBalance, accountEvent.CashBalance.Value.ToString(CultureInfo.InvariantCulture) },
                        { Key.CashStartDay, accountEvent.CashStartDay.Value.ToString(CultureInfo.InvariantCulture) },
                        { Key.CashActivityDay, accountEvent.CashActivityDay.Value.ToString(CultureInfo.InvariantCulture) },
                        { Key.MarginUsedLiquidation, accountEvent.MarginUsedLiquidation.Value.ToString(CultureInfo.InvariantCulture) },
                        { Key.MarginUsedMaintenance, accountEvent.MarginUsedMaintenance.Value.ToString(CultureInfo.InvariantCulture) },
                        { Key.MarginRatio, accountEvent.MarginRatio.ToString(CultureInfo.InvariantCulture) },
                        { Key.MarginCallStatus, accountEvent.MarginCallStatus.ToString() },
                        { Key.EventId, accountEvent.Id.ToString() },
                        { Key.EventTimestamp, accountEvent.Timestamp.ToIsoString() },
                    });
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(@event, nameof(@event));
            }
        }

        /// <summary>
        /// Deserializes the given Message Pack specification bytes to an event.
        /// </summary>
        /// <param name="eventBytes">The event bytes to deserialize.</param>
        /// <returns>The deserialized event.</returns>
        /// <exception cref="InvalidOperationException">Throws if the event cannot be deserialized.</exception>
        public Event Deserialize(byte[] eventBytes)
        {
            var unpacked = this.DeserializeFromMsgPack<MessagePackObjectDictionary>(eventBytes);

            var eventId = Guid.Parse(unpacked[Key.EventId].ToString());
            var eventTimestamp = unpacked[Key.EventTimestamp].ToString().ToZonedDateTimeFromIso();
            var eventType = unpacked[Key.Event].ToString();

            switch (eventType)
            {
                case nameof(OrderEvent):
                    return this.DeserializeOrderEvent(
                        eventId,
                        eventTimestamp,
                        unpacked);
                case nameof(AccountEvent):
                    var currency = unpacked[Key.Currency].ToString().ToEnum<Currency>();
                    return new AccountEvent(
                        new AccountId(unpacked[Key.AccountId].ToString()),
                        unpacked[Key.Broker].ToString().ToEnum<Brokerage>(),
                        unpacked[Key.AccountNumber].ToString(),
                        currency,
                        Money.Create(Convert.ToDecimal(unpacked[Key.CashBalance].ToString()), currency),
                        Money.Create(Convert.ToDecimal(unpacked[Key.CashStartDay].ToString()), currency),
                        Money.Create(Convert.ToDecimal(unpacked[Key.CashActivityDay].ToString()), currency),
                        Money.Create(Convert.ToDecimal(unpacked[Key.MarginUsedLiquidation].ToString()), currency),
                        Money.Create(Convert.ToDecimal(unpacked[Key.MarginUsedMaintenance].ToString()), currency),
                        Convert.ToDecimal(unpacked[Key.MarginRatio].ToString()),
                        unpacked[Key.MarginCallStatus].ToString(),
                        eventId,
                        eventTimestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(eventType, nameof(eventType));
            }
        }

        private byte[] SerializeOrderEvent(OrderEvent orderEvent)
        {
            var package = new MessagePackObjectDictionary
            {
                { Key.Event, nameof(OrderEvent) },
                { Key.Symbol, orderEvent.Symbol.ToString() },
                { Key.OrderId, orderEvent.OrderId.Value },
                { Key.EventId, orderEvent.Id.ToString() },
                { Key.EventTimestamp, orderEvent.Timestamp.ToIsoString() },
            };

            switch (orderEvent)
            {
                case OrderSubmitted @event:
                    package.Add(nameof(OrderEvent), nameof(OrderSubmitted));
                    package.Add(Key.SubmittedTime, @event.SubmittedTime.ToIsoString());
                    break;
                case OrderAccepted @event:
                    package.Add(nameof(OrderEvent), nameof(OrderAccepted));
                    package.Add(Key.AcceptedTime, @event.AcceptedTime.ToIsoString());
                    break;
                case OrderRejected @event:
                    package.Add(nameof(OrderEvent), nameof(OrderRejected));
                    package.Add(Key.RejectedTime, @event.RejectedTime.ToIsoString());
                    package.Add(Key.RejectedReason, @event.RejectedReason);
                    break;
                case OrderWorking @event:
                    package.Add(nameof(OrderEvent), nameof(OrderWorking));
                    package.Add(Key.BrokerOrderId, @event.OrderIdBroker.ToString());
                    package.Add(Key.Label, @event.Label.ToString());
                    package.Add(Key.OrderSide, @event.OrderSide.ToString());
                    package.Add(Key.OrderType, @event.OrderType.ToString());
                    package.Add(Key.Quantity, @event.Quantity.Value);
                    package.Add(Key.Price, @event.Price.ToString());
                    package.Add(Key.TimeInForce, @event.TimeInForce.ToString());
                    package.Add(Key.ExpireTime, MsgPackSerializationHelper.GetExpireTimeString(@event.ExpireTime));
                    package.Add(Key.WorkingTime, @event.WorkingTime.ToIsoString());
                    break;
                case OrderCancelled @event:
                    package.Add(Key.OrderEvent, nameof(OrderCancelled));
                    package.Add(Key.CancelledTime, @event.CancelledTime.ToIsoString());
                    break;
                case OrderCancelReject @event:
                    package.Add(nameof(OrderEvent), nameof(OrderCancelReject));
                    package.Add(Key.RejectedTime, @event.RejectedTime.ToIsoString());
                    package.Add(Key.RejectedResponse, @event.RejectedResponseTo);
                    package.Add(Key.RejectedReason, @event.RejectedReason);
                    break;
                case OrderModified @event:
                    package.Add(Key.OrderEvent, nameof(OrderModified));
                    package.Add(Key.BrokerOrderId, @event.BrokerOrderId.ToString());
                    package.Add(Key.ModifiedPrice, @event.ModifiedPrice.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(Key.ModifiedTime, @event.ModifiedTime.ToIsoString());
                    break;
                case OrderExpired @event:
                    package.Add(Key.OrderEvent, nameof(OrderExpired));
                    package.Add(Key.ExpiredTime, @event.ExpiredTime.ToIsoString());
                    break;
                case OrderPartiallyFilled @event:
                    package.Add(Key.OrderEvent, nameof(OrderPartiallyFilled));
                    package.Add(Key.ExecutionId, @event.ExecutionId.Value);
                    package.Add(Key.ExecutionTicket, @event.ExecutionTicket.Value);
                    package.Add(Key.OrderSide, @event.OrderSide.ToString());
                    package.Add(Key.FilledQuantity, @event.FilledQuantity.Value);
                    package.Add(Key.LeavesQuantity, @event.LeavesQuantity.Value);
                    package.Add(Key.AveragePrice, @event.AveragePrice.ToString());
                    package.Add(Key.ExecutionTime, @event.ExecutionTime.ToIsoString());
                    break;
                case OrderFilled @event:
                    package.Add(Key.OrderEvent, nameof(OrderFilled));
                    package.Add(Key.ExecutionId, @event.ExecutionId.Value);
                    package.Add(Key.ExecutionTicket, @event.ExecutionTicket.Value);
                    package.Add(Key.OrderSide, @event.OrderSide.ToString());
                    package.Add(Key.FilledQuantity, @event.FilledQuantity.Value);
                    package.Add(Key.AveragePrice, @event.AveragePrice.ToString());
                    package.Add(Key.ExecutionTime, @event.ExecutionTime.ToIsoString());
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(orderEvent, nameof(orderEvent));
            }

            return this.SerializeToMsgPack(package);
        }

        private OrderEvent DeserializeOrderEvent(
            Guid eventId,
            ZonedDateTime eventTimestamp,
            MessagePackObjectDictionary unpacked)
        {
            var symbol = MsgPackSerializationHelper.GetSymbol(unpacked[Key.Symbol].ToString());
            var orderId = new OrderId(unpacked[Key.OrderId].ToString());
            var orderEvent = unpacked[Key.OrderEvent].ToString();

            switch (orderEvent)
            {
                case nameof(OrderSubmitted):
                    return new OrderSubmitted(
                        symbol,
                        orderId,
                        unpacked[Key.SubmittedTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderAccepted):
                    return new OrderAccepted(
                        symbol,
                        orderId,
                        unpacked[Key.AcceptedTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderRejected):
                    return new OrderRejected(
                        symbol,
                        orderId,
                        unpacked[Key.RejectedTime].ToString().ToZonedDateTimeFromIso(),
                        unpacked[Key.RejectedReason].ToString(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderWorking):
                    return new OrderWorking(
                        symbol,
                        orderId,
                        new OrderId(unpacked[Key.BrokerOrderId].ToString()),
                        new Label(unpacked[Key.Label].ToString()),
                        unpacked[Key.OrderSide].ToString().ToEnum<OrderSide>(),
                        unpacked[Key.OrderType].ToString().ToEnum<OrderType>(),
                        Quantity.Create(unpacked[Key.Quantity].AsInt32()),
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.Price].ToString()).Value,
                        unpacked[Key.TimeInForce].ToString().ToEnum<TimeInForce>(),
                        MsgPackSerializationHelper.GetExpireTime(unpacked[Key.ExpireTime].ToString()),
                        unpacked[Key.WorkingTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderCancelled):
                    return new OrderCancelled(
                        symbol,
                        orderId,
                        unpacked[Key.CancelledTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderCancelReject):
                    return new OrderCancelReject(
                        symbol,
                        orderId,
                        unpacked[Key.RejectedTime].ToString().ToZonedDateTimeFromIso(),
                        unpacked[Key.RejectedResponse].ToString(),
                        unpacked[Key.RejectedReason].ToString(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderModified):
                    return new OrderModified(
                        symbol,
                        orderId,
                        new OrderId(unpacked[Key.BrokerOrderId].ToString()),
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.ModifiedPrice].ToString()).Value,
                        unpacked[Key.ModifiedTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderExpired):
                    return new OrderExpired(
                        symbol,
                        orderId,
                        unpacked[Key.ExpiredTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderPartiallyFilled):
                    return new OrderPartiallyFilled(
                        symbol,
                        orderId,
                        new ExecutionId(unpacked[Key.ExecutionId].ToString()),
                        new ExecutionTicket(unpacked[Key.ExecutionTicket].ToString()),
                        unpacked[Key.OrderSide].ToString().ToEnum<OrderSide>(),
                        Quantity.Create(unpacked[Key.FilledQuantity].AsInt32()),
                        Quantity.Create(unpacked[Key.LeavesQuantity].AsInt32()),
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.AveragePrice].ToString()).Value,
                        unpacked[Key.ExecutionTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderFilled):
                    return new OrderFilled(
                        symbol,
                        orderId,
                        new ExecutionId(unpacked[Key.ExecutionId].ToString()),
                        new ExecutionTicket(unpacked[Key.ExecutionTicket].ToString()),
                        unpacked[Key.OrderSide].ToString().ToEnum<OrderSide>(),
                        Quantity.Create(unpacked[Key.FilledQuantity].AsInt32()),
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.AveragePrice].ToString()).Value,
                        unpacked[Key.ExecutionTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(orderEvent, nameof(orderEvent));
            }
        }
    }
}
