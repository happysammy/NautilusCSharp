//--------------------------------------------------------------------------------------------------
// <copyright file="FixMessageHelper.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Correctness;
    using Nautilus.DomainModel.Enums;
    using NodaTime;
    using NodaTime.Text;
    using QuickFix.Fields;
    using SecurityType = Nautilus.DomainModel.Enums.SecurityType;
    using TimeInForce = Nautilus.DomainModel.Enums.TimeInForce;

    /// <summary>
    /// Provides useful methods for assisting with FIX messages.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public static class FixMessageHelper
    {
        private static readonly Dictionary<string, string> SecurityRequestResults = new Dictionary<string, string>
        {
            { "0", "Valid request" },
            { "1", "Invalid or unsupported request" },
            { "2", "No instruments found that match selection criteria" },
            { "3", "Not authorized to retrieve instrument data" },
            { "4", "Instrument data temporarily unavailable" },
            { "5", "Request for instrument data not supported" },
        };

        /// <summary>
        /// The get security request result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string GetSecurityRequestResult(SecurityRequestResult result)
        {
            return SecurityRequestResults.TryGetValue(result.ToString(), out var value)
                ? value
                : "Security request result unknown";
        }

        private static readonly Dictionary<string, string> CxlRejReasonStrings = new Dictionary<string, string>
        {
            { "0", "Too late to cancel" },
            { "1", "Unknown order" },
            { "2", "Broker Option" },
            { "3", "Order already in Pending Cancel or Pending Replace status" },
            { "4", "Unable to process Order Mass Cancel Request (q)" },
            { "5", "OrigOrdModTime (586) did not match last TransactTime (60) of order" },
            { "6", "Duplicate ClOrdID (11) received" },
            { "99", "Other" },
        };

        /// <summary>
        /// The get cancel reject reason string.
        /// </summary>
        /// <param name="rejectCode">The reject code.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string GetCancelRejectReasonString(string rejectCode)
        {
            return CxlRejReasonStrings.TryGetValue(rejectCode, out var value)
                ? value
                : "Other";
        }

        private static readonly Dictionary<string, string> CxlRejResponse = new Dictionary<string, string>
        {
            { "1", "OrderCancel" },
            { "2", "OrderCancelRequest" },
        };

        /// <summary>
        /// Returns the cancel reject response to.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string GetCxlRejResponseTo(CxlRejResponseTo response)
        {
            return CxlRejResponse.TryGetValue(response.ToString(), out var value)
                ? value
                : string.Empty;
        }

        private static readonly Dictionary<TimeInForce, QuickFix.Fields.TimeInForce> TimeInForceIndex = new Dictionary<TimeInForce, QuickFix.Fields.TimeInForce>
        {
            { TimeInForce.GTC, new QuickFix.Fields.TimeInForce(QuickFix.Fields.TimeInForce.GOOD_TILL_CANCEL) },
            { TimeInForce.GTD, new QuickFix.Fields.TimeInForce(QuickFix.Fields.TimeInForce.GOOD_TILL_DATE) },
            { TimeInForce.DAY, new QuickFix.Fields.TimeInForce(QuickFix.Fields.TimeInForce.DAY) },
            { TimeInForce.FOC, new QuickFix.Fields.TimeInForce(QuickFix.Fields.TimeInForce.FILL_OR_KILL) },
            { TimeInForce.IOC, new QuickFix.Fields.TimeInForce(QuickFix.Fields.TimeInForce.IMMEDIATE_OR_CANCEL) },
        };

        private static readonly Dictionary<string, TimeInForce> TimeInForceStringIndex = new Dictionary<string, TimeInForce>
        {
            { "GTC", TimeInForce.GTC },
            { "GTD", TimeInForce.GTD },
            { "DAY", TimeInForce.DAY },
            { "FOC", TimeInForce.FOC },
            { "IOC", TimeInForce.IOC },
            { "1", TimeInForce.GTC },
            { "6", TimeInForce.GTD },
            { "0", TimeInForce.DAY },
            { "4", TimeInForce.FOC },
            { "3", TimeInForce.IOC },
        };

        /// <summary>
        /// The get fix time in force.
        /// </summary>
        /// <param name="timeInForce">The time in force.</param>
        /// <returns>A <see cref="TimeInForce"/>.</returns>
        public static QuickFix.Fields.TimeInForce? GetFixTimeInForce(TimeInForce timeInForce)
        {
            return TimeInForceIndex.TryGetValue(timeInForce, out var value)
                ? value
                : null;
        }

        /// <summary>
        /// The get nautilus time in force.
        /// </summary>
        /// <param name="timeInForce">The time in force.</param>
        /// <returns>A <see cref="TimeInForce"/>.</returns>
        public static TimeInForce GetTimeInForce(string timeInForce)
        {
            return TimeInForceStringIndex.TryGetValue(timeInForce, out var value)
                ? value
                : TimeInForce.UNKNOWN;
        }

        /// <summary>
        /// The get security type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A <see cref="SecurityType"/>.</returns>
        public static SecurityType GetSecurityType(string type)
        {
            // TODO
            if (type == "1")
            {
                return SecurityType.FOREX;
            }

            return SecurityType.CFD;
        }

        /// <summary>
        /// The get nautilus order side.
        /// </summary>
        /// <param name="side">The side.</param>
        /// <returns>The <see cref="OrderSide"/>.</returns>
        public static OrderSide GetOrderSide(string side)
        {
            if (side == Side.BUY.ToString())
            {
                return OrderSide.BUY;
            }

            if (side == Side.SELL.ToString())
            {
                return OrderSide.SELL;
            }

            return OrderSide.UNKNOWN;
        }

        /// <summary>
        /// The get fix order side.
        /// </summary>
        /// <param name="orderSide">The order side.</param>
        /// <returns>A <see cref="Side"/>.</returns>
        public static Side GetFixOrderSide(OrderSide orderSide)
        {
            return orderSide == OrderSide.BUY
                ? new Side(Side.BUY)
                : new Side(Side.SELL);
        }

        /// <summary>
        /// The get opposite fix order side.
        /// </summary>
        /// <param name="orderSide">The order side.</param>
        /// <returns>A <see cref="Side"/>.</returns>
        public static Side GetOppositeFixOrderSide(OrderSide orderSide)
        {
            return orderSide == OrderSide.BUY
                ? new Side(Side.SELL)
                : new Side(Side.BUY);
        }

        /// <summary>
        /// The get nautilus order type.
        /// </summary>
        /// <param name="fixType">The fix type.</param>
        /// <returns>The <see cref="OrderType"/>.</returns>
        public static OrderType GetOrderType(string fixType)
        {
            if (fixType == OrdType.MARKET.ToString())
            {
                return OrderType.MARKET;
            }

            if (fixType == OrdType.STOP.ToString())
            {
                return OrderType.STOP_MARKET;
            }

            if (fixType == OrdType.STOP_LIMIT.ToString())
            {
                return OrderType.STOP_LIMIT;
            }

            if (fixType == OrdType.LIMIT.ToString())
            {
                return OrderType.LIMIT;
            }

            if (fixType == OrdType.MARKET_IF_TOUCHED.ToString())
            {
                return OrderType.MIT;
            }

            return OrderType.UNKNOWN;
        }

        /// <summary>
        /// The get fix order type.
        /// </summary>
        /// <param name="orderType">The order type.</param>
        /// <returns>The <see cref="OrdType"/>.</returns>
        public static OrdType GetFixOrderType(OrderType orderType)
        {
            switch (orderType)
            {
                case OrderType.MARKET:
                    return new OrdType(OrdType.MARKET);
                case OrderType.STOP_MARKET:
                    return new OrdType(OrdType.STOP);
                case OrderType.STOP_LIMIT:
                    return new OrdType(OrdType.STOP_LIMIT);
                case OrderType.LIMIT:
                    return new OrdType(OrdType.LIMIT);
                case OrderType.MIT:
                    return new OrdType(OrdType.MARKET_IF_TOUCHED);
                case OrderType.UNKNOWN:
                    goto default;
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(orderType, nameof(orderType));
            }
        }

        /// <summary>
        /// The get fix order status.
        /// </summary>
        /// <param name="orderStatus">The order status.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string GetFixOrderStatus(string orderStatus)
        {
            if (orderStatus == OrdStatus.PARTIALLY_FILLED.ToString())
            {
                return "PARTIALLY_FILLED";
            }

            if (orderStatus == OrdStatus.ACCEPTED_FOR_BIDDING.ToString())
            {
                return "ACCEPTED_FOR_BIDDING";
            }

            if (orderStatus == OrdStatus.CANCELED.ToString())
            {
                return "CANCELED";
            }

            if (orderStatus == OrdStatus.DONE_FOR_DAY.ToString())
            {
                return "DONE_FOR_DAY";
            }

            if (orderStatus == OrdStatus.EXPIRED.ToString())
            {
                return "EXPIRED";
            }

            if (orderStatus == OrdStatus.CALCULATED.ToString())
            {
                return "CALCULATED";
            }

            if (orderStatus == OrdStatus.FILLED.ToString())
            {
                return "FILLED";
            }

            if (orderStatus == OrdStatus.NEW.ToString())
            {
                return "NEW";
            }

            if (orderStatus == OrdStatus.PENDING_CANCEL.ToString())
            {
                return "PENDING_CANCEL";
            }

            if (orderStatus == OrdStatus.PENDING_CANCELREPLACE.ToString())
            {
                return "PENDING_CANCEL_REPLACE";
            }

            if (orderStatus == OrdStatus.REJECTED.ToString())
            {
                return "REJECTED";
            }

            if (orderStatus == OrdStatus.REPLACED.ToString())
            {
                return "REPLACED";
            }

            if (orderStatus == OrdStatus.STOPPED.ToString())
            {
                return "STOPPED";
            }

            if (orderStatus == OrdStatus.SUSPENDED.ToString())
            {
                return "SUSPENDED";
            }

            return "UNKNOWN?";
        }

        /// <summary>
        /// The get fxcm order status.
        /// </summary>
        /// <param name="orderStatus">The order status.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string GetFxcmOrderStatus(string orderStatus)
        {
            switch (orderStatus)
            {
                case "W":
                    return "Waiting";
                case "P":
                    return "In_Process";
                case "I":
                    return "Dealer_Intervention";
                case "Q":
                    return "Requoted";
                case "E":
                    return "Executing";
                case "C":
                    return "Cancelled";
                case "R":
                    return "Rejected";
                case "T":
                    return "Expired";
                case "F":
                    return "Executed";
                case "U":
                    return "Pending_Calculated";
                case "S":
                    return "Pending_Cancel";
                default:
                    throw ExceptionFactory.InvalidSwitchArgument(orderStatus, nameof(orderStatus));
            }
        }

        /// <summary>
        /// The get order price.
        /// </summary>
        /// <param name="orderType">The order type.</param>
        /// <param name="stopPrice">The stop price.</param>
        /// <param name="limitPrice">The limit price.</param>
        /// <returns>A <see cref="decimal"/>.</returns>
        public static decimal GetOrderPrice(OrderType orderType, decimal stopPrice, decimal limitPrice)
        {
            if (orderType == OrderType.STOP_MARKET || orderType == OrderType.MIT)
            {
                return stopPrice;
            }

            return limitPrice;
        }

        /// <summary>
        /// The get date time from market data string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>The converted <see cref="ZonedDateTime"/>.</returns>
        public static ZonedDateTime ConvertMarketDataString(string dateTime) =>
            MarketDataParsePattern.Parse(dateTime).Value;

        /// <summary>
        /// The get date time from execution string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>The converted <see cref="ZonedDateTime"/>.</returns>
        public static ZonedDateTime ConvertExecutionReportString(string dateTime) =>
            ExecutionReportParsePattern.Parse(dateTime).Value;

        private static readonly ZonedDateTimePattern MarketDataParsePattern =
            ZonedDateTimePattern.CreateWithInvariantCulture(
                "yyyyMMddHH:mm:ss.fff",
                DateTimeZoneProviders.Tzdb);

        private static readonly ZonedDateTimePattern ExecutionReportParsePattern =
            ZonedDateTimePattern.CreateWithInvariantCulture(
                "yyyyMMdd-HH:mm:ss.fff",
                DateTimeZoneProviders.Tzdb);
    }
}
