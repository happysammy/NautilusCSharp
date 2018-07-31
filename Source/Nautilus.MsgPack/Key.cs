// -------------------------------------------------------------------------------------------------
// <copyright file="Key.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.MsgPack
{
    /// <summary>
    /// Provides key strings for Message Pack serialization.
    /// </summary>
    internal static class Key
    {
        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string EventType => "event_type";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string EventId => "event_id";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string EventTimestamp => "event_timestamp";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string OrderEvent => "order_event";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Symbol => "symbol";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string OrderId => "order_id";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string OrderIdBroker => "order_id_broker";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Label => "label";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string SubmittedTime => "submitted_time";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string AcceptedTime => "accepted_time";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string CancelledTime => "cancelled_time";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string RejectedTime => "rejected_time";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string RejectedResponse => "rejected_response";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string RejectedReason => "rejected_reason";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string WorkingTime => "working_time";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string ModifiedTime => "modified_time";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string ModifiedPrice => "modified_price";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string ExpireTime => "expire_time";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string ExpiredTime => "expired_time";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string ExecutionTime => "execution_time";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string ExecutionId => "execution_id";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string ExecutionTicket => "execution_ticket";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string OrderSide => "order_side";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string OrderType => "order_type";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Price => "price";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Quantity => "quantity";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string TimeInForce => "time_in_force";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string FilledQuantity => "filled_quantity";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string LeavesQuantity => "leaves_quantity";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string AveragePrice => "average_price";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Timestamp => "timestamp";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string CommandType => "command_type";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string CommandId => "command_id";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string CommandTimestamp => "command_timestamp";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string OrderCommand => "order_command";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Order => "order";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string CancelReason => "cancel_reason";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Currency => "currency";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string CashBalance => "cash_balance";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string CashStartDay => "cash_start_day";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string CashActivityDay => "cash_activity_day";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string MarginUsedLiquidation => "margin_used_liquidation";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string MarginUsedMaintenance => "margin_used_maintenance";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string MarginRatio => "margin_ratio";

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string MarginCallStatus => "margin_call_status";
    }
}
