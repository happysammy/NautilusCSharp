//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixMessageHandler.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Brokerage.Fxcm
{
    using System;
    using System.Collections.Generic;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Core.Annotations;
    using Nautilus.Core.Correctness;
    using Nautilus.Core.Extensions;
    using Nautilus.Core.Types;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Events;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Fix;
    using Nautilus.Fix.Interfaces;
    using Nautilus.Messaging.Interfaces;
    using NodaTime;
    using QuickFix;
    using QuickFix.Fields;
    using QuickFix.FIX44;
    using Currency = Nautilus.DomainModel.Enums.Currency;
    using OrderCancelReject = Nautilus.DomainModel.Events.OrderCancelReject;
    using Price = Nautilus.DomainModel.ValueObjects.Price;
    using Quantity = Nautilus.DomainModel.ValueObjects.Quantity;
    using Symbol = Nautilus.DomainModel.Identifiers.Symbol;

    /// <summary>
    /// Provides an implementation for handling FXCM FIX messages.
    /// </summary>
    public sealed class FxcmFixMessageHandler : Component, IFixMessageHandler
    {
        private readonly AccountId accountId;
        private readonly Currency accountCurrency;
        private readonly Venue venue = new Venue("FXCM");
        private readonly SymbolConverter symbolConverter;
        private readonly ObjectCache<string, Symbol> symbolCache;

        private IDataGateway? dataGateway;
        private IEndpoint? tradingGateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="FxcmFixMessageHandler"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="accountId">The account identifier for the handler.</param>
        /// <param name="accountCurrency">The account currency.</param>
        /// <param name="symbolConverter">The instrument data provider.</param>
        public FxcmFixMessageHandler(
            IComponentryContainer container,
            AccountId accountId,
            Currency accountCurrency,
            SymbolConverter symbolConverter)
            : base(container)
        {
            this.accountId = accountId;
            this.accountCurrency = accountCurrency;
            this.symbolConverter = symbolConverter;
            this.symbolCache = new ObjectCache<string, Symbol>(Symbol.FromString);
        }

        /// <summary>
        /// Initializes the FIX data gateway.
        /// </summary>
        /// <param name="gateway">The execution gateway.</param>
        public void InitializeGateway(IDataGateway gateway)
        {
            this.dataGateway = gateway;
        }

        /// <summary>
        /// Initializes the FIX trading gateway.
        /// </summary>
        /// <param name="gateway">The trading gateway.</param>
        public void InitializeGateway(IEndpoint gateway)
        {
            this.tradingGateway = gateway;
        }

        /// <summary>
        /// Handles collateral inquiry acknowledgement messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SystemBoundary]
        public void OnMessage(CollateralInquiryAck message)
        {
            Debug.NotNull(this.tradingGateway, nameof(this.tradingGateway));

            this.Execute(() =>
            {
                var inquiryId = message.GetField(Tags.CollInquiryID);
                var accountNumber = Convert.ToInt32(message.GetField(Tags.Account)).ToString();

                this.Log.Verbose($"Received {message}");
            });
        }

        /// <summary>
        /// Handles business message reject messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SystemBoundary]
        public void OnMessage(BusinessMessageReject message)
        {
            this.Execute(() =>
            {
                this.Log.Verbose($"Received {message}");
            });
        }

        /// <summary>
        /// Handles position report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SystemBoundary]
        public void OnMessage(PositionReport message)
        {
            this.Execute(() =>
            {
                Condition.NotEmptyOrWhiteSpace(message.Account.ToString(), nameof(message.Account));

                this.Log.Debug($"PositionReport: ({message.Account})");
            });
        }

        /// <summary>
        /// Handles request for positions acknowledgement messages.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        [SystemBoundary]
        public void OnMessage(RequestForPositionsAck message)
        {
            Debug.NotNull(this.tradingGateway, nameof(this.tradingGateway));

            this.Execute(() =>
            {
                this.Log.Verbose($"Received {message}");
            });
        }

        /// <summary>
        /// Handles security list messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SystemBoundary]
        [PerformanceOptimized]
        public void OnMessage(SecurityList message)
        {
            Debug.NotNull(this.dataGateway, nameof(this.dataGateway));

            this.Execute(() =>
            {
                var instruments = new List<Instrument>();
                var groupCount = Convert.ToInt32(message.NoRelatedSym.ToString());
                var group = new SecurityList.NoRelatedSymGroup();

                for (var i = 1; i <= groupCount; i++)
                {
                    message.GetGroup(i, group);

                    var brokerSymbolCode = GetField(group, Tags.Symbol);
                    var symbol = this.GetSymbol(brokerSymbolCode);
                    if (symbol is null)
                    {
                        continue; // Symbol not set or convertible (error already logged)
                    }

                    var brokerSymbol = new BrokerSymbol(brokerSymbolCode);
                    var quoteCurrency = group.GetField(15).ToEnum<Nautilus.DomainModel.Enums.Currency>();

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

                this.Log.Verbose($"Received {message} (ResponseID={responseId}, Result={result})");
                this.dataGateway?.OnInstrumentsUpdate(instruments);
            });
        }

        /// <summary>
        /// Handles collateral report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SystemBoundary]
        public void OnMessage(CollateralReport message)
        {
            Debug.NotNull(this.tradingGateway, nameof(this.tradingGateway));

            this.Execute(() =>
            {
                var inquiryId = message.GetField(Tags.CollRptID);
                var accountNumber = message.GetField(Tags.Account);

                var cashBalance = Convert.ToDecimal(message.GetField(Tags.CashOutstanding));
                var cashStartDay = Convert.ToDecimal(message.GetField(Tags.StartCash));
                var cashDaily = Convert.ToDecimal(message.GetField(9047));
                var marginUsedMaintenance = Convert.ToDecimal(message.GetField(9046));
                var marginUsedLiq = Convert.ToDecimal(message.GetField(9038));
                var marginRatio = Convert.ToDecimal(message.GetField(Tags.MarginRatio));
                var marginCallStatus = message.GetField(9045);

                var accountEvent = new AccountStateEvent(
                    this.accountId,
                    this.accountCurrency,
                    Money.Create(cashBalance, this.accountCurrency),
                    Money.Create(cashStartDay, this.accountCurrency),
                    Money.Create(cashDaily, this.accountCurrency),
                    Money.Create(marginUsedLiq, this.accountCurrency),
                    Money.Create(marginUsedMaintenance, this.accountCurrency),
                    marginRatio,
                    marginCallStatus,
                    this.NewGuid(),
                    this.TimeNow());

                this.Log.Verbose($"Received {message}");
                this.tradingGateway?.Send(accountEvent);
            });
        }

        /// <summary>
        /// Handles quote status report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SystemBoundary]
        public void OnMessage(QuoteStatusReport message)
        {
            this.Execute(() =>
            {
                this.Log.Verbose($"Received {message}");
                this.Log.Information(message.Product.ToString());
            });
        }

        /// <summary>
        /// Handles market data request reject messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SystemBoundary]
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
        [SystemBoundary]
        [PerformanceOptimized]
        public void OnMessage(MarketDataSnapshotFullRefresh message)
        {
            Debug.NotNull(this.dataGateway, nameof(this.dataGateway));

            this.Execute(() =>
            {
                var symbol = this.GetSymbol(GetField(message, Tags.Symbol));
                if (symbol is null)
                {
                    return; // Symbol not set or convertible (error already logged)
                }

                var group = new MarketDataSnapshotFullRefresh.NoMDEntriesGroup();

                // Commented out code below is to capture the brokers tick timestamp although this has a lower
                // resolution than .TimeNow().
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

                this.dataGateway?.OnTick(new Tick(
                    symbol,
                    Price.Create(bidDecimal),
                    Price.Create(askDecimal),
                    this.TimeNow()));
            });
        }

        /// <summary>
        /// Handles order cancel reject messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SystemBoundary]
        public void OnMessage(QuickFix.FIX44.OrderCancelReject message)
        {
            Debug.NotNull(this.tradingGateway, nameof(this.tradingGateway));

            this.Execute(() =>
            {
                this.Log.Verbose($"Received {message}");

                var orderId = message.ClOrdID.ToString();
                var fxcmCode = message.GetField(9025);
                var cancelRejectResponseTo = message.CxlRejResponseTo.ToString();
                var cancelRejectReason = $"{message.CxlRejReason}, {message.Text.ToString().TrimEnd('.')}, FXCMCode={fxcmCode}";
                var timestamp = FixMessageHelper.ConvertExecutionReportString(GetField(message, Tags.TransactTime));

                var orderCancelReject = new OrderCancelReject(
                    new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                    this.accountId,
                    timestamp,
                    cancelRejectResponseTo,
                    cancelRejectReason,
                    this.NewGuid(),
                    this.TimeNow());

                this.tradingGateway?.Send(orderCancelReject);
            });
        }

        /// <summary>
        /// Handles execution report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SystemBoundary]
        public void OnMessage(ExecutionReport message)
        {
            Debug.NotNull(this.tradingGateway, nameof(this.tradingGateway));

            this.Execute(() =>
            {
                var symbol = this.GetSymbol(message.GetField(Tags.Symbol));
                if (symbol is null)
                {
                    return; // Error already logged.
                }

                var orderId = GetField(message, Tags.ClOrdID);
                var orderIdBroker = GetField(message, Tags.OrderID);
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

                    var orderRejected = new OrderRejected(
                        new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                        this.accountId,
                        timestamp,
                        rejectReason,
                        this.NewGuid(),
                        this.TimeNow());

                    this.tradingGateway?.Send(orderRejected);
                }

                if (orderStatus == OrdStatus.CANCELED.ToString())
                {
                    var orderCancelled = new OrderCancelled(
                        new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                        this.accountId,
                        timestamp,
                        this.NewGuid(),
                        this.TimeNow());

                    this.tradingGateway?.Send(orderCancelled);
                }

                if (orderStatus == OrdStatus.REPLACED.ToString())
                {
                    var orderModified = new OrderModified(
                        new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                        new OrderIdBroker(orderIdBroker),
                        this.accountId,
                        Price.Create(price, price.GetDecimalPlaces()),
                        timestamp,
                        this.NewGuid(),
                        this.TimeNow());

                    this.tradingGateway?.Send(orderModified);
                }

                if (orderStatus == OrdStatus.NEW.ToString())
                {
                    var expireTime = message.IsSetField(Tags.ExpireTime)
                        ? FixMessageHelper.ConvertExecutionReportString(message.GetField(Tags.ExpireTime))
                        : (ZonedDateTime?)null;

                    var orderWorking = new OrderWorking(
                        new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                        new OrderIdBroker(orderIdBroker),
                        this.accountId,
                        symbol,
                        new Label(orderLabel),
                        orderSide,
                        orderType,
                        Quantity.Create(orderQty),
                        Price.Create(price, price.GetDecimalPlaces()),
                        timeInForce,
                        expireTime,
                        timestamp,
                        this.NewGuid(),
                        this.TimeNow());

                    this.tradingGateway?.Send(orderWorking);
                }

                if (orderStatus == OrdStatus.EXPIRED.ToString())
                {
                    var orderExpired = new OrderExpired(
                        new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                        this.accountId,
                        timestamp,
                        this.NewGuid(),
                        this.TimeNow());

                    this.tradingGateway?.Send(orderExpired);
                }

                if (orderStatus == OrdStatus.FILLED.ToString())
                {
                    var executionId = GetField(message, Tags.ExecID);
                    var executionTicket = GetField(message, 9041);
                    var filledQuantity = Convert.ToInt32(GetField(message, Tags.CumQty));
                    var averagePrice = Convert.ToDecimal(GetField(message, Tags.AvgPx));

                    var orderFilled = new OrderFilled(
                        new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                        this.accountId,
                        new ExecutionId(executionId),
                        new ExecutionTicket(executionTicket),
                        symbol,
                        orderSide,
                        Quantity.Create(filledQuantity),
                        Price.Create(averagePrice, averagePrice.GetDecimalPlaces()),
                        timestamp,
                        this.NewGuid(),
                        this.TimeNow());

                    this.tradingGateway?.Send(orderFilled);
                }

                if (orderStatus == OrdStatus.PARTIALLY_FILLED.ToString())
                {
                    var executionId = GetField(message, Tags.ExecID);
                    var executionTicket = GetField(message, 9041);
                    var filledQuantity = Convert.ToInt32(GetField(message, Tags.CumQty));
                    var leavesQuantity = Convert.ToInt32(GetField(message, Tags.LeavesQty));
                    var averagePrice = Convert.ToDecimal(GetField(message, Tags.AvgPx));

                    var orderPartiallyFilled = new OrderPartiallyFilled(
                        new OrderId(OrderIdPostfixRemover.Remove(orderId)),
                        this.accountId,
                        new ExecutionId(executionId),
                        new ExecutionTicket(executionTicket),
                        symbol,
                        orderSide,
                        Quantity.Create(filledQuantity),
                        Quantity.Create(leavesQuantity),
                        Price.Create(averagePrice, averagePrice.GetDecimalPlaces()),
                        timestamp,
                        this.NewGuid(),
                        this.TimeNow());

                    this.tradingGateway?.Send(orderPartiallyFilled);
                }

                this.Log.Debug($"Received ExecutionReport(ClOrdID={orderId}, Status={message.GetField(Tags.OrdStatus)}).");
            });
        }

        private static string GetField(FieldMap map, int tag)
        {
            return map.IsSetField(tag)
                ? map.GetField(tag)
                : string.Empty;
        }

        private Symbol? GetSymbol(string brokerSymbolCode)
        {
            if (brokerSymbolCode == string.Empty)
            {
                this.Log.Error("The symbol tag did not contain a string.");
                return null;
            }

            var symbolCodeQuery = this.symbolConverter.GetNautilusSymbolCode(brokerSymbolCode);
            if (symbolCodeQuery is null)
            {
                this.Log.Error($"Could not find Nautilus symbol for {brokerSymbolCode}.");
                return null;
            }

            return this.symbolCache.Get($"{symbolCodeQuery}.{this.venue.Value}");
        }
    }
}
