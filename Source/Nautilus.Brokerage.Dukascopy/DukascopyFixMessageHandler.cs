//--------------------------------------------------------------------------------------------------
// <copyright file="DukascopyFixMessageHandler.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.Dukascopy
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix;
    using Nautilus.Fix.Interfaces;
    using NodaTime;
    using QuickFix;
    using QuickFix.Fields;
    using QuickFix.FIX44;
    using Currency = Nautilus.DomainModel.Enums.Currency;
    using Exception = System.Exception;
    using Symbol = Nautilus.DomainModel.ValueObjects.Symbol;

    /// <summary>
    /// Provides an implementation for handling Dukascopy FIX messages.
    /// </summary>
    public sealed class DukascopyFixMessageHandler : Component, IFixMessageHandler
    {
        private readonly SymbolConverter symbolConverter;

        private IFixGateway? fixGateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="DukascopyFixMessageHandler"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="symbolConverter">The symbol provider.</param>
        public DukascopyFixMessageHandler(
            IComponentryContainer container,
            SymbolConverter symbolConverter)
            : base(container)
        {
            this.symbolConverter = symbolConverter;
        }

        /// <summary>
        /// Initializes the execution gateway.
        /// </summary>
        /// <param name="gateway">The execution gateway.</param>
        public void InitializeGateway(IFixGateway gateway)
        {
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

                    var brokerSymbol = new BrokerSymbol(group.GetField(Tags.Symbol));

                    var symbolQuery = this.symbolConverter.GetNautilusSymbol(brokerSymbol.ToString());
                    if (symbolQuery.IsFailure)
                    {
                        this.Log.Warning(symbolQuery.Message);
                        continue;
                    }

                    var symbol = new Symbol(symbolQuery.Value, Venue.DUKASCOPY);
                    var instrumentId = new InstrumentId(symbol.ToString());
                    var quoteCurrency = message.GetField(Tags.Currency).ToEnum<Currency>();
                    var securityType = FixMessageHelper.GetSecurityType(group.GetField(9080));
                    var roundLot = Convert.ToInt32(group.GetField(561));
                    var tickPrecision = Convert.ToInt32(group.GetField(9001));

                    // Field 9002 gives 'point' size. Multiply by 0.1 to get tick size.
                    var tickSize = Convert.ToDecimal(group.GetField(9002)) * 0.1m;
                    var minStopDistanceEntry = Convert.ToInt32(group.GetField(9092));
                    var minLimitDistanceEntry = Convert.ToInt32(group.GetField(9093));
                    var minStopDistance = Convert.ToInt32(group.GetField(9090));
                    var minLimitDistance = Convert.ToInt32(group.GetField(9091));
                    var rolloverInterestBuy = Convert.ToDecimal(group.GetField(9003));
                    var rolloverInterestSell = Convert.ToDecimal(group.GetField(9004));
                    var minTradeSize = Convert.ToInt32(group.GetField(9095));
                    var maxTradeSize = Convert.ToInt32(group.GetField(9094));

                    var instrument = new Instrument(
                        instrumentId,
                        symbol,
                        brokerSymbol,
                        quoteCurrency,
                        securityType,
                        tickPrecision,
                        tickSize,
                        roundLot,
                        minStopDistanceEntry,
                        minLimitDistanceEntry,
                        minStopDistance,
                        minLimitDistance,
                        minTradeSize,
                        maxTradeSize,
                        rolloverInterestBuy,
                        rolloverInterestSell,
                        this.TimeNow());

                    instruments.Add(instrument);
                }

                var responseId = message.GetField(Tags.SecurityResponseID);
                var result = FixMessageHelper.GetSecurityRequestResult(message.SecurityRequestResult);

                this.fixGateway?.OnInstrumentsUpdate(instruments, responseId, result);
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
                var brokerSymbolString = message.GetField(Tags.Symbol);

                // var brokerSymbol = new BrokerSymbol(brokerSymbolString);
                var symbol = new Symbol(brokerSymbolString.Replace("/", string.Empty), Venue.DUKASCOPY);

                this.Log.Information($"QuoteStatusReport({symbol})");
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
                var inquiryId = message.GetField(Tags.CollInquiryID);
                var accountNumber = Convert.ToInt32(message.GetField(Tags.Account)).ToString();

                this.fixGateway?.OnCollateralInquiryAck(inquiryId, accountNumber);
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

                this.fixGateway?.OnAccountReport(
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
        /// <param name="message">The message.</param>
        public void OnMessage(RequestForPositionsAck message)
        {
            this.Execute(() =>
            {
                this.fixGateway?.OnRequestForPositionsAck(
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
                if (!message.IsSetField(Tags.Symbol))
                {
                    // Symbol is not set so return.
                    return;
                }

                var symbolQuery = this.symbolConverter.GetNautilusSymbol(message.GetField(Tags.Symbol));
                if (symbolQuery.IsFailure)
                {
                    this.Log.Error(symbolQuery.Message);
                    return; // Cannot get Nautilus symbol for broker symbol.
                }

                var symbolCode = symbolQuery.Value;

                var group = new MarketDataSnapshotFullRefresh.NoMDEntriesGroup();

                // var dateTimeString = group.GetField(Tags.MDEntryDate) + group.GetField(Tags.MDEntryTime);
                // var timestamp = FixMessageHelper.GetZonedDateTimeUtcFromMarketDataString(dateTimeString);
                message.GetGroup(1, group);
                var bid = group.GetField(Tags.MDEntryPx);

                message.GetGroup(2, group);
                var ask = group.GetField(Tags.MDEntryPx);

                var bidDecimal = decimal.Zero;
                var askDecimal = decimal.Zero;

                try
                {
                    bidDecimal = Convert.ToDecimal(bid);
                    askDecimal = Convert.ToDecimal(ask);
                }
                catch (Exception ex)
                {
                    if (ex is FormatException || ex is OverflowException)
                    {
                        this.Log.Error("Could not parse decimal.");
                        return;
                    }
                }

                this.fixGateway?.OnTick(
                    symbolCode,
                    Venue.FXCM,
                    bidDecimal,
                    askDecimal,
                    this.TimeNow());
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
                var orderId = message.ClOrdID.ToString();
                var fxcmCode = message.GetField(9025);
                var cancelRejectResponseTo = message.CxlRejResponseTo.ToString();
                var cancelRejectReason = $"{message.CxlRejReason}, {message.Text.ToString().TrimEnd('.')}, FXCMCode={fxcmCode}";
                var timestamp = FixMessageHelper.ConvertExecutionReportString(GetField(message, Tags.TransactTime));

                this.fixGateway?.OnOrderCancelReject(
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
                var brokerSymbol = message.GetField(Tags.Symbol);

                var symbol = message.IsSetField(Tags.Symbol)
                    ? this.symbolConverter.GetNautilusSymbol(brokerSymbol).Value
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
                var timestamp = FixMessageHelper.ConvertExecutionReportString(GetField(message, Tags.TransactTime));
                var orderQty = Convert.ToInt32(GetField(message, Tags.OrderQty));

                var orderStatus = message.GetField(Tags.OrdStatus);
                if (orderStatus == OrdStatus.REJECTED.ToString())
                {
                    var rejectReasonCode = message.GetField(9025);
                    var fxcmRejectCode = message.GetField(9029);
                    var rejectReasonText = GetField(message, Tags.CxlRejReason);
                    var rejectReason = $"Code({rejectReasonCode})={FixMessageHelper.GetCancelRejectReasonString(rejectReasonCode)}, FXCM({fxcmRejectCode})={rejectReasonText}";

                    this.fixGateway?.OnOrderRejected(
                        orderId,
                        rejectReason,
                        timestamp);
                }

                if (orderStatus == OrdStatus.CANCELED.ToString())
                {
                    this.fixGateway?.OnOrderCancelled(
                        orderId,
                        brokerOrderId,
                        orderLabel,
                        timestamp);
                }

                if (orderStatus == OrdStatus.REPLACED.ToString())
                {
                    this.fixGateway?.OnOrderModified(
                        orderId,
                        brokerOrderId,
                        orderLabel,
                        price,
                        timestamp);
                }

                if (orderStatus == OrdStatus.NEW.ToString())
                {
                    var expireTime = message.IsSetField(Tags.ExpireTime)
                        ? FixMessageHelper.ConvertExecutionReportString(message.GetField(Tags.ExpireTime))
                        : (ZonedDateTime?)null;

                    this.fixGateway?.OnOrderWorking(
                        orderId,
                        brokerOrderId,
                        symbol,
                        Venue.DUKASCOPY,
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
                    this.fixGateway?.OnOrderExpired(
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

                    this.fixGateway?.OnOrderFilled(
                        orderId,
                        brokerOrderId,
                        executionId,
                        executionTicket,
                        symbol,
                        Venue.DUKASCOPY,
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

                    this.fixGateway?.OnOrderPartiallyFilled(
                        orderId,
                        brokerOrderId,
                        executionId,
                        executionTicket,
                        symbol,
                        Venue.DUKASCOPY,
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
                this.fixGateway?.OnPositionReport(message.Account.ToString());
            });
        }

        private static string GetField(FieldMap report, int tag)
        {
            return report.IsSetField(tag)
                ? report.GetField(tag)
                : string.Empty;
        }
    }
}
