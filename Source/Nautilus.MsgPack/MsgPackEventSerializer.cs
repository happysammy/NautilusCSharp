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
    using global::MsgPack;
    using Nautilus.Core;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;
    using Price = Nautilus.DomainModel.ValueObjects.Price;
    using Quantity = Nautilus.DomainModel.ValueObjects.Quantity;
    using Symbol = Nautilus.DomainModel.ValueObjects.Symbol;
    using TimeInForce = Nautilus.DomainModel.Enums.TimeInForce;

    /// <summary>
    /// Provides a serializer for the Message Pack specification.
    /// </summary>
    public class MsgPackEventSerializer : IEventSerializer
    {
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
        private const string None = "none";

        /// <summary>
        /// Serialize the given order event to a byte array.
        /// </summary>
        /// <param name="orderEvent">The order event to serialize.</param>
        /// <returns>The serialized order event.</returns>
        /// <exception cref="InvalidOperationException">Throws if the event cannot be serialized.</exception>
        public byte[] SerializeOrderEvent(OrderEvent orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            var package = new MessagePackObjectDictionary
            {
                {new MessagePackObject(Key.Symbol), orderEvent.Symbol.ToString()},
                {new MessagePackObject(Key.OrderId), orderEvent.OrderId.Value},
                {new MessagePackObject(Key.EventId), orderEvent.Id.ToString()},
                {new MessagePackObject(Key.EventTimestamp), orderEvent.Timestamp.ToIsoString()}
            };

            switch (orderEvent)
            {
                case OrderSubmitted @event:
                    package.Add(new MessagePackObject(Key.EventType), OrderSubmitted);
                    package.Add(new MessagePackObject(Key.SubmittedTime), @event.SubmittedTime.ToIsoString());
                    break;

                case OrderAccepted @event:
                    package.Add(new MessagePackObject(Key.EventType), OrderAccepted);
                    package.Add(new MessagePackObject(Key.AcceptedTime), @event.AcceptedTime.ToIsoString());
                    break;

                case OrderRejected @event:
                    package.Add(new MessagePackObject(Key.EventType), OrderRejected);
                    package.Add(new MessagePackObject(Key.RejectedTime), @event.RejectedTime.ToIsoString());
                    package.Add(new MessagePackObject(Key.RejectedReason), @event.RejectedReason);
                    break;

                case OrderWorking @event:
                    package.Add(new MessagePackObject(Key.EventType), OrderWorking);
                    package.Add(new MessagePackObject(Key.OrderIdBroker), @event.OrderIdBroker.ToString());
                    package.Add(new MessagePackObject(Key.Label), @event.Label.ToString());
                    package.Add(new MessagePackObject(Key.OrderSide), @event.OrderSide.ToString());
                    package.Add(new MessagePackObject(Key.OrderType), @event.OrderType.ToString());
                    package.Add(new MessagePackObject(Key.Quantity), @event.Quantity.Value);
                    package.Add(new MessagePackObject(Key.Price), @event.Price.ToString());
                    package.Add(new MessagePackObject(Key.TimeInForce), @event.TimeInForce.ToString());
                    package.Add(new MessagePackObject(Key.ExpireTime), GetExpireTimeString(@event.ExpireTime));
                    package.Add(new MessagePackObject(Key.WorkingTime), @event.WorkingTime.ToIsoString());
                    break;

                case OrderCancelled @event:
                    package.Add(new MessagePackObject(Key.EventType), OrderCancelled);
                    package.Add(new MessagePackObject(Key.CancelledTime), @event.CancelledTime.ToIsoString());
                    break;

                case OrderCancelReject @event:
                    package.Add(new MessagePackObject(Key.EventType), OrderCancelReject);
                    package.Add(new MessagePackObject(Key.RejectedTime), @event.RejectedTime.ToIsoString());
                    package.Add(new MessagePackObject(Key.RejectedResponse), @event.RejectedResponseTo);
                    package.Add(new MessagePackObject(Key.RejectedReason), @event.RejectedReason);
                    break;

                case OrderModified @event:
                    package.Add(new MessagePackObject(Key.EventType), OrderModified);
                    package.Add(new MessagePackObject(Key.OrderIdBroker), @event.BrokerOrderId.ToString());
                    package.Add(new MessagePackObject(Key.ModifiedPrice), @event.ModifiedPrice.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(new MessagePackObject(Key.ModifiedTime), @event.ModifiedTime.ToIsoString());
                    break;

                case OrderExpired @event:
                    package.Add(new MessagePackObject(Key.EventType), OrderExpired);
                    package.Add(new MessagePackObject(Key.ExpiredTime), @event.ExpiredTime.ToIsoString());
                    break;

                case OrderPartiallyFilled @event:
                    package.Add(new MessagePackObject(Key.EventType), OrderPartiallyFilled);
                    package.Add(new MessagePackObject(Key.ExecutionId), @event.ExecutionId.Value);
                    package.Add(new MessagePackObject(Key.ExecutionTicket), @event.ExecutionTicket.Value);
                    package.Add(new MessagePackObject(Key.OrderSide), @event.OrderSide.ToString());
                    package.Add(new MessagePackObject(Key.FilledQuantity), @event.FilledQuantity.Value);
                    package.Add(new MessagePackObject(Key.LeavesQuantity), @event.LeavesQuantity.Value);
                    package.Add(new MessagePackObject(Key.AveragePrice), @event.AveragePrice.ToString());
                    package.Add(new MessagePackObject(Key.ExecutionTime), @event.ExecutionTime.ToIsoString());
                    break;

                case OrderFilled @event:
                    package.Add(new MessagePackObject(Key.EventType), OrderFilled);
                    package.Add(new MessagePackObject(Key.ExecutionId), @event.ExecutionId.Value);
                    package.Add(new MessagePackObject(Key.ExecutionTicket), @event.ExecutionTicket.Value);
                    package.Add(new MessagePackObject(Key.OrderSide), @event.OrderSide.ToString());
                    package.Add(new MessagePackObject(Key.FilledQuantity), @event.FilledQuantity.Value);
                    package.Add(new MessagePackObject(Key.AveragePrice), @event.AveragePrice.ToString());
                    package.Add(new MessagePackObject(Key.ExecutionTime), @event.ExecutionTime.ToIsoString());
                    break;

                default: throw new InvalidOperationException("Cannot serialize the order event object.");
            }

            return MsgPackSerializer.Serialize(package.Freeze());
        }

        /// <summary>
        /// Deserializes the given byte array to an order event.
        /// </summary>
        /// <param name="bytes">The order event to deserialize.</param>
        /// <returns>The deserialized order event.</returns>
        /// <exception cref="InvalidOperationException">Throws if the event cannot be deserialized.</exception>
        public OrderEvent DeserializeOrderEvent(byte[] bytes)
        {
            Debug.NotNull(bytes, nameof(bytes));

            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(bytes);

            var splitSymbol = unpacked[Key.Symbol].ToString().Split('.');
            var symbol = new Symbol(splitSymbol[0], splitSymbol[1].ToEnum<Venue>());
            var orderId = new EntityId(unpacked[Key.OrderId].ToString());
            var eventId = Guid.Parse(unpacked[Key.EventId].ToString());
            var eventTimestamp = unpacked[Key.EventTimestamp].ToString().ToZonedDateTimeFromIso();

            switch (unpacked[Key.EventType].ToString())
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
                        new EntityId(unpacked[Key.OrderIdBroker].ToString()),
                        new Label(unpacked[Key.Label].ToString()),
                        unpacked[Key.OrderSide].ToString().ToEnum<OrderSide>(),
                        unpacked[Key.OrderType].ToString().ToEnum<OrderType>(),
                        Quantity.Create(unpacked[Key.Quantity].AsInt32()),
                        GetPrice(unpacked[Key.Price].ToString()),
                        unpacked[Key.TimeInForce].ToString().ToEnum<TimeInForce>(),
                        GetExpireTime(unpacked[Key.ExpireTime].ToString()),
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
                        new EntityId(unpacked[Key.OrderIdBroker].ToString()),
                        GetPrice(unpacked[Key.ModifiedPrice].ToString()),
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
                        new EntityId(unpacked[Key.ExecutionId].ToString()),
                        new EntityId(unpacked[Key.ExecutionTicket].ToString()),
                        unpacked[Key.OrderSide].ToString().ToEnum<OrderSide>(),
                        Quantity.Create(unpacked[Key.FilledQuantity].AsInt32()),
                        Quantity.Create(unpacked[Key.LeavesQuantity].AsInt32()),
                        GetPrice(unpacked[Key.AveragePrice].ToString()),
                        unpacked[Key.ExecutionTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);

                case OrderFilled:
                    return new OrderFilled(
                        symbol,
                        orderId,
                        new EntityId(unpacked[Key.ExecutionId].ToString()),
                        new EntityId(unpacked[Key.ExecutionTicket].ToString()),
                        unpacked[Key.OrderSide].ToString().ToEnum<OrderSide>(),
                        Quantity.Create(unpacked[Key.FilledQuantity].AsInt32()),
                        GetPrice(unpacked[Key.AveragePrice].ToString()),
                        unpacked[Key.ExecutionTime].ToString().ToZonedDateTimeFromIso(),
                        eventId,
                        eventTimestamp);

                default: throw new InvalidOperationException("Cannot deserialize the order event byte array.");
            }
        }

        private static Price GetPrice(string priceString)
        {
            var priceDecimal = Convert.ToDecimal(priceString);
            var priceDecimalPlaces = priceDecimal.GetDecimalPlaces();

            return Price.Create(priceDecimal, priceDecimalPlaces);
        }

        private static Option<ZonedDateTime?> GetExpireTime(string expireTimeString)
        {
            return expireTimeString == None
                ? Option<ZonedDateTime?>.None()
                : Option<ZonedDateTime?>.Some(expireTimeString.ToZonedDateTimeFromIso());
        }

        private static string GetExpireTimeString(Option<ZonedDateTime?> expireTime)
        {
            return expireTime.HasNoValue
                ? None
                : expireTime.Value.ToIsoString();
        }

        private static class Key
        {
            internal static string EventType => "event_type";
            internal static string EventId => "event_id";
            internal static string EventTimestamp => "event_timestamp";
            internal static string Symbol => "symbol";
            internal static string OrderId => "order_id";
            internal static string OrderIdBroker => "order_id_broker";
            internal static string Label => "label";
            internal static string SubmittedTime => "submitted_time";
            internal static string AcceptedTime => "accepted_time";
            internal static string CancelledTime => "cancelled_time";
            internal static string RejectedTime => "rejected_time";
            internal static string RejectedResponse => "rejected_response";
            internal static string RejectedReason => "rejected_reason";
            internal static string WorkingTime => "working_time";
            internal static string ModifiedTime => "modified_time";
            internal static string ModifiedPrice => "modified_price";
            internal static string ExpireTime => "expire_time";
            internal static string ExpiredTime => "expired_time";
            internal static string ExecutionTime => "execution_time";
            internal static string ExecutionId => "execution_id";
            internal static string ExecutionTicket => "execution_ticket";
            internal static string OrderSide => "order_side";
            internal static string OrderType => "order_type";
            internal static string Price => "price";
            internal static string Quantity => "quantity";
            internal static string TimeInForce => "time_in_force";
            internal static string FilledQuantity => "filled_quantity";
            internal static string LeavesQuantity => "leaves_quantity";
            internal static string AveragePrice => "average_price";
        }
    }
}
