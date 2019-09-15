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
    using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<OrderID, OrderId> orderIdIndex;

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
            this.orderIdIndex = new ConcurrentDictionary<OrderID, OrderId>();
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
            this.Execute<ArgumentException>(() =>
            {
                Debug.NotNull(this.tradingGateway, nameof(this.tradingGateway));

                var inquiryId = message.GetField(Tags.CollInquiryID);
                var accountNumber = Convert.ToInt32(message.GetField(Tags.Account)).ToString();

                this.Log.Debug($"<-- {nameof(CollateralInquiryAck)}(InquiryId={inquiryId}, AccountNumber={accountNumber}).");
            });
        }

        /// <summary>
        /// Handles business message reject messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SystemBoundary]
        public void OnMessage(BusinessMessageReject message)
        {
            this.Execute<ArgumentException>(() =>
            {
                this.Log.Debug($"<-- {nameof(BusinessMessageReject)}");
            });
        }

        /// <summary>
        /// Handles position report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SystemBoundary]
        public void OnMessage(PositionReport message)
        {
            this.Execute<ArgumentException>(() =>
            {
                Condition.NotEmptyOrWhiteSpace(message.Account.ToString(), nameof(message.Account));

                this.Log.Debug($"<-- {nameof(PositionReport)}({message.Account})");
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
            this.Execute<ArgumentException>(() =>
            {
                Debug.NotNull(this.tradingGateway, nameof(this.tradingGateway));

                this.Log.Debug($"<-- {nameof(RequestForPositionsAck)}");
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
            this.Execute<ArgumentException>(() =>
            {
                Debug.NotNull(this.dataGateway, nameof(this.dataGateway));

                var responseId = message.GetField(Tags.SecurityResponseID);
                var result = FixMessageHelper.GetSecurityRequestResult(message.SecurityRequestResult);
                this.Log.Debug($"<-- {nameof(SecurityList)}(ResponseId={responseId}, Result={result}).");

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
            this.Execute<ArgumentException>(() =>
            {
                Debug.NotNull(this.tradingGateway, nameof(this.tradingGateway));

                var inquiryId = message.GetField(Tags.CollRptID);
                var accountNumber = message.GetField(Tags.Account);
                this.Log.Debug($"<-- {nameof(CollateralReport)}(InquiryId={inquiryId}, AccountNumber={accountNumber}).");

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
            this.Execute<ArgumentException>(() =>
            {
                this.Log.Verbose($"<-- {nameof(QuoteStatusReport)}");
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
                this.Log.Warning($"<-- {nameof(BusinessMessageReject)}(Text={message.GetField(Tags.Text)}).");
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
            this.Execute<ArgumentException>(() =>
            {
                Debug.NotNull(this.dataGateway, nameof(this.dataGateway));

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
            this.Execute<ArgumentException>(() =>
            {
                Debug.NotNull(this.tradingGateway, nameof(this.tradingGateway));

                this.Log.Debug($"<-- {nameof(OrderCancelReject)}");

                var orderId = this.GetOrderId(message.OrderID);
                if (orderId is null)
                {
                    return; // Error logged
                }

                var rejectReasonCode = GetField(message, 9025);
                var fxcmRejectCode = message.GetField(9029);
                var rejectResponseTo = FixMessageHelper.GetCxlRejResponseTo(message.CxlRejResponseTo);
                var rejectReasonText = GetField(message, Tags.CxlRejReason);
                var rejectReason = $"Code({rejectReasonCode})={FixMessageHelper.GetCancelRejectReasonString(rejectReasonCode)}, FXCM({fxcmRejectCode})={rejectReasonText}";
                var rejectedTime = FixMessageHelper.ConvertExecutionReportString(GetField(message, Tags.TransactTime));

                var orderCancelReject = new OrderCancelReject(
                    orderId,
                    this.accountId,
                    rejectedTime,
                    rejectResponseTo,
                    rejectReason,
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
            this.Execute<ArgumentException>(() =>
            {
                Debug.NotNull(this.tradingGateway, nameof(this.tradingGateway));

                var orderIdBroker = new OrderIdBroker(GetField(message, Tags.OrderID));
                var orderLabel = new Label(GetField(message, Tags.SecondaryClOrdID));
                var orderSide = FixMessageHelper.GetOrderSide(GetField(message, Tags.Side));
                var orderType = FixMessageHelper.GetOrderType(GetField(message, Tags.OrdType));
                var timeInForce = FixMessageHelper.GetTimeInForce(GetField(message, Tags.TimeInForce));
                var timestamp = FixMessageHelper.ConvertExecutionReportString(GetField(message, Tags.TransactTime));
                var orderQty = Quantity.Create(Convert.ToInt32(GetField(message, Tags.OrderQty)));

                this.Log.Debug($"<-- {nameof(ExecutionReport)}(heading into switch)");
                switch (message.OrdStatus.Obj)
                {
                    case OrdStatus.REJECTED:
                    {
                        this.Log.Debug($"<-- {nameof(ExecutionReport)}({nameof(OrdStatus.REJECTED)})");

                        var orderId = this.GetOrderId(message.OrderID);
                        if (orderId is null)
                        {
                            return; // Error logged
                        }

                        var rejectReasonCode = message.GetField(9025);
                        var fxcmRejectCode = message.GetField(9029);
                        var rejectReasonText = GetField(message, Tags.CxlRejReason);
                        var rejectReason = $"Code({rejectReasonCode})={FixMessageHelper.GetCancelRejectReasonString(rejectReasonCode)}, FXCM({fxcmRejectCode})={rejectReasonText}";

                        var orderRejected = new OrderRejected(
                            orderId,
                            this.accountId,
                            timestamp,
                            rejectReason,
                            this.NewGuid(),
                            this.TimeNow());

                        this.tradingGateway?.Send(orderRejected);
                        break;
                    }

                    case OrdStatus.CANCELED:
                    {
                        this.Log.Debug($"<-- {nameof(ExecutionReport)}({nameof(OrdStatus.CANCELED)})");

                        var orderId = this.GetOrderId(message.OrderID);
                        if (orderId is null)
                        {
                            return; // Error logged
                        }

                        var orderCancelled = new OrderCancelled(
                            orderId,
                            this.accountId,
                            timestamp,
                            this.NewGuid(),
                            this.TimeNow());

                        this.tradingGateway?.Send(orderCancelled);
                        break;
                    }

                    case OrdStatus.REPLACED:
                    {
                        this.Log.Debug($"<-- {nameof(ExecutionReport)}({nameof(OrdStatus.REPLACED)})");

                        var orderId = this.GetOrderId(message.OrderID);
                        if (orderId is null)
                        {
                            return; // Error logged
                        }

                        var price = Price.Create(FixMessageHelper.GetOrderPrice(
                            orderType,
                            Convert.ToDecimal(GetField(message, Tags.StopPx)),
                            Convert.ToDecimal(GetField(message, Tags.Price))));

                        var orderModified = new OrderModified(
                            orderId,
                            orderIdBroker,
                            this.accountId,
                            price,
                            timestamp,
                            this.NewGuid(),
                            this.TimeNow());

                        this.tradingGateway?.Send(orderModified);
                        break;
                    }

                    case OrdStatus.NEW:
                    {
                        this.Log.Debug($"<-- {nameof(ExecutionReport)}({nameof(OrdStatus.NEW)})");

                        var orderId = new OrderId(message.OrigClOrdID.ToString());
                        this.orderIdIndex[message.OrderID] = orderId;

                        var symbol = this.GetSymbol(message.GetField(Tags.Symbol));
                        if (symbol is null)
                        {
                            return;  // Error logged
                        }

                        var price = Price.Create(FixMessageHelper.GetOrderPrice(
                            orderType,
                            Convert.ToDecimal(GetField(message, Tags.StopPx)),
                            Convert.ToDecimal(GetField(message, Tags.Price))));

                        var expireTime = message.IsSetField(Tags.ExpireTime)
                            ? FixMessageHelper.ConvertExecutionReportString(message.GetField(Tags.ExpireTime))
                            : (ZonedDateTime?)null;

                        var orderWorking = new OrderWorking(
                            orderId,
                            orderIdBroker,
                            this.accountId,
                            symbol,
                            orderLabel,
                            orderSide,
                            orderType,
                            orderQty,
                            price,
                            timeInForce,
                            expireTime,
                            timestamp,
                            this.NewGuid(),
                            this.TimeNow());

                        this.tradingGateway?.Send(orderWorking);
                        break;
                    }

                    case OrdStatus.EXPIRED:
                    {
                        this.Log.Debug($"<-- {nameof(ExecutionReport)}({nameof(OrdStatus.EXPIRED)})");

                        var orderId = this.GetOrderId(message.OrderID);
                        if (orderId is null)
                        {
                            return; // Error logged
                        }

                        var orderExpired = new OrderExpired(
                            orderId,
                            this.accountId,
                            timestamp,
                            this.NewGuid(),
                            this.TimeNow());

                        this.tradingGateway?.Send(orderExpired);
                        break;
                    }

                    case OrdStatus.FILLED:
                    {
                        this.Log.Debug($"<-- {nameof(ExecutionReport)}({nameof(OrdStatus.EXPIRED)})");

                        var orderId = this.GetOrderId(message.OrderID);
                        if (orderId is null)
                        {
                            return; // Error logged
                        }

                        var executionId = new ExecutionId(GetField(message, Tags.ExecID));
                        var executionTicket = new ExecutionTicket(GetField(message, 9041));

                        var symbol = this.GetSymbol(message.GetField(Tags.Symbol));
                        if (symbol is null)
                        {
                            return;  // Error logged
                        }

                        var filledQuantity = Quantity.Create(Convert.ToInt32(GetField(message, Tags.CumQty)));
                        var averagePrice = Price.Create(Convert.ToDecimal(GetField(message, Tags.AvgPx)));

                        var orderFilled = new OrderFilled(
                            orderId,
                            this.accountId,
                            executionId,
                            executionTicket,
                            symbol,
                            orderSide,
                            filledQuantity,
                            averagePrice,
                            timestamp,
                            this.NewGuid(),
                            this.TimeNow());

                        this.tradingGateway?.Send(orderFilled);
                        break;
                    }

                    case OrdStatus.PARTIALLY_FILLED:
                    {
                        this.Log.Debug($"<-- {nameof(ExecutionReport)}({nameof(OrdStatus.PARTIALLY_FILLED)})");

                        var orderId = this.GetOrderId(message.OrderID);
                        if (orderId is null)
                        {
                            return; // Error logged
                        }

                        var executionId = new ExecutionId(GetField(message, Tags.ExecID));
                        var executionTicket = new ExecutionTicket(GetField(message, 9041));

                        var symbol = this.GetSymbol(message.GetField(Tags.Symbol));
                        if (symbol is null)
                        {
                            return;  // Error logged
                        }

                        var filledQuantity = Quantity.Create(Convert.ToInt32(GetField(message, Tags.CumQty)));
                        var averagePrice = Price.Create(Convert.ToDecimal(GetField(message, Tags.AvgPx)));
                        var leavesQuantity = Quantity.Create(Convert.ToInt32(GetField(message, Tags.LeavesQty)));

                        var orderPartiallyFilled = new OrderPartiallyFilled(
                            orderId,
                            this.accountId,
                            executionId,
                            executionTicket,
                            symbol,
                            orderSide,
                            filledQuantity,
                            leavesQuantity,
                            averagePrice,
                            timestamp,
                            this.NewGuid(),
                            this.TimeNow());

                        this.tradingGateway?.Send(orderPartiallyFilled);
                        break;
                    }

                    case OrdStatus.STOPPED:
                    {
                        this.Log.Warning($"<-- Unhandled ExecutionReport({nameof(OrdStatus.STOPPED)}).");
                        break;
                    }

                    case OrdStatus.SUSPENDED:
                    {
                        this.Log.Warning($"<-- Unhandled ExecutionReport({nameof(OrdStatus.SUSPENDED)}).");
                        break;
                    }

                    case OrdStatus.CALCULATED:
                    {
                        this.Log.Warning($"<-- Unhandled ExecutionReport({nameof(OrdStatus.CALCULATED)}).");
                        break;
                    }

                    case OrdStatus.DONE_FOR_DAY:
                    {
                        this.Log.Warning($"<-- Unhandled ExecutionReport({nameof(OrdStatus.DONE_FOR_DAY)}).");
                        break;
                    }

                    case OrdStatus.PENDING_NEW:
                    {
                        this.Log.Warning($"<-- Unhandled ExecutionReport({nameof(OrdStatus.PENDING_NEW)}).");
                        break;
                    }

                    case OrdStatus.PENDING_CANCEL:
                    {
                        this.Log.Warning($"<-- Unhandled ExecutionReport({nameof(OrdStatus.PENDING_CANCEL)}).");
                        break;
                    }

                    case OrdStatus.PENDING_REPLACE:
                    {
                        this.Log.Warning($"<-- Unhandled ExecutionReport({nameof(OrdStatus.PENDING_REPLACE)}).");
                        break;
                    }

                    case OrdStatus.ACCEPTED_FOR_BIDDING:
                    {
                        this.Log.Warning($"<-- Unhandled ExecutionReport({nameof(OrdStatus.ACCEPTED_FOR_BIDDING)}).");
                        break;
                    }

                    default:
                        ExceptionFactory.InvalidSwitchArgument(message.OrdStatus, nameof(message.OrdStatus));
                        break;
                }
            });
        }

        private static string GetField(FieldMap map, int tag)
        {
            return map.IsSetField(tag)
                ? map.GetField(tag)
                : string.Empty;
        }

        private OrderId? GetOrderId(OrderID orderIdBroker)
        {
            if (this.orderIdIndex.TryGetValue(orderIdBroker, out var orderId))
            {
                return orderId;
            }

            this.Log.Error($"Could not find OrderId for OrderIdBroker({orderIdBroker}).");
            return null;
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
