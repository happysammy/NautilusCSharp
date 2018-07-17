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
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

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
                    package.Add(new MessagePackObject(Key.RejectedTime), @event.WorkingTime.ToIsoString());
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
                    package.Add(new MessagePackObject(Key.ExecutionId), @event.ExecutionTicket.Value);
                    package.Add(new MessagePackObject(Key.OrderSide), @event.OrderSide.ToString());
                    package.Add(new MessagePackObject(Key.FilledQuantity), @event.FilledQuantity.Value);
                    package.Add(new MessagePackObject(Key.LeavesQuantity), @event.LeavesQuantity.Value);
                    package.Add(new MessagePackObject(Key.AveragePrice), @event.AveragePrice.ToString());
                    package.Add(new MessagePackObject(Key.ExecutionTime), @event.OrderSide.ToString());
                    break;

                case OrderFilled @event:
                    package.Add(new MessagePackObject(Key.EventType), OrderFilled);
                    package.Add(new MessagePackObject(Key.ExecutionId), @event.ExecutionId.Value);
                    package.Add(new MessagePackObject(Key.ExecutionId), @event.ExecutionTicket.Value);
                    package.Add(new MessagePackObject(Key.OrderSide), @event.OrderSide.ToString());
                    package.Add(new MessagePackObject(Key.FilledQuantity), @event.FilledQuantity.Value);
                    package.Add(new MessagePackObject(Key.AveragePrice), @event.AveragePrice.ToString());
                    package.Add(new MessagePackObject(Key.ExecutionTime), @event.OrderSide.ToString());
                    break;

                default: throw new InvalidOperationException("Cannot serialize the order event.");
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
            var eventId = unpacked[Key.EventId].ToString();
            var eventTimestamp = unpacked[Key.EventTimestamp].ToString().ToZonedDateTimeFromIso();

            switch (unpacked[Key.EventType].ToString())
            {
                case OrderSubmitted:
                    return new OrderSubmitted(
                        symbol,
                        orderId,
                        unpacked[Key.SubmittedTime].ToString().ToZonedDateTimeFromIso(),
                        Guid.Parse(eventId),
                        eventTimestamp);

//                case OrderAccepted @event:
//                    package.Add(new MessagePackObject(Key.EventType), Value.OrderAccepted);
//                    package.Add(new MessagePackObject(Key.AcceptedTime), @event.AcceptedTime.ToIsoString());
//                    break;
//
//                case OrderRejected @event:
//                    package.Add(new MessagePackObject(Key.EventType), Value.OrderRejected);
//                    package.Add(new MessagePackObject(Key.RejectedTime), @event.RejectedTime.ToIsoString());
//                    package.Add(new MessagePackObject(Key.RejectedReason), @event.RejectedReason);
//                    break;
//
//                case OrderWorking @event:
//                    package.Add(new MessagePackObject(Key.EventType), Value.OrderWorking);
//                    package.Add(new MessagePackObject(Key.RejectedTime), @event.WorkingTime.ToIsoString());
//                    break;
//
//                case OrderCancelled @event:
//                    package.Add(new MessagePackObject(Key.EventType), Value.OrderCancelled);
//                    package.Add(new MessagePackObject(Key.CancelledTime), @event.CancelledTime.ToIsoString());
//                    break;
//
//                case OrderCancelReject @event:
//                    package.Add(new MessagePackObject(Key.EventType), Value.OrderCancelReject);
//                    package.Add(new MessagePackObject(Key.RejectedTime), @event.RejectedTime.ToIsoString());
//                    package.Add(new MessagePackObject(Key.RejectedResponse), @event.RejectedResponseTo);
//                    package.Add(new MessagePackObject(Key.RejectedReason), @event.RejectedReason);
//                    break;
//
//                case OrderModified @event:
//                    package.Add(new MessagePackObject(Key.EventType), Value.OrderModified);
//                    package.Add(new MessagePackObject(Key.OrderIdBroker), @event.BrokerOrderId.ToString());
//                    package.Add(new MessagePackObject(Key.ModifiedPrice), @event.ModifiedPrice.Value.ToString(CultureInfo.InvariantCulture));
//                    package.Add(new MessagePackObject(Key.ModifiedTime), @event.ModifiedTime.ToIsoString());
//                    break;
//
//                case OrderExpired @event:
//                    package.Add(new MessagePackObject(Key.EventType), Value.OrderExpired);
//                    package.Add(new MessagePackObject(Key.ExpiredTime), @event.ExpiredTime.ToIsoString());
//                    break;
//
//                case OrderPartiallyFilled @event:
//                    package.Add(new MessagePackObject(Key.EventType), Value.OrderPartiallyFilled);
//                    package.Add(new MessagePackObject(Key.ExecutionId), @event.ExecutionId.Value);
//                    package.Add(new MessagePackObject(Key.ExecutionId), @event.ExecutionTicket.Value);
//                    package.Add(new MessagePackObject(Key.OrderSide), @event.OrderSide.ToString());
//                    package.Add(new MessagePackObject(Key.FilledQuantity), @event.FilledQuantity.Value);
//                    package.Add(new MessagePackObject(Key.LeavesQuantity), @event.LeavesQuantity.Value);
//                    package.Add(new MessagePackObject(Key.AveragePrice), @event.AveragePrice.ToString());
//                    package.Add(new MessagePackObject(Key.ExecutionTime), @event.OrderSide.ToString());
//                    break;
//
//                case OrderFilled @event:
//                    package.Add(new MessagePackObject(Key.EventType), Value.OrderPartiallyFilled);
//                    package.Add(new MessagePackObject(Key.ExecutionId), @event.ExecutionId.Value);
//                    package.Add(new MessagePackObject(Key.ExecutionId), @event.ExecutionTicket.Value);
//                    package.Add(new MessagePackObject(Key.OrderSide), @event.OrderSide.ToString());
//                    package.Add(new MessagePackObject(Key.FilledQuantity), @event.FilledQuantity.Value);
//                    package.Add(new MessagePackObject(Key.AveragePrice), @event.AveragePrice.ToString());
//                    package.Add(new MessagePackObject(Key.ExecutionTime), @event.OrderSide.ToString());
//                    break;

                default: throw new InvalidOperationException("Cannot serialize the order event.");
            }
        }

        private static class Key
        {
            internal static string EventType => "event_type";
            internal static string EventId => "event_id";
            internal static string EventTimestamp => "event_timestamp";
            internal static string Symbol => "symbol";
            internal static string OrderId => "order_id";
            internal static string OrderIdBroker => "order_id_broker";
            internal static string SubmittedTime => "submitted_time";
            internal static string AcceptedTime => "accepted_time";
            internal static string CancelledTime => "cancelled_time";
            internal static string RejectedTime => "rejected_time";
            internal static string RejectedResponse => "rejected_response";
            internal static string RejectedReason => "rejected_reason";
            internal static string ModifiedTime => "modified_time";
            internal static string ModifiedPrice => "modified_price";
            internal static string ExpiredTime => "expired_time";
            internal static string ExecutionTime => "execution_time";
            internal static string ExecutionId => "execution_id";
            internal static string ExecutionTicket => "execution_ticket";
            internal static string OrderSide => "order_side";
            internal static string FilledQuantity => "filled_quantity";
            internal static string LeavesQuantity => "leaves_quantity";
            internal static string AveragePrice => "average_price";
        }
    }
}
