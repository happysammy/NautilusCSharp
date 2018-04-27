// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixMessageHelper.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Nautilus.DomainModel.Enums;

    using NodaTime;
    using NodaTime.Text;

    using QuickFix.Fields;

    using SecurityType = Nautilus.DomainModel.Enums.SecurityType;
    using TimeInForce = Nautilus.DomainModel.Enums.TimeInForce;

    /// <summary>
    /// The FXCM quick fix message helper.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public static class FxcmFixMessageHelper
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
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetSecurityRequestResult(SecurityRequestResult result)
        {
            return SecurityRequestResults.ContainsKey(result.ToString()) ? SecurityRequestResults[result.ToString()] : "security request result unknown";
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
        /// <param name="rejectCode">
        /// The reject code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetCancelRejectReasonString(string rejectCode)
        {
            return CxlRejReasonStrings.ContainsKey(rejectCode) ? CxlRejReasonStrings[rejectCode] : CxlRejReasonStrings["99"];
        }

        private static readonly Dictionary<string, string> CxlRejResponse = new Dictionary<string, string>
        {
            { "1", "OrderCancel" },
            { "2", "OrderCancelRequest" },
        };

        /// <summary>
        /// Returns the cancel reject response to.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetCxlRejResponseTo(CxlRejResponseTo response)
        {
            return CxlRejResponse.ContainsKey(response.ToString()) ? CxlRejResponse[response.ToString()] : string.Empty;
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
        /// <param name="timeInForce">
        /// The time in force.
        /// </param>
        /// <returns>
        /// The <see cref="TimeInForce"/>.
        /// </returns>
        public static QuickFix.Fields.TimeInForce GetFixTimeInForce(TimeInForce timeInForce)
        {
            return TimeInForceIndex.ContainsKey(timeInForce) ? TimeInForceIndex[timeInForce] : null;
        }

        /// <summary>
        /// The get nautilus time in force.
        /// </summary>
        /// <param name="timeInForce">
        /// The time in force.
        /// </param>
        /// <returns>
        /// The <see cref="TimeInForce"/>.
        /// </returns>
        public static TimeInForce GetNautilusTimeInForce(string timeInForce)
        {
            return TimeInForceStringIndex.ContainsKey(timeInForce) ? TimeInForceStringIndex[timeInForce] : TimeInForce.Unknown;
        }

        /// <summary>
        /// The get security type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="SecurityType"/>.
        /// </returns>
        public static SecurityType GetSecurityType(string type)
        {
            // TODO
            if (type == "1")
            {
                return SecurityType.Forex;
            }

            return SecurityType.Cfd;
        }

        /// <summary>
        /// The get nautilus order side.
        /// </summary>
        /// <param name="side">
        /// The side.
        /// </param>
        /// <returns>
        /// The <see cref="OrderSide"/>.
        /// </returns>
        public static OrderSide GetNautilusOrderSide(string side)
        {
            if (side == Side.BUY.ToString())
            {
                return OrderSide.Buy;
            }

            if (side == Side.SELL.ToString())
            {
                return OrderSide.Sell;
            }

            return OrderSide.Undefined;
        }

        /// <summary>
        /// The get fix order side.
        /// </summary>
        /// <param name="orderSide">
        /// The order side.
        /// </param>
        /// <returns>
        /// The <see cref="Side"/>.
        /// </returns>
        public static Side GetFixOrderSide(OrderSide orderSide)
        {
            if (orderSide == OrderSide.Buy)
            {
                return new Side(Side.BUY);
            }

            return new Side(Side.SELL);
        }

        /// <summary>
        /// The get opposite fix order side.
        /// </summary>
        /// <param name="orderSide">
        /// The order side.
        /// </param>
        /// <returns>
        /// The <see cref="Side"/>.
        /// </returns>
        public static Side GetOppositeFixOrderSide(OrderSide orderSide)
        {
            if (orderSide == OrderSide.Buy)
            {
                return new Side(Side.SELL);
            }

            return new Side(Side.BUY);
        }

        /// <summary>
        /// The get nautilus order type.
        /// </summary>
        /// <param name="fixType">
        /// The fix type.
        /// </param>
        /// <returns>
        /// The <see cref="OrderType"/>.
        /// </returns>
        public static OrderType GetNautilusOrderType(string fixType)
        {
            if (fixType == OrdType.MARKET.ToString())
            {
                return OrderType.Market;
            }

            if (fixType == OrdType.STOP.ToString())
            {
                return OrderType.StopMarket;
            }

            if (fixType == OrdType.STOP_LIMIT.ToString())
            {
                return OrderType.StopLimit;
            }

            if (fixType == OrdType.LIMIT.ToString())
            {
                return OrderType.Limit;
            }

            if (fixType == OrdType.MARKET_IF_TOUCHED.ToString())
            {
                return OrderType.MarketIfTouched;
            }

            return OrderType.Unknown;
        }

        /// <summary>
        /// The get fix order type.
        /// </summary>
        /// <param name="orderType">
        /// The order type.
        /// </param>
        /// <returns>
        /// The <see cref="OrdType"/>.
        /// </returns>
        public static OrdType GetFixOrderType(OrderType orderType)
        {
            if (orderType == OrderType.Market)
            {
                return new OrdType(OrdType.MARKET);
            }

            if (orderType == OrderType.StopMarket)
            {
                return new OrdType(OrdType.STOP);
            }

            if (orderType == OrderType.StopLimit)
            {
                return new OrdType(OrdType.STOP_LIMIT);
            }

            if (orderType == OrderType.Limit)
            {
                return new OrdType(OrdType.LIMIT);
            }

            if (orderType == OrderType.MarketIfTouched)
            {
                return new OrdType(OrdType.MARKET_IF_TOUCHED);
            }

            return null;
        }

        /// <summary>
        /// The get fix order status.
        /// </summary>
        /// <param name="orderStatus">
        /// The order status.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
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
                return "PENDING_CANCELREPLACE";
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
        /// <param name="orderStatus">
        /// The order status.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetFxcmOrderStatus(string orderStatus)
        {
            if (orderStatus == "W")
            {
                return "Waiting";
            }

            if (orderStatus == "P")
            {
                return "In_Process";
            }

            if (orderStatus == "I")
            {
                return "Dealer_Intervention";
            }

            if (orderStatus == "Q")
            {
                return "Requoted";
            }

            if (orderStatus == "E")
            {
                return "Executing";
            }

            if (orderStatus == "C")
            {
                return "Cancelled";
            }

            if (orderStatus == "R")
            {
                return "Rejected";
            }

            if (orderStatus == "T")
            {
                return "Expired";
            }

            if (orderStatus == "F")
            {
                return "Executed";
            }

            if (orderStatus == "U")
            {
                return "Pending_Calculated";
            }

            if (orderStatus == "S")
            {
                return "Pending_Cancel";
            }

            return "Unknown?";
        }

        /// <summary>
        /// The get order price.
        /// </summary>
        /// <param name="orderType">
        /// The order type.
        /// </param>
        /// <param name="stopPrice">
        /// The stop price.
        /// </param>
        /// <param name="limitPrice">
        /// The limit price.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        public static decimal GetOrderPrice(OrderType orderType, decimal stopPrice, decimal limitPrice)
        {
            if (orderType == OrderType.StopMarket)
            {
                return stopPrice;
            }

            return limitPrice;
        }

        private static readonly ZonedDateTimePattern MarketDataParsePattern =
            ZonedDateTimePattern.CreateWithInvariantCulture(
                "yyyyMMddHH:mm:ss.fff",
                DateTimeZoneProviders.Tzdb);

        private static readonly ZonedDateTimePattern ExecutionReportParsePattern =
            ZonedDateTimePattern.CreateWithInvariantCulture(
                "yyyyMMdd-HH:mm:ss",
                DateTimeZoneProviders.Tzdb);


        /// <summary>
        /// The get date time from market data string.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="ZonedDateTime"/>.
        /// </returns>
        public static ZonedDateTime GetZonedDateTimeUtcFromMarketDataString(string dateTime) =>
            MarketDataParsePattern.Parse(dateTime).Value;

        /// <summary>
        /// The get date time from execution string.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        /// <returns>
        /// The <see cref="ZonedDateTime"/>.
        /// </returns>
        public static ZonedDateTime GetZonedDateTimeUtcFromExecutionReportString(string dateTime) =>
            ExecutionReportParsePattern.Parse(dateTime).Value;
    }
}
