//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixMessageHandler.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.FXCM
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.Common.Interfaces;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.Fix;
    using Nautilus.Fix.Interfaces;
    using NodaTime;
    using QuickFix;
    using QuickFix.Fields;
    using QuickFix.FIX44;
    using Symbol = DomainModel.ValueObjects.Symbol;

    /// <summary>
    /// The FXCM quick fix message handler.
    /// </summary>
    public class FxcmFixMessageHandler : ComponentBase, IFixMessageHandler
    {
        private readonly IReadOnlyDictionary<string, int> tickSizeIndex;
        private readonly ITickProcessor tickProcessor;
        private IBrokerageGateway brokerageGateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="FxcmFixMessageHandler"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="tickProcessor">The tick data processor.</param>
        public FxcmFixMessageHandler(
            IComponentryContainer container,
            ITickProcessor tickProcessor)
            : base(
                ServiceContext.FIX,
                LabelFactory.Component(nameof(FxcmFixMessageHandler)),
                container)
        {
            Validate.NotNull(tickProcessor, nameof(tickProcessor));

            this.tickSizeIndex = FxcmTickSizeProvider.GetIndex();
            this.tickProcessor = tickProcessor;
        }

        /// <summary>
        /// Initializes the brokerage gateway.
        /// </summary>
        /// <param name="gateway">The brokerage gateway.</param>
        public void InitializeBrokerageGateway(IBrokerageGateway gateway)
        {
            Validate.NotNull(gateway, nameof(gateway));

            this.brokerageGateway = gateway;
        }

        /// <summary>
        /// Handles business message reject messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(BusinessMessageReject message)
        {
            Debug.NotNull(message, nameof(message));

            this.Log.Debug($"BusinessMessageReject: {message}");
        }

        /// <summary>
        /// Handles security list messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(SecurityList message)
        {
            Debug.NotNull(message, nameof(message));

            var securityList = new List<Instrument>();
            var groupCount = Convert.ToInt32(message.NoRelatedSym.ToString());
            var group = new SecurityList.NoRelatedSymGroup();

            for (int i = 1; i <= groupCount; i++)
            {
                message.GetGroup(i, group);

                var symbol = group.IsSetField(Tags.Symbol)
                    ? new Symbol(FxcmSymbolProvider.GetNautilusSymbol(group.GetField(Tags.Symbol)).Value, Exchange.FXCM)
                    : new Symbol("AUDUSD", Exchange.FXCM);

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
        /// Handles collateral inquiry acknowledgement messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(CollateralInquiryAck message)
        {
            Debug.NotNull(message, nameof(message));

            var inquiryId = message.GetField(Tags.CollInquiryID);
            var accountNumber = Convert.ToInt32(message.GetField(Tags.Account)).ToString();

            this.brokerageGateway.OnCollateralInquiryAck(inquiryId, accountNumber);
        }

        /// <summary>
        /// Handles collateral report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(CollateralReport message)
        {
            Debug.NotNull(message, nameof(message));

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
        /// Handles request for positions acknowledgement messages.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void OnMessage(RequestForPositionsAck message)
        {
            Debug.NotNull(message, nameof(message));

            this.brokerageGateway.OnRequestForPositionsAck(
                message.Account.ToString(),
                message.PosReqID.ToString());
        }

        /// <summary>
        /// Handles market data request reject messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(MarketDataRequestReject message)
        {
            Debug.NotNull(message, nameof(message));

            this.Log.Warning($"MarketDataRequestReject: {message.GetField(Tags.Text)}");
        }

        /// <summary>
        /// Handles market data snapshot full refresh messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(MarketDataSnapshotFullRefresh message)
        {
            Debug.NotNull(message, nameof(message));

            var fxcmSymbol = message.GetField(Tags.Symbol);

            var symbol = message.IsSetField(Tags.Symbol)
                ? FxcmSymbolProvider.GetNautilusSymbol(fxcmSymbol).Value
                : string.Empty;

            var group = new MarketDataSnapshotFullRefresh.NoMDEntriesGroup();

            message.GetGroup(1, group);
            var dateTimeString = group.GetField(Tags.MDEntryDate) + group.GetField(Tags.MDEntryTime);
            var timestamp = FxcmFixMessageHelper.GetZonedDateTimeUtcFromMarketDataString(dateTimeString);
            var bid = group.GetField(Tags.MDEntryPx);

            message.GetGroup(2, group);
            var ask = group.GetField(Tags.MDEntryPx);

            // TODO: Hardcoded exchange.
            this.tickProcessor.OnTick(
                symbol,
                Exchange.FXCM,
                Convert.ToDecimal(bid),
                Convert.ToDecimal(ask),
                this.tickSizeIndex[fxcmSymbol],
                timestamp);
        }

        /// <summary>
        /// Handles order cancel reject messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(OrderCancelReject message)
        {
            Debug.NotNull(message, nameof(message));

            var symbol = FxcmSymbolProvider.GetNautilusSymbol(GetField(message, Tags.Symbol)).Value;
            var orderId = message.ClOrdID.ToString();
            var brokerOrderId = message.OrderID.ToString();
            var fxcmcode = message.GetField(9025);
            var cancelRejectResponseTo = message.CxlRejResponseTo.ToString();
            var cancelRejectReason = $"ReasonCode={message.CxlRejReason}, {message.Text}, FXCMCode={fxcmcode})";
            var timestamp = FxcmFixMessageHelper.GetZonedDateTimeUtcFromExecutionReportString(GetField(message, Tags.TransactTime));

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
        /// Handles execution report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here because the actual variable name used for FIX is 'clOrderId'.")]
        public void OnMessage(ExecutionReport message)
        {
            Debug.NotNull(message, nameof(message));

            var symbol = FxcmSymbolProvider.GetNautilusSymbol(GetField(message, Tags.Symbol)).Value;
            var orderId = GetField(message, Tags.ClOrdID);
            var brokerOrderId = GetField(message, Tags.OrderID);
            var orderLabel = GetField(message, Tags.SecondaryClOrdID);
            var orderSide = FxcmFixMessageHelper.GetNautilusOrderSide(GetField(message, Tags.Side));
            var orderType = FxcmFixMessageHelper.GetNautilusOrderType(GetField(message, Tags.OrdType));
            var price = FxcmFixMessageHelper.GetOrderPrice(
                orderType,
                Convert.ToDecimal(GetField(message, Tags.StopPx)),
                Convert.ToDecimal(GetField(message, Tags.Price)));
            var timeInForce = FxcmFixMessageHelper.GetNautilusTimeInForce(GetField(message, Tags.TimeInForce));
            var timestamp = FxcmFixMessageHelper.GetZonedDateTimeUtcFromExecutionReportString(GetField(message, Tags.TransactTime));
            var orderQty = Convert.ToInt32(GetField(message, Tags.OrderQty));

            if (message.GetField(Tags.OrdStatus) == OrdStatus.REJECTED.ToString())
            {
                var rejectReasonCode = message.GetField(9025);
                var fxcmRejectCode = message.GetField(9029);
                var rejectReasonText = GetField(message, Tags.CxlRejReason);
                var rejectReason = $"Code({rejectReasonCode})={FxcmFixMessageHelper.GetCancelRejectReasonString(rejectReasonCode)}, FXCM({fxcmRejectCode})={rejectReasonText}";

                this.brokerageGateway.OnOrderRejected(
                    symbol,
                    Exchange.FXCM,
                    orderId,
                    rejectReason,
                    timestamp);
            }

            if (message.GetField(Tags.OrdStatus) == OrdStatus.CANCELED.ToString())
            {
                this.brokerageGateway.OnOrderCancelled(
                    symbol,
                    Exchange.FXCM,
                    orderId,
                    brokerOrderId,
                    orderLabel,
                    timestamp);
            }

            if (message.GetField(Tags.OrdStatus) == OrdStatus.REPLACED.ToString())
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

            if (message.GetField(Tags.OrdStatus) == OrdStatus.NEW.ToString())
            {
                var expireTime = message.IsSetField(Tags.ExpireTime)
                                     ? (ZonedDateTime?)FxcmFixMessageHelper.GetZonedDateTimeUtcFromExecutionReportString(message.GetField(Tags.ExpireTime))
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

            if (message.GetField(Tags.OrdStatus) == OrdStatus.EXPIRED.ToString())
            {
                this.brokerageGateway.OnOrderExpired(
                    symbol,
                    Exchange.FXCM,
                    orderId,
                    brokerOrderId,
                    orderLabel,
                    timestamp);
            }

            if (message.GetField(Tags.OrdStatus) == OrdStatus.FILLED.ToString())
            {
                var executionId = GetField(message, Tags.ExecID);
                var executionTicket = GetField(message, 9041);
                var filledQuantity = Convert.ToInt32(GetField(message, Tags.CumQty));
                var averagePrice = Convert.ToDecimal(GetField(message, Tags.AvgPx));

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

            if (message.GetField(Tags.OrdStatus) == OrdStatus.PARTIALLY_FILLED.ToString())
            {
                var executionId = GetField(message, Tags.ExecID);
                var executionTicket = GetField(message, 9041);
                var filledQuantity = Convert.ToInt32(GetField(message, Tags.CumQty));
                var leavesQuantity = Convert.ToInt32(GetField(message, Tags.LeavesQty));
                var averagePrice = Convert.ToDecimal(GetField(message, Tags.AvgPx));

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
        /// Handles position report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(PositionReport message)
        {
            Debug.NotNull(message, nameof(message));

            this.brokerageGateway.OnPositionReport(message.Account.ToString());
        }

        private static string GetField(FieldMap report, int tag)
        {
            Debug.NotNull(report, nameof(report));

            return report.IsSetField(tag) ? report.GetField(tag) : string.Empty;
        }
    }
}
