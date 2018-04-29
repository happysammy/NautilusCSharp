//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixMessageHandler.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fix
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using NautechSystems.CSharp.Extensions;
    using NautechSystems.CSharp.Validation;
    using Nautilus.BlackBox.Core.Interfaces;
    using Nautilus.Brokerage.FXCM;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using NodaTime;
    using QuickFix;
    using QuickFix.Fields;
    using QuickFix.FIX44;
    using Symbol = DomainModel.ValueObjects.Symbol;

    /// <summary>
    /// The FXCM quick fix message handler.
    /// </summary>
    public class FxcmFixMessageHandler
    {
        private IBrokerageGateway brokerageGateway;

        /// <summary>
        /// The initialize brokerage gateway.
        /// </summary>
        /// <param name="gateway">
        /// The gateway.
        /// </param>
        public void InitializeBrokerageGateway(IBrokerageGateway gateway)
        {
            Validate.NotNull(gateway, nameof(gateway));

            this.brokerageGateway = gateway;
        }

        /// <summary>
        /// The on business message reject.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void OnBusinessMessageReject(BusinessMessageReject message)
        {
            Validate.NotNull(message, nameof(message));

            this.brokerageGateway.OnBusinessMessage(message.Text.ToString());
        }

        /// <summary>
        /// The on security list.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void OnSecurityList(SecurityList message)
        {
            Validate.NotNull(message, nameof(message));

            var securityList = new List<Instrument>();
            var groupCount = Convert.ToInt32(message.NoRelatedSym.ToString());
            var group = new SecurityList.NoRelatedSymGroup();

            for (int i = 1; i <= groupCount; i++)
            {
                message.GetGroup(i, group);

                var symbol = group.IsSetField(Tags.Symbol) ? new Symbol(FxcmSymbolMapper.GetNautilusSymbol(group.GetField(Tags.Symbol)).Value, Exchange.FXCM) : new Symbol("AUDUSD", Exchange.FXCM);
                var symbolId = new EntityId(symbol.ToString());
                var brokerSymbol = new EntityId(group.GetField(Tags.Symbol));
                var quoteCurrency = group.GetField(15).ToEnum<CurrencyCode>();
                var securityType = FxcmFixMessageHelper.GetSecurityType(group.GetField(9080));
                var roundLot = Convert.ToInt32(group.GetField(561)); // TODO what is this??
                var tickSize = Convert.ToInt32(group.GetField(9001)).ToTickSize();
                var tickValueQuoteCurrency = FxcmTickValueProvider.GetTickValue(brokerSymbol.ToString());
                var targetDirectSpread = FxcmTargetDirectSpreadProvider.GetTargetDirectSpread(brokerSymbol.ToString());
                var contractSize = 1; // always 1 for FXCM
                var minStopDistanceEntry = Convert.ToInt32(group.GetField(9092));
                var minLimitDistanceEntry = Convert.ToInt32(group.GetField(9093));
                var minStopDistance = Convert.ToInt32(group.GetField(9090));
                var minLimitDistance = Convert.ToInt32(group.GetField(9091));
                var interestBuy = Convert.ToDecimal(group.GetField(9003));
                var interestSell = Convert.ToDecimal(group.GetField(9004));
                var minTradeSize = Convert.ToInt32(group.GetField(9095));
                var maxTradeSize = Convert.ToInt32(group.GetField(9094));

                var instrument = new Instrument(
                    symbol,
                    symbolId,
                    brokerSymbol,
                    quoteCurrency,
                    securityType,
                    tickSize,
                    tickValueQuoteCurrency.Value,
                    targetDirectSpread.Value,
                    contractSize,
                    minStopDistanceEntry,
                    minLimitDistanceEntry,
                    minStopDistance,
                    minLimitDistance,
                    minTradeSize,
                    maxTradeSize,
                    decimal.Zero, // TODO margin requirement
                    interestBuy,
                    interestSell,
                    this.brokerageGateway.GetTimeNow());

                securityList.Add(instrument);
            }

            var responseId = message.GetField(Tags.SecurityResponseID);
            var result = FxcmFixMessageHelper.GetSecurityRequestResult(message.SecurityRequestResult);

            this.brokerageGateway.OnInstrumentsUpdate(securityList, responseId, result);
        }

        /// <summary>
        /// The on collateral inquiry acknowledgement.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void OnCollateralInquiryAck(CollateralInquiryAck message)
        {
            Validate.NotNull(message, nameof(message));

            var inquiryId = message.GetField(Tags.CollInquiryID);
            var accountNumber = Convert.ToInt32(message.GetField(Tags.Account)).ToString();

            this.brokerageGateway.OnCollateralInquiryAck(inquiryId, accountNumber);
        }

        /// <summary>
        /// The on collateral report.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void OnCollateralReport(CollateralReport message)
        {
            Validate.NotNull(message, nameof(message));

            var inquiryId = message.GetField(Tags.CollRptID);
            var accountNumber = message.GetField(Tags.Account);
            var cashBalance = Convert.ToDecimal(message.GetField(Tags.CashOutstanding));
            var cashStart = Convert.ToDecimal(message.GetField(Tags.StartCash));
            var cashDaily = Convert.ToDecimal(message.GetField(9047));
            var marginUsedMaint = Convert.ToDecimal(message.GetField(9046));
            var marginUsedLiq = Convert.ToDecimal(message.GetField(9038));
            var marginRatio = Convert.ToDecimal(message.GetField(Tags.MarginRatio));
            var marginCall = message.GetField(9045);
            var time = this.brokerageGateway.GetTimeNow(); // TODO: Replace with message time.

            this.brokerageGateway.OnAccountReport(
                inquiryId,
                accountNumber,
                cashBalance,
                cashStart,
                cashDaily,
                marginUsedMaint,
                marginUsedLiq,
                marginRatio,
                marginCall,
                time);
        }

        /// <summary>
        /// The on request for positions acknowledgement.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void OnRequestForPositionsAck(RequestForPositionsAck message)
        {
            Validate.NotNull(message, nameof(message));

            this.brokerageGateway.OnRequestForPositionsAck(message.Account.ToString(), message.PosReqID.ToString());
        }

        /// <summary>
        /// The on market data snapshot full refresh.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void OnMarketDataSnapshotFullRefresh(MarketDataSnapshotFullRefresh message)
        {
            Validate.NotNull(message, nameof(message));

            var symbol = message.IsSetField(Tags.Symbol)
                ? FxcmSymbolMapper.GetNautilusSymbol(message.GetField(Tags.Symbol)).Value
                : string.Empty;

            var group = new MarketDataSnapshotFullRefresh.NoMDEntriesGroup();

            message.GetGroup(1, group);
            var dateTimeString = group.GetField(Tags.MDEntryDate) + group.GetField(Tags.MDEntryTime);
            var timestamp = FxcmFixMessageHelper.GetZonedDateTimeUtcFromMarketDataString(dateTimeString);
            var bid = group.GetField(Tags.MDEntryPx);

            message.GetGroup(2, group);
            var ask = group.GetField(Tags.MDEntryPx);

            this.brokerageGateway.OnQuote(
                symbol,
                Exchange.FXCM,
                Convert.ToDecimal(bid),
                Convert.ToDecimal(ask),
                timestamp);
        }

        /// <summary>
        /// The on order cancel reject.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void OnOrderCancelReject(OrderCancelReject message)
        {
            Validate.NotNull(message, nameof(message));

            var symbol = FxcmSymbolMapper.GetNautilusSymbol(GetMessageField(message, Tags.Symbol)).Value;
            var orderId = message.ClOrdID.ToString();
            var brokerOrderId = message.OrderID.ToString();
            var fxcmcode = message.GetField(9025);
            var cancelRejectResponseTo = message.CxlRejResponseTo.ToString();
            var cancelRejectReason = $"ReasonCode={message.CxlRejReason}, {message.Text}, FXCMCode={fxcmcode})";
            var timestamp = FxcmFixMessageHelper.GetZonedDateTimeUtcFromExecutionReportString(GetMessageField(message, Tags.TransactTime));

            this.brokerageGateway.OnOrderCancelReject(
                symbol,
                Exchange.FXCM,
                orderId,
                brokerOrderId,
                cancelRejectResponseTo,
                cancelRejectReason,
                timestamp);
        }

        /// <summary>
        /// The on execution report.
        /// </summary>
        /// <param name="report">
        /// The report.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here because the actual variable name used for FIX is 'clOrderId'.")]
        public void OnExecutionReport(ExecutionReport report)
        {
            Validate.NotNull(report, nameof(report));

            var symbol = FxcmSymbolMapper.GetNautilusSymbol(GetMessageField(report, Tags.Symbol)).Value;
            var orderId = GetMessageField(report, Tags.ClOrdID);
            var brokerOrderId = GetMessageField(report, Tags.OrderID);
            var orderLabel = GetMessageField(report, Tags.SecondaryClOrdID);
            var orderSide = FxcmFixMessageHelper.GetNautilusOrderSide(GetMessageField(report, Tags.Side));
            var orderType = FxcmFixMessageHelper.GetNautilusOrderType(GetMessageField(report, Tags.OrdType));
            var price = FxcmFixMessageHelper.GetOrderPrice(
                orderType,
                Convert.ToDecimal(GetMessageField(report, Tags.StopPx)),
                Convert.ToDecimal(GetMessageField(report, Tags.Price)));
            var timeInForce = FxcmFixMessageHelper.GetNautilusTimeInForce(GetMessageField(report, Tags.TimeInForce));
            var timestamp = FxcmFixMessageHelper.GetZonedDateTimeUtcFromExecutionReportString(GetMessageField(report, Tags.TransactTime));
            var orderQty = Convert.ToInt32(GetMessageField(report, Tags.OrderQty));

            if (report.GetField(Tags.OrdStatus) == OrdStatus.REJECTED.ToString())
            {
                var rejectReasonCode = report.GetField(9025);
                var fxcmRejectCode = report.GetField(9029);
                var rejectReasonText = GetMessageField(report, Tags.CxlRejReason);
                var rejectReason = $"Code({rejectReasonCode})={FxcmFixMessageHelper.GetCancelRejectReasonString(rejectReasonCode)}, FXCM({fxcmRejectCode})={rejectReasonText}";

                this.brokerageGateway.OnOrderRejected(
                    symbol,
                    Exchange.FXCM,
                    orderId,
                    rejectReason,
                    timestamp);
            }

            if (report.GetField(Tags.OrdStatus) == OrdStatus.CANCELED.ToString())
            {
                this.brokerageGateway.OnOrderCancelled(
                    symbol,
                    Exchange.FXCM,
                    orderId,
                    brokerOrderId,
                    orderLabel,
                    timestamp);
            }

            if (report.GetField(Tags.OrdStatus) == OrdStatus.REPLACED.ToString())
            {
                this.brokerageGateway.OnOrderModified(
                    symbol,
                    Exchange.FXCM,
                    orderId,
                    brokerOrderId,
                    orderLabel,
                    price,
                    timestamp);
            }

            if (report.GetField(Tags.OrdStatus) == OrdStatus.NEW.ToString())
            {
                var expireTime = report.IsSetField(Tags.ExpireTime)
                                     ? (ZonedDateTime?)FxcmFixMessageHelper.GetZonedDateTimeUtcFromExecutionReportString(report.GetField(Tags.ExpireTime))
                                     : null;

                this.brokerageGateway.OnOrderWorking(
                    symbol,
                    Exchange.FXCM,
                    orderId,
                    brokerOrderId,
                    orderLabel,
                    orderSide,
                    orderType,
                    orderQty,
                    price,
                    timeInForce,
                    expireTime,
                    timestamp);
            }

            if (report.GetField(Tags.OrdStatus) == OrdStatus.EXPIRED.ToString())
            {
                this.brokerageGateway.OnOrderExpired(
                    symbol,
                    Exchange.FXCM,
                    orderId,
                    brokerOrderId,
                    orderLabel,
                    timestamp);
            }

            if (report.GetField(Tags.OrdStatus) == OrdStatus.FILLED.ToString())
            {
                var executionId = GetMessageField(report, Tags.ExecID);
                var executionTicket = GetMessageField(report, 9041);
                var filledQuantity = Convert.ToInt32(GetMessageField(report, Tags.CumQty));
                var averagePrice = Convert.ToDecimal(GetMessageField(report, Tags.AvgPx));

                this.brokerageGateway.OnOrderFilled(
                    symbol,
                    Exchange.FXCM,
                    orderId,
                    brokerOrderId,
                    executionId,
                    executionTicket,
                    orderLabel,
                    orderSide,
                    filledQuantity,
                    averagePrice,
                    timestamp);
            }

            if (report.GetField(Tags.OrdStatus) == OrdStatus.PARTIALLY_FILLED.ToString())
            {
                var executionId = GetMessageField(report, Tags.ExecID);
                var executionTicket = GetMessageField(report, 9041);
                var filledQuantity = Convert.ToInt32(GetMessageField(report, Tags.CumQty));
                var leavesQuantity = Convert.ToInt32(GetMessageField(report, Tags.LeavesQty));
                var averagePrice = Convert.ToDecimal(GetMessageField(report, Tags.AvgPx));

                this.brokerageGateway.OnOrderPartiallyFilled(
                    symbol,
                    Exchange.FXCM,
                    orderId,
                    brokerOrderId,
                    executionId,
                    executionTicket,
                    orderLabel,
                    orderSide,
                    filledQuantity,
                    leavesQuantity,
                    averagePrice,
                    timestamp);
            }
        }

        /// <summary>
        /// The on position report.
        /// </summary>
        /// <param name="report">
        /// The message.
        /// </param>
        public void OnPositionReport(PositionReport report)
        {
            Validate.NotNull(report, nameof(report));

            this.brokerageGateway.OnPositionReport(report.Account.ToString());
        }

        private static string GetMessageField(FieldMap report, int tag)
        {
            Debug.NotNull(report, nameof(report));

            return report.IsSetField(tag) ? report.GetField(tag) : string.Empty;
        }
    }
}
