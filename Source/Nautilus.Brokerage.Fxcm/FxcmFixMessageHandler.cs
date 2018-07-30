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
    using Nautilus.DomainModel.Identifiers;
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
        private readonly IReadOnlyDictionary<string, int> pricePrecisionIndex;
        private readonly ITickProcessor tickProcessor;

        private IExecutionGateway executionGateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="FxcmFixMessageHandler"/> class.
        /// </summary>
        /// <param name="container">The setup container.</param>
        /// <param name="tickProcessor">The tick data processor.</param>
        public FxcmFixMessageHandler(
            IComponentryContainer container,
            ITickProcessor tickProcessor)
            : base(
                NautilusService.FIX,
                LabelFactory.Component(nameof(FxcmFixMessageHandler)),
                container)
        {
            Validate.NotNull(tickProcessor, nameof(tickProcessor));

            this.pricePrecisionIndex = FxcmPricePrecisionProvider.GetIndex();
            this.tickProcessor = tickProcessor;
        }

        /// <summary>
        /// Initializes the execution gateway.
        /// </summary>
        /// <param name="gateway">The execution gateway.</param>
        public void InitializeGateway(IExecutionGateway gateway)
        {
            Validate.NotNull(gateway, nameof(gateway));

            this.executionGateway = gateway;
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

                for (int i = 1; i <= groupCount; i++)
                {
                    message.GetGroup(i, group);

                    if (!group.IsSetField(Tags.Symbol))
                    {
                        // Symbol is not set so continue to next item.
                        continue;
                    }

                    var symbolQuery = FxcmSymbolProvider.GetNautilusSymbol(group.GetField(Tags.Symbol));
                    if (symbolQuery.IsFailure)
                    {
                        throw new InvalidOperationException($"Cannot find symbol for {group.GetField(Tags.Symbol)}");
                    }

                    var symbol = new Symbol(symbolQuery.Value, Venue.FXCM);
                    var symbolId = new InstrumentId(symbol.ToString());
                    var brokerSymbol = new ValidString(group.GetField(Tags.Symbol));
                    var quoteCurrency = group.GetField(15).ToEnum<CurrencyCode>();
                    var securityType = FixMessageHelper.GetSecurityType(group.GetField(9080));
                    //var roundLot = Convert.ToInt32(group.GetField(561)); // TODO what is this??
                    var tickDecimals = Convert.ToInt32(group.GetField(9001));
                    var tickSize = Convert.ToDecimal(group.GetField(9002));

                    var tickValueQuery = FxcmTickValueProvider.GetTickValue(brokerSymbol.ToString());
                    if (tickValueQuery.IsFailure)
                    {
                        throw new InvalidOperationException($"Cannot find tick value for {group.GetField(Tags.Symbol)}");
                    }
                    var tickValue = tickValueQuery.Value;

                    var targetDirectSpreadQuery = FxcmTargetDirectSpreadProvider.GetTargetDirectSpread(brokerSymbol.ToString());
                    if (targetDirectSpreadQuery.IsFailure)
                    {
                        throw new InvalidOperationException($"Cannot find target direct spread for {group.GetField(Tags.Symbol)}");
                    }
                    var targetDirectSpread = targetDirectSpreadQuery.Value;

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
                        tickDecimals,
                        tickSize,
                        tickValue,
                        targetDirectSpread,
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
                        this.TimeNow());

                    instruments.Add(instrument);
                }

                var responseId = message.GetField(Tags.SecurityResponseID);
                var result = FixMessageHelper.GetSecurityRequestResult(message.SecurityRequestResult);

                this.executionGateway.OnInstrumentsUpdate(instruments, responseId, result);
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

                this.executionGateway.OnCollateralInquiryAck(inquiryId, accountNumber);
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
                var marginUsedMaint = Convert.ToDecimal(message.GetField(9046));
                var marginUsedLiq = Convert.ToDecimal(message.GetField(9038));
                var marginRatio = Convert.ToDecimal(message.GetField(Tags.MarginRatio));
                var marginCall = message.GetField(9045);
                var time = this.TimeNow(); // TODO: Replace with message time.

                this.executionGateway.OnAccountReport(
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

                this.executionGateway.OnRequestForPositionsAck(
                    message.Account.ToString(),
                    message.PosReqID.ToString());
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

                var fxcmSymbol = message.GetField(Tags.Symbol);

                var symbol = message.IsSetField(Tags.Symbol)
                    ? FxcmSymbolProvider.GetNautilusSymbol(fxcmSymbol).Value
                    : "NONE";

                var group = new MarketDataSnapshotFullRefresh.NoMDEntriesGroup();

                message.GetGroup(1, group);
                var dateTimeString = group.GetField(Tags.MDEntryDate) + group.GetField(Tags.MDEntryTime);
                var timestamp = FixMessageHelper.GetZonedDateTimeUtcFromMarketDataString(dateTimeString);
                var bid = group.GetField(Tags.MDEntryPx);

                message.GetGroup(2, group);
                var ask = group.GetField(Tags.MDEntryPx);

                this.tickProcessor.OnTick(
                    symbol,
                    Venue.FXCM,
                    Convert.ToDecimal(bid),
                    Convert.ToDecimal(ask),
                    this.pricePrecisionIndex[fxcmSymbol],
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

                var symbol = FxcmSymbolProvider.GetNautilusSymbol(GetField(message, Tags.Symbol)).Value;
                var orderId = message.ClOrdID.ToString();
                var brokerOrderId = message.OrderID.ToString();
                var fxcmcode = message.GetField(9025);
                var cancelRejectResponseTo = message.CxlRejResponseTo.ToString();
                var cancelRejectReason = $"ReasonCode={message.CxlRejReason}, {message.Text}, FXCMCode={fxcmcode})";
                var timestamp = FixMessageHelper.GetZonedDateTimeUtcFromExecutionReportString(GetField(message, Tags.TransactTime));

                this.executionGateway.OnOrderCancelReject(
                    symbol,
                    Venue.FXCM,
                    orderId,
                    brokerOrderId,
                    cancelRejectResponseTo,
                    cancelRejectReason,
                    timestamp);
            });
        }

        /// <summary>
        /// Handles execution report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here because the actual variable name used for FIX is 'clOrderId'.")]
        public void OnMessage(ExecutionReport message)
        {
            this.Execute(() =>
            {
                Validate.NotNull(message, nameof(message));

                var fxcmSymbol = message.GetField(Tags.Symbol);

                var symbol = message.IsSetField(Tags.Symbol)
                    ? FxcmSymbolProvider.GetNautilusSymbol(fxcmSymbol).Value
                    : string.Empty;

                var orderId = GetField(message, Tags.ClOrdID);
                var brokerOrderId = GetField(message, Tags.OrderID);
                var orderLabel = GetField(message, Tags.SecondaryClOrdID);
                var orderSide = FixMessageHelper.GetNautilusOrderSide(GetField(message, Tags.Side));
                var orderType = FixMessageHelper.GetNautilusOrderType(GetField(message, Tags.OrdType));
                var price = FixMessageHelper.GetOrderPrice(
                    orderType,
                    Convert.ToDecimal(GetField(message, Tags.StopPx)),
                    Convert.ToDecimal(GetField(message, Tags.Price)));
                var timeInForce = FixMessageHelper.GetNautilusTimeInForce(GetField(message, Tags.TimeInForce));
                var timestamp = FixMessageHelper.GetZonedDateTimeUtcFromExecutionReportString(GetField(message, Tags.TransactTime));
                var orderQty = Convert.ToInt32(GetField(message, Tags.OrderQty));

                if (message.GetField(Tags.OrdStatus) == OrdStatus.REJECTED.ToString())
                {
                    var rejectReasonCode = message.GetField(9025);
                    var fxcmRejectCode = message.GetField(9029);
                    var rejectReasonText = GetField(message, Tags.CxlRejReason);
                    var rejectReason = $"Code({rejectReasonCode})={FixMessageHelper.GetCancelRejectReasonString(rejectReasonCode)}, FXCM({fxcmRejectCode})={rejectReasonText}";

                    this.executionGateway.OnOrderRejected(
                        symbol,
                        Venue.FXCM,
                        orderId,
                        rejectReason,
                        timestamp);
                }

                if (message.GetField(Tags.OrdStatus) == OrdStatus.CANCELED.ToString())
                {
                    this.executionGateway.OnOrderCancelled(
                        symbol,
                        Venue.FXCM,
                        orderId,
                        brokerOrderId,
                        orderLabel,
                        timestamp);
                }

                if (message.GetField(Tags.OrdStatus) == OrdStatus.REPLACED.ToString())
                {
                    this.executionGateway.OnOrderModified(
                        symbol,
                        Venue.FXCM,
                        orderId,
                        brokerOrderId,
                        orderLabel,
                        price,
                        this.pricePrecisionIndex[fxcmSymbol],
                        timestamp);
                }

                if (message.GetField(Tags.OrdStatus) == OrdStatus.NEW.ToString())
                {
                    var expireTime = message.IsSetField(Tags.ExpireTime)
                                       ? (ZonedDateTime?)FixMessageHelper.GetZonedDateTimeUtcFromExecutionReportString(message.GetField(Tags.ExpireTime))
                                       : null;

                    this.executionGateway.OnOrderWorking(
                        symbol,
                        Venue.FXCM,
                        orderId,
                        brokerOrderId,
                        orderLabel,
                        orderSide,
                        orderType,
                        orderQty,
                        price,
                        this.pricePrecisionIndex[fxcmSymbol],
                        timeInForce,
                        expireTime,
                        timestamp);
                }

                if (message.GetField(Tags.OrdStatus) == OrdStatus.EXPIRED.ToString())
                {
                    this.executionGateway.OnOrderExpired(
                        symbol,
                        Venue.FXCM,
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

                    this.executionGateway.OnOrderFilled(
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
                        this.pricePrecisionIndex[fxcmSymbol],
                        timestamp);
                }

                if (message.GetField(Tags.OrdStatus) == OrdStatus.PARTIALLY_FILLED.ToString())
                {
                    var executionId = GetField(message, Tags.ExecID);
                    var executionTicket = GetField(message, 9041);
                    var filledQuantity = Convert.ToInt32(GetField(message, Tags.CumQty));
                    var leavesQuantity = Convert.ToInt32(GetField(message, Tags.LeavesQty));
                    var averagePrice = Convert.ToDecimal(GetField(message, Tags.AvgPx));

                    this.executionGateway.OnOrderPartiallyFilled(
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
                        this.pricePrecisionIndex[fxcmSymbol],
                        timestamp);
                }
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

                this.executionGateway.OnPositionReport(message.Account.ToString());
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
