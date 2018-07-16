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

    /// <summary>
    /// Provides a serializer for the Message Pack specification.
    /// </summary>
    public class MsgPackEventSerializer : IEventSerializer
    {
        private const string KeyEventType = "event_type";
        private const string KeyEventId = "event_id";
        private const string KeyEventTimestamp = "event_timestamp";
        private const string KeySymbol = "symbol";
        private const string KeyOrderId = "order_id";
        private const string KeyOrderIdBroker = "order_id_broker";
        private const string KeySubmittedTime = "submitted_time";
        private const string KeyAcceptedTime = "accepted_time";
        private const string KeyCancelledTime = "cancelled_time";
        private const string KeyRejectedTime = "rejected_time";
        private const string KeyRejectedResponse = "rejected_response";
        private const string KeyRejectedReason= "rejected_reason";
        private const string KeyModifiedTime = "modified_time";
        private const string KeyModifiedPrice = "modified_price";
        private const string KeyExpiredTime = "expired_time";
        private const string ValueOrderAccepted = "order_accepted";
        private const string ValueOrderCancelled = "order_cancelled";
        private const string ValueOrderCancelReject = "order_cancel_reject";
        private const string ValueOrderExpired = "order_expired";
        private const string ValueOrderFilled = "order_filled";
        private const string ValueOrderModified = "order_modified";
        private const string ValueOrderPartiallyFilled = "order_partially_filled";
        private const string ValueOrderRejected = "order_rejected";
        private const string ValueOrderSubmitted = "order_submitted";
        private const string ValueOrderWorking = "order_working";

        /// <summary>
        /// Serialize the given order event.
        /// </summary>
        /// <param name="orderEvent">The order event to serialize.</param>
        /// <returns>The serialized order event.</returns>
        /// <exception cref="InvalidOperationException">Throws if the event cannot be serialized.</exception>
        public byte[] SerializeOrderEvent(OrderEvent orderEvent)
        {
            Debug.NotNull(orderEvent, nameof(orderEvent));

            var package = new MessagePackObjectDictionary
            {
                {new MessagePackObject(KeySymbol), orderEvent.Symbol.ToString()},
                {new MessagePackObject(KeyOrderId), orderEvent.OrderId.Value},
                {new MessagePackObject(KeyEventId), orderEvent.Id.ToString()},
                {new MessagePackObject(KeyEventTimestamp), orderEvent.Timestamp.ToIsoString()}
            };

            switch (orderEvent)
            {
                case OrderSubmitted @event:
                    package.Add(new MessagePackObject(KeyEventType), ValueOrderSubmitted);
                    package.Add(new MessagePackObject(KeySubmittedTime), @event.SubmittedTime.ToIsoString());
                    break;

                case OrderAccepted @event:
                    package.Add(new MessagePackObject(KeyEventType), ValueOrderAccepted);
                    package.Add(new MessagePackObject(KeyAcceptedTime), @event.AcceptedTime.ToIsoString());
                    break;

                case OrderCancelled @event:
                    package.Add(new MessagePackObject(KeyEventType), ValueOrderCancelled);
                    package.Add(new MessagePackObject(KeyCancelledTime), @event.CancelledTime.ToIsoString());
                    break;

                case OrderRejected @event:
                    package.Add(new MessagePackObject(KeyEventType), ValueOrderRejected);
                    package.Add(new MessagePackObject(KeyRejectedTime), @event.RejectedTime.ToIsoString());
                    package.Add(new MessagePackObject(KeyRejectedReason), @event.RejectedReason);
                    break;

                case OrderExpired @event:
                    package.Add(new MessagePackObject(KeyEventType), ValueOrderExpired);
                    package.Add(new MessagePackObject(KeyExpiredTime), @event.ExpiredTime.ToIsoString());
                    break;

                case OrderCancelReject @event:
                    package.Add(new MessagePackObject(KeyEventType), ValueOrderCancelReject);
                    package.Add(new MessagePackObject(KeyRejectedTime), @event.RejectedTime.ToIsoString());
                    package.Add(new MessagePackObject(KeyRejectedResponse), @event.RejectedResponseTo);
                    package.Add(new MessagePackObject(KeyRejectedReason), @event.RejectedReason);
                    break;

                case OrderModified @event:
                    package.Add(new MessagePackObject(KeyEventType), ValueOrderModified);
                    package.Add(new MessagePackObject(KeyOrderIdBroker), @event.BrokerOrderId.ToString());
                    package.Add(new MessagePackObject(KeyModifiedTime), @event.ModifiedTime.ToIsoString());
                    package.Add(new MessagePackObject(KeyModifiedPrice), @event.ModifiedPrice.Value.ToString(CultureInfo.InvariantCulture));
                    break;

                default: throw new InvalidOperationException("Cannot serialize order event.");
            }

            return MsgPackSerializer.Serialize(package.Freeze());

        }
    }
}
