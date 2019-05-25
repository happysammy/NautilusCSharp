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
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;

    /// <summary>
    /// Provides an events binary serializer for the Message Pack specification.
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
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.Label, orderEvent.OrderLabel.ToString());
                    package.Add(Key.OrderSide, orderEvent.OrderSide.ToString());
                    package.Add(Key.OrderType, orderEvent.OrderType.ToString());
                    package.Add(Key.Quantity, orderEvent.Quantity.Value);
                    package.Add(Key.Price, MsgPackObjectConverter.FromNullablePrice(orderEvent.Price));
                    package.Add(Key.TimeInForce, orderEvent.TimeInForce.ToString());
                    package.Add(Key.ExpireTime, MsgPackObjectConverter.ToExpireTime(orderEvent.ExpireTime));
                    break;
                case OrderSubmitted orderEvent:
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.SubmittedTime, orderEvent.SubmittedTime.ToIsoString());
                    break;
                case OrderAccepted orderEvent:
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.AcceptedTime, orderEvent.AcceptedTime.ToIsoString());
                    break;
                case OrderRejected orderEvent:
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.RejectedTime, orderEvent.RejectedTime.ToIsoString());
                    package.Add(Key.RejectedReason, orderEvent.RejectedReason);
                    break;
                case OrderWorking orderEvent:
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.BrokerOrderId, orderEvent.OrderIdBroker.ToString());
                    package.Add(Key.Label, orderEvent.Label.ToString());
                    package.Add(Key.OrderSide, orderEvent.OrderSide.ToString());
                    package.Add(Key.OrderType, orderEvent.OrderType.ToString());
                    package.Add(Key.Quantity, orderEvent.Quantity.Value);
                    package.Add(Key.Price, orderEvent.Price.ToString());
                    package.Add(Key.TimeInForce, orderEvent.TimeInForce.ToString());
                    package.Add(Key.ExpireTime, MsgPackObjectConverter.ToExpireTime(orderEvent.ExpireTime));
                    package.Add(Key.WorkingTime, orderEvent.WorkingTime.ToIsoString());
                    break;
                case OrderCancelled orderEvent:
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.CancelledTime, orderEvent.CancelledTime.ToIsoString());
                    break;
                case OrderCancelReject orderEvent:
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.RejectedTime, orderEvent.RejectedTime.ToIsoString());
                    package.Add(Key.RejectedResponse, orderEvent.RejectedResponseTo);
                    package.Add(Key.RejectedReason, orderEvent.RejectedReason);
                    break;
                case OrderModified orderEvent:
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.BrokerOrderId, orderEvent.BrokerOrderId.ToString());
                    package.Add(Key.ModifiedPrice, orderEvent.ModifiedPrice.Value.ToString(CultureInfo.InvariantCulture));
                    package.Add(Key.ModifiedTime, orderEvent.ModifiedTime.ToIsoString());
                    break;
                case OrderExpired orderEvent:
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.ExpiredTime, orderEvent.ExpiredTime.ToIsoString());
                    break;
                case OrderPartiallyFilled orderEvent:
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.ExecutionId, orderEvent.ExecutionId.ToString());
                    package.Add(Key.ExecutionTicket, orderEvent.ExecutionTicket.ToString());
                    package.Add(Key.OrderSide, orderEvent.OrderSide.ToString());
                    package.Add(Key.FilledQuantity, orderEvent.FilledQuantity.Value);
                    package.Add(Key.LeavesQuantity, orderEvent.LeavesQuantity.Value);
                    package.Add(Key.AveragePrice, orderEvent.AveragePrice.ToString());
                    package.Add(Key.ExecutionTime, orderEvent.ExecutionTime.ToIsoString());
                    break;
                case OrderFilled orderEvent:
                    package.Add(Key.Symbol, orderEvent.Symbol.ToString());
                    package.Add(Key.OrderId, orderEvent.OrderId.ToString());
                    package.Add(Key.ExecutionId, orderEvent.ExecutionId.ToString());
                    package.Add(Key.ExecutionTicket, orderEvent.ExecutionTicket.ToString());
                    package.Add(Key.OrderSide, orderEvent.OrderSide.ToString());
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
        public Event Deserialize(byte[] eventBytes)
        {
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(eventBytes);

            var eventId = MsgPackObjectConverter.ToGuid(unpacked[Key.EventId]);
            var eventTimestamp = MsgPackObjectConverter.ToZonedDateTime(unpacked[Key.EventTimestamp]);
            var @event = unpacked[Key.Event].ToString();

            switch (@event)
            {
                case nameof(AccountEvent):
                    var currency = MsgPackObjectConverter.ToCurrency(unpacked[Key.Currency]);
                    return new AccountEvent(
                        new AccountId(unpacked[Key.AccountId].ToString()),
                        MsgPackObjectConverter.ToBrokerage(unpacked[Key.Broker]),
                        unpacked[Key.AccountNumber].ToString(),
                        currency,
                        MsgPackObjectConverter.ToMoney(unpacked[Key.CashBalance], currency),
                        MsgPackObjectConverter.ToMoney(unpacked[Key.CashStartDay], currency),
                        MsgPackObjectConverter.ToMoney(unpacked[Key.CashActivityDay], currency),
                        MsgPackObjectConverter.ToMoney(unpacked[Key.MarginUsedLiquidation], currency),
                        MsgPackObjectConverter.ToMoney(unpacked[Key.MarginUsedMaintenance], currency),
                        Convert.ToDecimal(unpacked[Key.MarginRatio].ToString()),
                        unpacked[Key.MarginCallStatus].ToString(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderInitialized):
                    return new OrderInitialized(
                        MsgPackObjectConverter.ToSymbol(unpacked[Key.Symbol]),
                        MsgPackObjectConverter.ToOrderId(unpacked[Key.OrderId]),
                        MsgPackObjectConverter.ToLabel(unpacked[Key.Label]),
                        MsgPackObjectConverter.ToOrderSide(unpacked[Key.OrderSide]),
                        MsgPackObjectConverter.ToOrderType(unpacked[Key.OrderType]),
                        MsgPackObjectConverter.ToQuantity(unpacked[Key.Quantity]),
                        MsgPackObjectConverter.ToNullablePrice(unpacked[Key.Price]),
                        MsgPackObjectConverter.ToTimeInForce(unpacked[Key.TimeInForce]),
                        MsgPackObjectConverter.ToExpireTime(unpacked[Key.ExpireTime]),
                        eventId,
                        eventTimestamp);
                case nameof(OrderSubmitted):
                    return new OrderSubmitted(
                        MsgPackObjectConverter.ToSymbol(unpacked[Key.Symbol]),
                        MsgPackObjectConverter.ToOrderId(unpacked[Key.OrderId]),
                        MsgPackObjectConverter.ToZonedDateTime(unpacked[Key.SubmittedTime]),
                        eventId,
                        eventTimestamp);
                case nameof(OrderAccepted):
                    return new OrderAccepted(
                        MsgPackObjectConverter.ToSymbol(unpacked[Key.Symbol]),
                        MsgPackObjectConverter.ToOrderId(unpacked[Key.OrderId]),
                        MsgPackObjectConverter.ToZonedDateTime(unpacked[Key.AcceptedTime]),
                        eventId,
                        eventTimestamp);
                case nameof(OrderRejected):
                    return new OrderRejected(
                        MsgPackObjectConverter.ToSymbol(unpacked[Key.Symbol]),
                        MsgPackObjectConverter.ToOrderId(unpacked[Key.OrderId]),
                        MsgPackObjectConverter.ToZonedDateTime(unpacked[Key.RejectedTime]),
                        unpacked[Key.RejectedReason].ToString(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderWorking):
                    return new OrderWorking(
                        MsgPackObjectConverter.ToSymbol(unpacked[Key.Symbol]),
                        MsgPackObjectConverter.ToOrderId(unpacked[Key.OrderId]),
                        MsgPackObjectConverter.ToOrderId(unpacked[Key.BrokerOrderId]),
                        MsgPackObjectConverter.ToLabel(unpacked[Key.Label]),
                        MsgPackObjectConverter.ToOrderSide(unpacked[Key.OrderSide]),
                        MsgPackObjectConverter.ToOrderType(unpacked[Key.OrderType]),
                        MsgPackObjectConverter.ToQuantity(unpacked[Key.Quantity]),
                        MsgPackObjectConverter.ToPrice(unpacked[Key.Price]),
                        MsgPackObjectConverter.ToTimeInForce(unpacked[Key.TimeInForce]),
                        MsgPackObjectConverter.ToExpireTime(unpacked[Key.ExpireTime]),
                        MsgPackObjectConverter.ToZonedDateTime(unpacked[Key.WorkingTime]),
                        eventId,
                        eventTimestamp);
                case nameof(OrderCancelled):
                    return new OrderCancelled(
                        MsgPackObjectConverter.ToSymbol(unpacked[Key.Symbol]),
                        MsgPackObjectConverter.ToOrderId(unpacked[Key.OrderId]),
                        MsgPackObjectConverter.ToZonedDateTime(unpacked[Key.CancelledTime]),
                        eventId,
                        eventTimestamp);
                case nameof(OrderCancelReject):
                    return new OrderCancelReject(
                        MsgPackObjectConverter.ToSymbol(unpacked[Key.Symbol]),
                        MsgPackObjectConverter.ToOrderId(unpacked[Key.OrderId]),
                        MsgPackObjectConverter.ToZonedDateTime(unpacked[Key.RejectedTime]),
                        unpacked[Key.RejectedResponse].ToString(),
                        unpacked[Key.RejectedReason].ToString(),
                        eventId,
                        eventTimestamp);
                case nameof(OrderModified):
                    return new OrderModified(
                        MsgPackObjectConverter.ToSymbol(unpacked[Key.Symbol]),
                        MsgPackObjectConverter.ToOrderId(unpacked[Key.OrderId]),
                        MsgPackObjectConverter.ToOrderId(unpacked[Key.OrderId]),
                        MsgPackObjectConverter.ToPrice(unpacked[Key.ModifiedPrice]),
                        MsgPackObjectConverter.ToZonedDateTime(unpacked[Key.ModifiedTime]),
                        eventId,
                        eventTimestamp);
                case nameof(OrderExpired):
                    return new OrderExpired(
                        MsgPackObjectConverter.ToSymbol(unpacked[Key.Symbol]),
                        MsgPackObjectConverter.ToOrderId(unpacked[Key.OrderId]),
                        MsgPackObjectConverter.ToZonedDateTime(unpacked[Key.ExpiredTime]),
                        eventId,
                        eventTimestamp);
                case nameof(OrderPartiallyFilled):
                    return new OrderPartiallyFilled(
                        MsgPackObjectConverter.ToSymbol(unpacked[Key.Symbol]),
                        MsgPackObjectConverter.ToOrderId(unpacked[Key.OrderId]),
                        MsgPackObjectConverter.ToExecutionId(unpacked[Key.ExecutionId]),
                        MsgPackObjectConverter.ToExecutionTicket(unpacked[Key.ExecutionId]),
                        MsgPackObjectConverter.ToOrderSide(unpacked[Key.OrderSide]),
                        MsgPackObjectConverter.ToQuantity(unpacked[Key.FilledQuantity]),
                        MsgPackObjectConverter.ToQuantity(unpacked[Key.LeavesQuantity]),
                        MsgPackObjectConverter.ToPrice(unpacked[Key.AveragePrice]),
                        MsgPackObjectConverter.ToZonedDateTime(unpacked[Key.ExecutionTime]),
                        eventId,
                        eventTimestamp);
                case nameof(OrderFilled):
                    return new OrderFilled(
                        MsgPackObjectConverter.ToSymbol(unpacked[Key.Symbol]),
                        MsgPackObjectConverter.ToOrderId(unpacked[Key.OrderId]),
                        MsgPackObjectConverter.ToExecutionId(unpacked[Key.ExecutionId]),
                        MsgPackObjectConverter.ToExecutionTicket(unpacked[Key.ExecutionId]),
                        MsgPackObjectConverter.ToOrderSide(unpacked[Key.OrderSide]),
                        MsgPackObjectConverter.ToQuantity(unpacked[Key.FilledQuantity]),
                        MsgPackObjectConverter.ToPrice(unpacked[Key.AveragePrice]),
                        MsgPackObjectConverter.ToZonedDateTime(unpacked[Key.ExecutionTime]),
                        eventId,
                        eventTimestamp);
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(@event, nameof(@event));
            }
        }
    }
}
