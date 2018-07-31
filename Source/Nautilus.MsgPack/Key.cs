// -------------------------------------------------------------------------------------------------
// <copyright file="Key.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.MsgPack
{
    internal static class Key
    {
        internal static string EventType => "event_type";
        internal static string EventId => "event_id";
        internal static string EventTimestamp => "event_timestamp";
        internal static string OrderEvent => "order_event";
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
        internal static string Timestamp => "timestamp";
        internal static string CommandType => "command_type";
        internal static string CommandId => "command_id";
        internal static string CommandTimestamp => "command_timestamp";
        internal static string OrderCommand => "order_command";
        internal static string Order => "order";
        internal static string CancelReason => "cancel_reason";
        internal static string Currency => "currency";
        internal static string CashBalance => "cash_balance";
        internal static string CashStartDay => "cash_start_day";
        internal static string CashActivityDay => "cash_activity_day";
        internal static string MarginUsedLiquidation => "margin_used_liquidation";
        internal static string MarginUsedMaintenance => "margin_used_maintenance";
        internal static string MarginRatio => "margin_ratio";
        internal static string MarginCallStatus => "margin_call_status";
    }
}
