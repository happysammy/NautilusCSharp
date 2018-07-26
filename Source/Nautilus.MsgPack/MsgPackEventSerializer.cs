// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackEventSerializer.cs" company="Nautech Systems Pty Ltd.">
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
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Events;
    using Nautilus.Core;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using global::MsgPack;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Provides a serializer for the Message Pack specification.
    /// </summary>
    public class MsgPackEventSerializer : IEventSerializer
    {
        private const string AccountEvent = "account_event";
        private const string OrderEvent = "order_event";
        private const string OrderSubmitted = "order_submitted";
        private const string OrderAccepted = "order_accepted";
        private const string OrderRejected = "order_rejected";
        private const string OrderCancelled = "order_cancelled";
        private const string OrderCancelReject = "order_cancel_reject";
        private const string OrderWorking = "order_working";
        private const string OrderModified = "order_modified";
        private const string OrderExpired = "order_expired";
        private const string OrderPartiallyFilled = "order_partially_filled";
        private const string OrderFilled = "order_filled";

        /// <summary>
        /// Serialize the given order event to a byte array.
        /// </summary>
        /// <param name="event">The order event to serialize.</param>
        /// <returns>The serialized order event.</returns>
        /// <exception cref="InvalidOperationException">Throws if the event cannot be serialized.</exception>
        public byte[] Serialize(Event @event)
        {
            Debug.NotNull(@event, nameof(@event));

            switch (@event)
            {
                case OrderEvent orderEvent:
                    return SerializeOrderEvent(orderEvent);

                default: throw new InvalidOperationException(
                    "Cannot serialize the event (unrecognized event).");
            }
        }

        /// <summary>
        /// Deserializes the given byte array to an order event.
        /// </summary>
        /// <param name="bytes">The order event to deserialize.</param>
        /// <returns>The deserialized order event.</returns>
        /// <exception cref="InvalidOperationException">Throws if the event cannot be deserialized.</exception>
        public Event Deserialize(byte[] bytes)
        {
            Debug.NotNull(bytes, nameof(bytes));

            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(bytes);

            switch (unpacked[Key.EventType].ToString())
            {
                case OrderEvent:
                    return DeserializeOrderEvent(unpacked);

                default: throw new InvalidOperationException(
                    "Cannot deserialize the event (unrecognized byte[] pattern).");
            }
        }

        private static byte[] SerializeOrderEvent(OrderEvent orderEvent)
        {
            var package = new MessagePackObjectDictionary
            {
                {new MessagePackObject(Key.EventType), OrderEvent},
                {new MessagePackObject(Key.Symbol), orderEvent.Symbol.ToString()},
                {new MessagePackObject(Key.OrderId), orderEvent.OrderId.Value},
                {new MessagePackObject(Key.EventId), orderEvent.Id.ToString()},
                {new MessagePackObject(Key.EventTimestamp), orderEvent.Timestamp.ToIsoString()}
            };

            switch (orderEvent)
            {
                case OrderSubmitted @event:
                    package.Add(new MessagePackObject(Key.OrderEvent), OrderSubmitted);
                    package.Add(new MessagePackObject(Key.SubmittedTime), @event.SubmittedTime.ToIsoString());
                    break;

                case OrderAccepted @event:
                    package.Add(new MessagePackObject(Key.OrderEvent), OrderAccepted);
                    package.Add(new MessagePackObject(Key.AcceptedTime), @event.AcceptedTime.ToIsoString());
                    break;

                case OrderRejected @event:
                    package.Add(new MessagePackObject(Key.OrderEvent), OrderRejected);
                    package.Add(new MessagePackObject(Key.RejectedTime), @event.RejectedTime.ToIsoString());
                    package.Add(new MessagePackObject(Key.RejectedReason), @event.RejectedReason);
                    break;

                case OrderWorking @event:
                    package.Add(new MessagePackObject(Key.OrderEvent), OrderWorking);
                    package.Add(new MessagePackObject(Key.OrderIdBroker), @event.OrderIdBroker.ToString());
                    package.Add(new MessagePackObject(Key.Label), @event.Label.ToString());
                    package.Add(new MessagePackObject(Key.OrderSide), @event.OrderSide.ToString());
                    package.Add(new MessagePackObject(Key.OrderType), @event.OrderType.ToString());
                    package.Add(new MessagePackObject(Key.Quantity), @event.Quantity.Value);
                    package.Add(new MessagePackObject(Key.Price), @event.Price.ToString());
                    package.Add(new MessagePackObject(Key.TimeInForce), @event.TimeInForce.ToString());
                    package.Add(new MessagePackObject(Key.ExpireTime), MsgPackSerializationHelper.GetExpireTimeString(@event.ExpireTime));
                    package.Add(new MessagePackObject(Key.WorkingTime), @event.WorkingTime.ToIsoString());
                    break;

                case OrderCancelled @event:
                    package.Add(new MessagePackObject(Key.OrderEvent), OrderCancelled);
                    package.Add(new MessagePackObject(Key.CancelledTime), @event.CancelledTime.ToIsoString());
                    break;

                case OrderCancelReject @event:
                    package.Add(new MessagePackObject(Key.OrderEvent), OrderCancelReject);
                    package.Add(new MessagePackObject(Key.RejectedTime), @event.RejectedTime.ToIsoString());
                    package.Add(new MessagePackObject(Key.RejectedResponse), @event.RejectedResponseTo);
                    package.Add(new MessagePackObject(Key.RejectedReason), @event.RejectedReason);
                    break;

                case OrderModified @event:
                    package.Add(new MessagePackObject(Key.OrderEvent), OrderModified);
                    package.Add(new MessagePackObject(Key.OrderIdBroker), @event.BrokerOrderId.ToString());
                    package.Add(new MessagePackObject(Key.ModifiedPrice), @event.ModifiedPrice.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(new MessagePackObject(Key.ModifiedTime), @event.ModifiedTime.ToIsoString());
                    break;

                case OrderExpired @event:
                    package.Add(new MessagePackObject(Key.OrderEvent), OrderExpired);
                    package.Add(new MessagePackObject(Key.ExpiredTime), @event.ExpiredTime.ToIsoString());
                    break;

                case OrderPartiallyFilled @event:
                    package.Add(new MessagePackObject(Key.OrderEvent), OrderPartiallyFilled);
                    package.Add(new MessagePackObject(Key.ExecutionId), @event.ExecutionId.Value);
                    package.Add(new MessagePackObject(Key.ExecutionTicket), @event.ExecutionTicket.Value);
                    package.Add(new MessagePackObject(Key.OrderSide), @event.OrderSide.ToString());
                    package.Add(new MessagePackObject(Key.FilledQuantity), @event.FilledQuantity.Value);
                    package.Add(new MessagePackObject(Key.LeavesQuantity), @event.LeavesQuantity.Value);
                    package.Add(new MessagePackObject(Key.AveragePrice), @event.AveragePrice.ToString());
                    package.Add(new MessagePackObject(Key.ExecutionTime), @event.ExecutionTime.ToIsoString());
                    break;

                case OrderFilled @event:
                    package.Add(new MessagePackObject(Key.OrderEvent), OrderFilled);
                    package.Add(new MessagePackObject(Key.ExecutionId), @event.ExecutionId.Value);
                    package.Add(new MessagePackObject(Key.ExecutionTicket), @event.ExecutionTicket.Value);
                    package.Add(new MessagePackObject(Key.OrderSide), @event.OrderSide.ToString());
                    package.Add(new MessagePackObject(Key.FilledQuantity), @event.FilledQuantity.Value);
                    package.Add(new MessagePackObject(Key.AveragePrice), @event.AveragePrice.ToString());
                    package.Add(new MessagePackObject(Key.ExecutionTime), @event.ExecutionTime.ToIsoString());
                    break;

                default: throw new InvalidOperationException(
                    "Cannot serialize the order event (unrecognized order event).");
            }

            return MsgPackSerializer.Serialize(package.Freeze());
        }

        private static OrderEvent DeserializeOrderEvent(MessagePackObjectDictionary unpacked)
        {
            var symbol = MsgPackSerializationHelper.GetSymbol(unpacked[Key.Symbol].ToString());
            var orderId = new OrderId(unpacked[Key.OrderId].ToString());
            var eventId = Guid.Parse(unpacked[Key.EventId].ToString());
            var eventTimestamp = unpacked[Key.EventTimestamp].ToString().ToZonedDateTimeFromIso();

            switch (unpacked[Key.OrderEvent].ToString())
            {
                case OrderSubmitted:
                    return new OrderSubmitted(
                        symbol,
                        orderId,
                        unpacked[Key.SubmittedTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);

                case OrderAccepted:
                    return new OrderAccepted(
                        symbol,
                        orderId,
                        unpacked[Key.AcceptedTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);

                case OrderRejected:
                    return new OrderRejected(
                        symbol,
                        orderId,
                        unpacked[Key.RejectedTime].ToString().ToZonedDateTimeFromIso(),
                        unpacked[Key.RejectedReason].ToString(),
                        eventId,
                        eventTimestamp);

                case OrderWorking:
                    return new OrderWorking(
                        symbol,
                        orderId,
                        new OrderId(unpacked[Key.OrderIdBroker].ToString()),
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

                case OrderCancelled:
                    return new OrderCancelled(
                        symbol,
                        orderId,
                        unpacked[Key.CancelledTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);

                case OrderCancelReject:
                    return new OrderCancelReject(
                        symbol,
                        orderId,
                        unpacked[Key.RejectedTime].ToString().ToZonedDateTimeFromIso(),
                        unpacked[Key.RejectedResponse].ToString(),
                        unpacked[Key.RejectedReason].ToString(),
                        eventId,
                        eventTimestamp);

                case OrderModified:
                    return new OrderModified(
                        symbol,
                        orderId,
                        new OrderId(unpacked[Key.OrderIdBroker].ToString()),
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.ModifiedPrice].ToString()).Value,
                        unpacked[Key.ModifiedTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);

                case OrderExpired:
                    return new OrderExpired(
                        symbol,
                        orderId,
                        unpacked[Key.ExpiredTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);

                case OrderPartiallyFilled:
                    return new OrderPartiallyFilled(
                        symbol,
                        orderId,
                        new ExecutionId(unpacked[Key.ExecutionId].ToString()),
                        new ExecutionId(unpacked[Key.ExecutionTicket].ToString()),
                        unpacked[Key.OrderSide].ToString().ToEnum<OrderSide>(),
                        Quantity.Create(unpacked[Key.FilledQuantity].AsInt32()),
                        Quantity.Create(unpacked[Key.LeavesQuantity].AsInt32()),
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.AveragePrice].ToString()).Value,
                        unpacked[Key.ExecutionTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);

                case OrderFilled:
                    return new OrderFilled(
                        symbol,
                        orderId,
                        new ExecutionId(unpacked[Key.ExecutionId].ToString()),
                        new ExecutionId(unpacked[Key.ExecutionTicket].ToString()),
                        unpacked[Key.OrderSide].ToString().ToEnum<OrderSide>(),
                        Quantity.Create(unpacked[Key.FilledQuantity].AsInt32()),
                        MsgPackSerializationHelper.GetPrice(unpacked[Key.AveragePrice].ToString()).Value,
                        unpacked[Key.ExecutionTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);

                default: throw new InvalidOperationException(
                    "Cannot deserialize the order event (unrecognized byte[] pattern).");
            }
        }
    }
}
