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
            var package = new MessagePackObjectDictionary
            {
                { Key.Type, nameof(Event) },
                { Key.EventId, @event.Id.ToString() },
                { Key.EventTimestamp, @event.Timestamp.ToIsoString() },
            };

            switch (@event)
            {
                case AccountEvent accountEvent:
                    package.Add(Key.Event, nameof(AccountEvent));
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
                    package.Add(Key.MarginCallStatus, accountEvent.MarginCallStatus.ToString());
                    break;
                case OrderSubmitted orderEvent:
                    package.Add(Key.Event, nameof(OrderSubmitted));
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.Value);
                    package.Add(Key.SubmittedTime, orderEvent.SubmittedTime.ToIsoString());
                    break;
                case OrderAccepted orderEvent:
                    package.Add(Key.Event, nameof(OrderAccepted));
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.Value);
                    package.Add(Key.AcceptedTime, orderEvent.AcceptedTime.ToIsoString());
                    break;
                case OrderRejected orderEvent:
                    package.Add(Key.Event, nameof(OrderRejected));
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.Value);
                    package.Add(Key.RejectedTime, orderEvent.RejectedTime.ToIsoString());
                    package.Add(Key.RejectedReason, orderEvent.RejectedReason);
                    break;
                case OrderWorking orderEvent:
                    package.Add(Key.Event, nameof(OrderWorking));
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.Value);
                    package.Add(Key.BrokerOrderId, orderEvent.OrderIdBroker.ToString());
                    package.Add(Key.Label, orderEvent.Label.ToString());
                    package.Add(Key.OrderSide, orderEvent.OrderSide.ToString());
                    package.Add(Key.OrderType, orderEvent.OrderType.ToString());
                    package.Add(Key.Quantity, orderEvent.Quantity.Value);
                    package.Add(Key.Price, orderEvent.Price.ToString());
                    package.Add(Key.TimeInForce, orderEvent.TimeInForce.ToString());
                    package.Add(Key.ExpireTime, MsgPackSerializationHelper.GetExpireTimeString(orderEvent.ExpireTime));
                    package.Add(Key.WorkingTime, orderEvent.WorkingTime.ToIsoString());
                    break;
                case OrderCancelled orderEvent:
                    package.Add(Key.Event, nameof(OrderCancelled));
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.Value);
                    package.Add(Key.CancelledTime, orderEvent.CancelledTime.ToIsoString());
                    break;
                case OrderCancelReject orderEvent:
                    package.Add(Key.Event, nameof(OrderCancelReject));
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.Value);
                    package.Add(Key.RejectedTime, orderEvent.RejectedTime.ToIsoString());
                    package.Add(Key.RejectedResponse, orderEvent.RejectedResponseTo);
                    package.Add(Key.RejectedReason, orderEvent.RejectedReason);
                    break;
                case OrderModified orderEvent:
                    package.Add(Key.Event, nameof(OrderModified));
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.Value);
                    package.Add(Key.BrokerOrderId, orderEvent.BrokerOrderId.ToString());
                    package.Add(Key.ModifiedPrice, orderEvent.ModifiedPrice.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(Key.ModifiedTime, orderEvent.ModifiedTime.ToIsoString());
                    break;
                case OrderExpired orderEvent:
                    package.Add(Key.Event, nameof(OrderExpired));
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.Value);
                    package.Add(Key.ExpiredTime, orderEvent.ExpiredTime.ToIsoString());
                    break;
                case OrderPartiallyFilled orderEvent:
                    package.Add(Key.Event, nameof(OrderPartiallyFilled));
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.Value);
                    package.Add(Key.ExecutionId, orderEvent.ExecutionId.Value);
                    package.Add(Key.ExecutionTicket, orderEvent.ExecutionTicket.Value);
                    package.Add(Key.OrderSide, orderEvent.OrderSide.ToString());
                    package.Add(Key.FilledQuantity, orderEvent.FilledQuantity.Value);
                    package.Add(Key.LeavesQuantity, orderEvent.LeavesQuantity.Value);
                    package.Add(Key.AveragePrice, orderEvent.AveragePrice.ToString());
                    package.Add(Key.ExecutionTime, orderEvent.ExecutionTime.ToIsoString());
                    break;
                case OrderFilled orderEvent:
                    package.Add(Key.Event, nameof(OrderFilled));
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.Value);
                    package.Add(Key.ExecutionId, orderEvent.ExecutionId.Value);
                    package.Add(Key.ExecutionTicket, orderEvent.ExecutionTicket.Value);
                    package.Add(Key.OrderSide, orderEvent.OrderSide.ToString());
                    package.Add(Key.FilledQuantity, orderEvent.FilledQuantity.Value);
                    package.Add(Key.AveragePrice, orderEvent.AveragePrice.ToString());
                    package.Add(Key.ExecutionTime, orderEvent.ExecutionTime.ToIsoString());
                    break;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(@event, nameof(@event));
            }

            return SerializeToMsgPack(package);
        }

        /// <summary>
        /// Deserializes the given Message Pack specification bytes to an event.
        /// </summary>
        /// <param name="eventBytes">The event bytes to deserialize.</param>
        /// <returns>The deserialized event.</returns>
        /// <exception cref="InvalidOperationException">Throws if the event cannot be deserialized.</exception>
        public Event Deserialize(byte[] eventBytes)
        {
            var unpacked = DeserializeFromMsgPack<MessagePackObjectDictionary>(eventBytes);

            var eventId = Guid.Parse(unpacked[Key.EventId].ToString());
            var eventTimestamp = unpacked[Key.EventTimestamp].ToString().ToZonedDateTimeFromIso();
            var @event = unpacked[Key.Event].ToString();

            switch (@event)
            {
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
                case nameof(OrderSubmitted):
                    return new OrderSubmitted(
                        MsgPackSerializationHelper.GetSymbol(unpacked[Key.Symbol].ToString()),
                        new OrderId(unpacked[Key.OrderId].ToString()),
                        unpacked[Key.SubmittedTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderAccepted):
                    return new OrderAccepted(
                        MsgPackSerializationHelper.GetSymbol(unpacked[Key.Symbol].ToString()),
                        new OrderId(unpacked[Key.OrderId].ToString()),
                        unpacked[Key.AcceptedTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderRejected):
                    return new OrderRejected(
                        MsgPackSerializationHelper.GetSymbol(unpacked[Key.Symbol].ToString()),
                        new OrderId(unpacked[Key.OrderId].ToString()),
                        unpacked[Key.RejectedTime].ToString().ToZonedDateTimeFromIso(),
                        unpacked[Key.RejectedReason].ToString(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderWorking):
                    return new OrderWorking(
                        MsgPackSerializationHelper.GetSymbol(unpacked[Key.Symbol].ToString()),
                        new OrderId(unpacked[Key.OrderId].ToString()),
                        new OrderId(unpacked[Key.BrokerOrderId].ToString()),
                        new Label(unpacked[Key.Label].ToString()),
                        unpacked[Key.OrderSide].ToString().ToEnum<OrderSide>(),
                        unpacked[Key.OrderType].ToString().ToEnum<OrderType>(),
                        Quantity.Create(unpacked[Key.Quantity].AsInt32()),
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.Price].ToString()),
                        unpacked[Key.TimeInForce].ToString().ToEnum<TimeInForce>(),
                        MsgPackSerializationHelper.GetExpireTime(unpacked[Key.ExpireTime].ToString()),
                        unpacked[Key.WorkingTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderCancelled):
                    return new OrderCancelled(
                        MsgPackSerializationHelper.GetSymbol(unpacked[Key.Symbol].ToString()),
                        new OrderId(unpacked[Key.OrderId].ToString()),
                        unpacked[Key.CancelledTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderCancelReject):
                    return new OrderCancelReject(
                        MsgPackSerializationHelper.GetSymbol(unpacked[Key.Symbol].ToString()),
                        new OrderId(unpacked[Key.OrderId].ToString()),
                        unpacked[Key.RejectedTime].ToString().ToZonedDateTimeFromIso(),
                        unpacked[Key.RejectedResponse].ToString(),
                        unpacked[Key.RejectedReason].ToString(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderModified):
                    return new OrderModified(
                        MsgPackSerializationHelper.GetSymbol(unpacked[Key.Symbol].ToString()),
                        new OrderId(unpacked[Key.OrderId].ToString()),
                        new OrderId(unpacked[Key.BrokerOrderId].ToString()),
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.ModifiedPrice].ToString()),
                        unpacked[Key.ModifiedTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderExpired):
                    return new OrderExpired(
                        MsgPackSerializationHelper.GetSymbol(unpacked[Key.Symbol].ToString()),
                        new OrderId(unpacked[Key.OrderId].ToString()),
                        unpacked[Key.ExpiredTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderPartiallyFilled):
                    return new OrderPartiallyFilled(
                        MsgPackSerializationHelper.GetSymbol(unpacked[Key.Symbol].ToString()),
                        new OrderId(unpacked[Key.OrderId].ToString()),
                        new ExecutionId(unpacked[Key.ExecutionId].ToString()),
                        new ExecutionTicket(unpacked[Key.ExecutionTicket].ToString()),
                        unpacked[Key.OrderSide].ToString().ToEnum<OrderSide>(),
                        Quantity.Create(unpacked[Key.FilledQuantity].AsInt32()),
                        Quantity.Create(unpacked[Key.LeavesQuantity].AsInt32()),
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.AveragePrice].ToString()),
                        unpacked[Key.ExecutionTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderFilled):
                    return new OrderFilled(
                        MsgPackSerializationHelper.GetSymbol(unpacked[Key.Symbol].ToString()),
                        new OrderId(unpacked[Key.OrderId].ToString()),
                        new ExecutionId(unpacked[Key.ExecutionId].ToString()),
                        new ExecutionTicket(unpacked[Key.ExecutionTicket].ToString()),
                        unpacked[Key.OrderSide].ToString().ToEnum<OrderSide>(),
                        Quantity.Create(unpacked[Key.FilledQuantity].AsInt32()),
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.AveragePrice].ToString()),
                        unpacked[Key.ExecutionTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);

                default:
                    throw ExceptionFactory.InvalidSwitchArgument(@event, nameof(@event));
            }
        }
    }
}
