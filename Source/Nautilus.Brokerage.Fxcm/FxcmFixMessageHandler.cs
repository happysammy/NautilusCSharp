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
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Enums;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Validation;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix;
    using Nautilus.Fix.Interfaces;
    using NodaTime;
    using QuickFix;
    using QuickFix.Fields;
    using QuickFix.FIX44;
    using Symbol = Nautilus.DomainModel.ValueObjects.Symbol;

    /// <summary>
    /// Provides an implementation for handling FXCM FIX messages.
    /// </summary>
    public class FxcmFixMessageHandler : ComponentBase, IFixMessageHandler
    {
        private readonly InstrumentDataProvider instrumentData;

        private IFixGateway fixGateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="FxcmFixMessageHandler"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="instrumentData">The instrument data provider.</param>
        public FxcmFixMessageHandler(
            IComponentryContainer container,
            InstrumentDataProvider instrumentData)
            : base(
                NautilusService.FIX,
                LabelFactory.Component(nameof(FxcmFixMessageHandler)),
                container)
        {
            Validate.NotNull(container, nameof(container));
            Validate.NotNull(instrumentData, nameof(instrumentData));

            this.instrumentData = instrumentData;
        }

        /// <summary>
        /// Initializes the execution gateway.
        /// </summary>
        /// <param name="gateway">The execution gateway.</param>
        public void InitializeGateway(IFixGateway gateway)
        {
            Validate.NotNull(gateway, nameof(gateway));

            this.fixGateway = gateway;
        }

        /// <summary>
        /// Handles business message reject messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(BusinessMessageReject message)
        {
            this.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));

                this.Log.Debug($"BusinessMessageReject: {message}");
            });
        }

        /// <summary>
        /// Handles security list messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(SecurityList message)
        {
            this.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));

                var instruments = new List<Instrument>();
                var groupCount = Convert.ToInt32(message.NoRelatedSym.ToString());
                var group = new SecurityList.NoRelatedSymGroup();

                for (var i = 1; i <= groupCount; i++)
                {
                    message.GetGroup(i, group);

                    if (!group.IsSetField(Tags.Symbol))
                    {
                        // Symbol is not set so continue to next item.
                        continue;
                    }

                    var brokerSymbolString = group.GetField(Tags.Symbol);
                    var brokerSymbol = new BrokerSymbol(brokerSymbolString);

                    var symbolQuery = this.instrumentData.GetNautilusSymbol(brokerSymbol.Value);
                    if (symbolQuery.IsFailure)
                    {
                        this.Log.Error(symbolQuery.Message);
                        continue;
                    }

                    var symbol = new Symbol(symbolQuery.Value, Venue.FXCM);

                    var symbolId = new InstrumentId(symbol.ToString());
                    var quoteCurrency = group.GetField(15).ToEnum<CurrencyCode>();
                    var securityType = FixMessageHelper.GetSecurityType(group.GetField(9080));
                    var roundLot = Convert.ToInt32(group.GetField(561));
                    var tickDecimals = Convert.ToInt32(group.GetField(9001));

                    // Field 9002 gives 'point' size. Multiply by 0.1 to get tick size.
                    var tickSize = Convert.ToDecimal(group.GetField(9002)) * 0.1m;

                    var tickValueQuery = this.instrumentData.GetTickValue(brokerSymbol.Value);
                    if (tickValueQuery.IsFailure)
                    {
                        this.Log.Error(tickValueQuery.Message);
                        continue;
                    }

                    var tickValue = tickValueQuery.Value;

                    var targetDirectSpreadQuery = this.instrumentData.GetTargetDirectSpread(brokerSymbol.Value);
                    if (targetDirectSpreadQuery.IsFailure)
                    {
                        this.Log.Error(targetDirectSpreadQuery.Message);
                        continue;
                    }

                    var targetDirectSpread = targetDirectSpreadQuery.Value;

                    var contractSize = 1; // always 1 for FXCM
                    var minStopDistanceEntry = Convert.ToInt32(group.GetField(9092));
                    var minLimitDistanceEntry = Convert.ToInt32(group.GetField(9093));
                    var minStopDistance = Convert.ToInt32(group.GetField(9090));
                    var minLimitDistance = Convert.ToInt32(group.GetField(9091));
                    var rolloverInterestBuy = Convert.ToDecimal(group.GetField(9003));
                    var rolloverInterestSell = Convert.ToDecimal(group.GetField(9004));
                    var minTradeSize = Convert.ToInt32(group.GetField(9095));
                    var maxTradeSize = Convert.ToInt32(group.GetField(9094));

                    var instrument = new Instrument(
                        symbol,
                        symbolId,
                        brokerSymbol,
                        quoteCurrency,
                        securityType,
                        tickDecimals,
                        tickSize,
                        tickValue,
                        targetDirectSpread,
                        roundLot,
                        contractSize,
                        minStopDistanceEntry,
                        minLimitDistanceEntry,
                        minStopDistance,
                        minLimitDistance,
                        minTradeSize,
                        maxTradeSize,
                        this.instrumentData.GetMarginRequirement(brokerSymbolString).Value,
                        rolloverInterestBuy,
                        rolloverInterestSell,
                        this.TimeNow());

                    instruments.Add(instrument);
                }

                var responseId = message.GetField(Tags.SecurityResponseID);
                var result = FixMessageHelper.GetSecurityRequestResult(message.SecurityRequestResult);

                this.fixGateway.OnInstrumentsUpdate(instruments, responseId, result);
            });
        }

        /// <summary>
        /// Handles collateral inquiry acknowledgement messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(CollateralInquiryAck message)
        {
            this.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));

                var inquiryId = message.GetField(Tags.CollInquiryID);
                var accountNumber = Convert.ToInt32(message.GetField(Tags.Account)).ToString();

                this.fixGateway.OnCollateralInquiryAck(inquiryId, accountNumber);
            });
        }

        /// <summary>
        /// Handles collateral report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(CollateralReport message)
        {
            this.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));

                var inquiryId = message.GetField(Tags.CollRptID);
                var accountNumber = message.GetField(Tags.Account);
                var cashBalance = Convert.ToDecimal(message.GetField(Tags.CashOutstanding));
                var cashStart = Convert.ToDecimal(message.GetField(Tags.StartCash));
                var cashDaily = Convert.ToDecimal(message.GetField(9047));
                var marginUsedMaintenance = Convert.ToDecimal(message.GetField(9046));
                var marginUsedLiq = Convert.ToDecimal(message.GetField(9038));
                var marginRatio = Convert.ToDecimal(message.GetField(Tags.MarginRatio));
                var marginCall = message.GetField(9045);
                var time = this.TimeNow();

                this.fixGateway.OnAccountReport(
                    inquiryId,
                    accountNumber,
                    cashBalance,
                    cashStart,
                    cashDaily,
                    marginUsedMaintenance,
                    marginUsedLiq,
                    marginRatio,
                    marginCall,
                    time);
            });
        }

        /// <summary>
        /// Handles request for positions acknowledgement messages.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void OnMessage(RequestForPositionsAck message)
        {
            this.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));

                this.fixGateway.OnRequestForPositionsAck(
                    message.Account.ToString(),
                    message.PosReqID.ToString());
            });
        }

        /// <summary>
        /// Handles quote status report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(QuoteStatusReport message)
        {
            this.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));

                this.Log.Information(message.Product.ToString());
            });
        }

        /// <summary>
        /// Handles market data request reject messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(MarketDataRequestReject message)
        {
            this.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));

                this.Log.Warning($"MarketDataRequestReject: {message.GetField(Tags.Text)}");
            });
        }

        /// <summary>
        /// Handles market data snapshot full refresh messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(MarketDataSnapshotFullRefresh message)
        {
            this.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));

                if (!message.IsSetField(Tags.Symbol))
                {
                    // Symbol is not set so return.
                    return;
                }

                var brokerSymbol = message.GetField(Tags.Symbol);

                var symbolQuery = this.instrumentData.GetNautilusSymbol(brokerSymbol);
                if (symbolQuery.IsFailure)
                {
                    throw new InvalidOperationException(symbolQuery.Message);
                }

                var symbol = symbolQuery.Value;

                var group = new MarketDataSnapshotFullRefresh.NoMDEntriesGroup();

                message.GetGroup(1, group);
                var dateTimeString = group.GetField(Tags.MDEntryDate) + group.GetField(Tags.MDEntryTime);
                var timestamp = FixMessageHelper.GetZonedDateTimeUtcFromMarketDataString(dateTimeString);
                var bid = group.GetField(Tags.MDEntryPx);

                message.GetGroup(2, group);
                var ask = group.GetField(Tags.MDEntryPx);

                this.fixGateway.OnTick(
                    symbol,
                    Venue.FXCM,
                    Convert.ToDecimal(bid),
                    Convert.ToDecimal(ask),
                    timestamp);
            });
        }

        /// <summary>
        /// Handles order cancel reject messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(OrderCancelReject message)
        {
            this.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));

                var orderId = message.ClOrdID.ToString();
                var fxcmCode = message.GetField(9025);
                var cancelRejectResponseTo = message.CxlRejResponseTo.ToString();
                var cancelRejectReason = $"{message.CxlRejReason}, {message.Text.ToString().TrimEnd('.')}, FXCMCode={fxcmCode}";
                var timestamp = FixMessageHelper.GetZonedDateTimeUtcFromExecutionReportString(GetField(message, Tags.TransactTime));

                this.fixGateway.OnOrderCancelReject(
                    "NULL",
                    Venue.FXCM,
                    orderId,
                    cancelRejectResponseTo,
                    cancelRejectReason,
                    timestamp);
            });
        }

        /// <summary>
        /// Handles execution report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(ExecutionReport message)
        {
            this.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));

                var brokerSymbol = message.GetField(Tags.Symbol);

                var symbol = message.IsSetField(Tags.Symbol)
                    ? this.instrumentData.GetNautilusSymbol(brokerSymbol).Value
                    : string.Empty;

                var orderId = GetField(message, Tags.ClOrdID);
                var brokerOrderId = GetField(message, Tags.OrderID);
                var orderLabel = GetField(message, Tags.SecondaryClOrdID);
                var orderSide = FixMessageHelper.GetOrderSide(GetField(message, Tags.Side));
                var orderType = FixMessageHelper.GetOrderType(GetField(message, Tags.OrdType));
                var price = FixMessageHelper.GetOrderPrice(
                    orderType,
                    Convert.ToDecimal(GetField(message, Tags.StopPx)),
                    Convert.ToDecimal(GetField(message, Tags.Price)));
                var timeInForce = FixMessageHelper.GetTimeInForce(GetField(message, Tags.TimeInForce));
                var timestamp = FixMessageHelper.GetZonedDateTimeUtcFromExecutionReportString(GetField(message, Tags.TransactTime));
                var orderQty = Convert.ToInt32(GetField(message, Tags.OrderQty));

                var orderStatus = message.GetField(Tags.OrdStatus);
                if (orderStatus == OrdStatus.REJECTED.ToString())
                {
                    var rejectReasonCode = message.GetField(9025);
                    var fxcmRejectCode = message.GetField(9029);
                    var rejectReasonText = GetField(message, Tags.CxlRejReason);
                    var rejectReason = $"Code({rejectReasonCode})={FixMessageHelper.GetCancelRejectReasonString(rejectReasonCode)}, FXCM({fxcmRejectCode})={rejectReasonText}";

                    this.fixGateway.OnOrderRejected(
                        symbol,
                        Venue.FXCM,
                        orderId,
                        rejectReason,
                        timestamp);
                }

                if (orderStatus == OrdStatus.CANCELED.ToString())
                {
                    this.fixGateway.OnOrderCancelled(
                        symbol,
                        Venue.FXCM,
                        orderId,
                        brokerOrderId,
                        orderLabel,
                        timestamp);
                }

                if (orderStatus == OrdStatus.REPLACED.ToString())
                {
                    this.fixGateway.OnOrderModified(
                        symbol,
                        Venue.FXCM,
                        orderId,
                        brokerOrderId,
                        orderLabel,
                        price,
                        timestamp);
                }

                if (orderStatus == OrdStatus.NEW.ToString())
                {
                    var expireTime = message.IsSetField(Tags.ExpireTime)
                                       ? (ZonedDateTime?)FixMessageHelper.GetZonedDateTimeUtcFromExecutionReportString(message.GetField(Tags.ExpireTime))
                                       : null;

                    this.fixGateway.OnOrderWorking(
                        symbol,
                        Venue.FXCM,
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

                if (orderStatus == OrdStatus.EXPIRED.ToString())
                {
                    this.fixGateway.OnOrderExpired(
                        symbol,
                        Venue.FXCM,
                        orderId,
                        brokerOrderId,
                        orderLabel,
                        timestamp);
                }

                if (orderStatus == OrdStatus.FILLED.ToString())
                {
                    var executionId = GetField(message, Tags.ExecID);
                    var executionTicket = GetField(message, 9041);
                    var filledQuantity = Convert.ToInt32(GetField(message, Tags.CumQty));
                    var averagePrice = Convert.ToDecimal(GetField(message, Tags.AvgPx));

                    this.fixGateway.OnOrderFilled(
                        symbol,
                        Venue.FXCM,
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

                if (orderStatus == OrdStatus.PARTIALLY_FILLED.ToString())
                {
                    var executionId = GetField(message, Tags.ExecID);
                    var executionTicket = GetField(message, 9041);
                    var filledQuantity = Convert.ToInt32(GetField(message, Tags.CumQty));
                    var leavesQuantity = Convert.ToInt32(GetField(message, Tags.LeavesQty));
                    var averagePrice = Convert.ToDecimal(GetField(message, Tags.AvgPx));

                    this.fixGateway.OnOrderPartiallyFilled(
                        symbol,
                        Venue.FXCM,
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

                this.Log.Debug($"ExecutionReport(order_id={orderId}, status={message.GetField(Tags.OrdStatus)}).");
            });
        }

        /// <summary>
        /// Handles position report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnMessage(PositionReport message)
        {
            this.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));

                this.fixGateway.OnPositionReport(message.Account.ToString());
            });
        }

        private static string GetField(FieldMap report, int tag)
        {
            Debug.NotNull(report, nameof(report));

            return report.IsSetField(tag)
                ? report.GetField(tag)
                : string.Empty;
        }
    }
}
