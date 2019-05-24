// -------------------------------------------------------------------------------------------------
// <copyright file="Key.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.Serialization
{
    /// <summary>
    /// Provides key strings for Message Pack serialization.
    /// </summary>
    internal static class Key
    {
        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Type => nameof(Type);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Command => nameof(Command);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Event => nameof(Event);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string EventId => nameof(EventId);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string EventTimestamp => nameof(EventTimestamp);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Symbol => nameof(Symbol);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string TraderId => nameof(TraderId);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string StrategyId => nameof(StrategyId);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string PositionId => nameof(PositionId);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string OrderId => nameof(OrderId);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string BrokerOrderId => nameof(BrokerOrderId);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Label => nameof(Label);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string SubmittedTime => nameof(SubmittedTime);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string AcceptedTime => nameof(AcceptedTime);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string CancelledTime => nameof(CancelledTime);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string RejectedTime => nameof(RejectedTime);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string RejectedResponse => nameof(RejectedResponse);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string RejectedReason => nameof(RejectedReason);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string WorkingTime => nameof(WorkingTime);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string ModifiedTime => nameof(ModifiedTime);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string ModifiedPrice => nameof(ModifiedPrice);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string ExpireTime => nameof(ExpireTime);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string ExpiredTime => nameof(ExpiredTime);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string ExecutionTime => nameof(ExecutionTime);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string ExecutionId => nameof(ExecutionId);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string ExecutionTicket => nameof(ExecutionTicket);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string OrderSide => nameof(OrderSide);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string OrderType => nameof(OrderType);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Price => nameof(Price);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Quantity => nameof(Quantity);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string TimeInForce => nameof(TimeInForce);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string FilledQuantity => nameof(FilledQuantity);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string LeavesQuantity => nameof(LeavesQuantity);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string AveragePrice => nameof(AveragePrice);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Timestamp => nameof(Timestamp);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string CommandId => nameof(CommandId);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string CommandTimestamp => nameof(CommandTimestamp);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Order => nameof(Order);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string CancelReason => nameof(CancelReason);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Broker => nameof(Broker);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string AccountId => nameof(AccountId);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string AccountNumber => nameof(AccountNumber);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Currency => nameof(Currency);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string CashBalance => nameof(CashBalance);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string CashStartDay => nameof(CashStartDay);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string CashActivityDay => nameof(CashActivityDay);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string MarginUsedLiquidation => nameof(MarginUsedLiquidation);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string MarginUsedMaintenance => nameof(MarginUsedMaintenance);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string MarginRatio => nameof(MarginRatio);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string MarginCallStatus => nameof(MarginCallStatus);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string Entry => nameof(Entry);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string StopLoss => nameof(StopLoss);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string TakeProfit => nameof(TakeProfit);

        /// <summary>
        /// Gets the key string.
        /// </summary>
        internal static string HasTakeProfit => nameof(HasTakeProfit);
    }
}
