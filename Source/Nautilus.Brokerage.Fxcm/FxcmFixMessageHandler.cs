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
        private const string RECV = "<--";
        private const string FIX = "[FIX]";

        private readonly AccountId accountId;
        private readonly Currency accountCurrency;
        private readonly Venue venue = new Venue("FXCM");
        private readonly SymbolConverter symbolConverter;
        private readonly ObjectCache<string, Symbol> symbolCache;
        private readonly ConcurrentDictionary<OrderID, OrderId> orderIdIndex;
        private readonly MarketDataIncrementalRefresh.NoMDEntriesGroup mdBidGroup;
        private readonly MarketDataIncrementalRefresh.NoMDEntriesGroup mdAskGroup;
        private readonly Func<ZonedDateTime> tickTimestampProvider;

        private IDataGateway? dataGateway;
        private IEndpoint? tradingGateway;

        /// <summary>
        /// Initializes a new instance of the <see cref="FxcmFixMessageHandler"/> class.
        /// </summary>
        /// <param name="container">The componentry container.</param>
        /// <param name="accountId">The account identifier for the handler.</param>
        /// <param name="accountCurrency">The account currency.</param>
        /// <param name="symbolConverter">The instrument data provider.</param>
        /// <param name="useBrokerTimestampForTicks">The flag to use the brokers timestamp for ticks.</param>
        public FxcmFixMessageHandler(
            IComponentryContainer container,
            AccountId accountId,
            Currency accountCurrency,
            SymbolConverter symbolConverter,
            bool useBrokerTimestampForTicks = false)
            : base(container)
        {
            this.accountId = accountId;
            this.accountCurrency = accountCurrency;
            this.symbolConverter = symbolConverter;
            this.symbolCache = new ObjectCache<string, Symbol>(Symbol.FromString);
            this.orderIdIndex = new ConcurrentDictionary<OrderID, OrderId>();
            this.mdBidGroup = new MarketDataIncrementalRefresh.NoMDEntriesGroup();
            this.mdAskGroup = new MarketDataIncrementalRefresh.NoMDEntriesGroup();

            if (useBrokerTimestampForTicks)
            {
                // FXCM market data timestamp has a lower resolution than .TimeNow()
                this.tickTimestampProvider = this.GetMarketDataTimestamp;
            }
            else
            {
                this.tickTimestampProvider = this.TimeNow;
            }
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
            this.Execute<FieldNotFoundException>(() =>
            {
                var inquiryId = message.GetField(Tags.CollInquiryID);
                var accountNumber = message.GetField(Tags.Account);

                this.Log.Debug($"{RECV}{FIX} {nameof(CollateralInquiryAck)}(InquiryId={inquiryId}, AccountNumber={accountNumber}).");
            });
        }

        /// <summary>
        /// Handles business message reject messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SystemBoundary]
        public void OnMessage(BusinessMessageReject message)
        {
            this.Execute<FieldNotFoundException>(() =>
            {
                this.Log.Debug($"{RECV}{FIX} {nameof(BusinessMessageReject)}");
            });
        }

        /// <summary>
        /// Handles business message reject messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SystemBoundary]
        public void OnMessage(Email message)
        {
            this.Execute<FieldNotFoundException>(() =>
            {
                this.Log.Debug($"{RECV}{FIX} {nameof(Email)}");
            });
        }

        /// <summary>
        /// Handles position report messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SystemBoundary]
        public void OnMessage(PositionReport message)
        {
            this.Execute<FieldNotFoundException>(() =>
            {
                this.Log.Debug($"{RECV}{FIX} {nameof(PositionReport)}({message.Account})");
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
            this.Execute<FieldNotFoundException>(() =>
            {
                this.Log.Debug($"{RECV}{FIX} {nameof(RequestForPositionsAck)}");
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
            this.Execute<FieldNotFoundException>(() =>
            {
                Debug.NotNull(this.dataGateway, nameof(this.dataGateway));

                var responseId = message.GetField(Tags.SecurityResponseID);
                var result = FixMessageHelper.GetSecurityRequestResult(message.SecurityRequestResult);
                this.Log.Debug($"{RECV}{FIX} {nameof(SecurityList)}(ResponseId={responseId}, Result={result}).");

                var instruments = new List<Instrument>();
                var groupCount = Convert.ToInt32(message.NoRelatedSym.ToString());
                var group = new SecurityList.NoRelatedSymGroup();

                for (var i = 1; i <= groupCount; i++)
                {
                    message.GetGroup(i, group);

                    var brokerSymbolCode = group.GetField(Tags.Symbol);
                    var symbol = this.GetSymbol(brokerSymbolCode);
                    var brokerSymbol = new BrokerSymbol(brokerSymbolCode);
                    var quoteCurrency = group.GetField(15).ToEnum<Nautilus.DomainModel.Enums.Currency>();
                    var securityType = FixMessageHelper.GetSecurityType(group.GetField(9080));
                    var tickPrecision = Convert.ToInt32(group.GetField(9001));
                    var tickSize = Convert.ToDecimal(group.GetField(9002)) * 0.1m;  // Field 9002 gives 'point' size (* 0.1m to get tick size)
                    var roundLot = Convert.ToInt32(group.GetField(561));
                    var minStopDistanceEntry = Convert.ToInt32(group.GetField(9092));
                    var minLimitDistanceEntry = Convert.ToInt32(group.GetField(9093));
                    var minStopDistance = Convert.ToInt32(group.GetField(9090));
                    var minLimitDistance = Convert.ToInt32(group.GetField(9091));
                    var minTradeSize = Convert.ToInt32(group.GetField(9095));
                    var maxTradeSize = Convert.ToInt32(group.GetField(9094));
                    var rolloverInterestBuy = Convert.ToDecimal(group.GetField(9003));
                    var rolloverInterestSell = Convert.ToDecimal(group.GetField(9004));

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
            this.Execute<FieldNotFoundException>(() =>
            {
                Debug.NotNull(this.tradingGateway, nameof(this.tradingGateway));

                var inquiryId = message.GetField(Tags.CollRptID);
                var accountNumber = message.GetField(Tags.Account);
                this.Log.Debug($"{RECV}{FIX} {nameof(CollateralReport)}(InquiryId={inquiryId}, AccountNumber={accountNumber}).");

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
            this.Execute<FieldNotFoundException>(() =>
            {
                this.Log.Verbose($"{RECV}{FIX} {nameof(QuoteStatusReport)}");
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
            this.Execute<FieldNotFoundException>(() =>
            {
                this.Log.Warning($"{RECV}{FIX} {nameof(MarketDataRequestReject)}(Text={message.GetField(Tags.Text)}).");
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
            this.Execute<FieldNotFoundException>(() =>
            {
                Debug.NotNull(this.dataGateway, nameof(this.dataGateway));

                try
                {
                    message.GetGroup(1, this.mdBidGroup);
                    message.GetGroup(2, this.mdAskGroup);

                    this.dataGateway?.OnTick(new Tick(
                        this.GetSymbol(message.GetField(Tags.Symbol)),
                        Price.Create(Convert.ToDecimal(this.mdBidGroup.GetField(Tags.MDEntryPx))),
                        Price.Create(Convert.ToDecimal(this.mdAskGroup.GetField(Tags.MDEntryPx))),
                        this.tickTimestampProvider()));
                }
                catch (Exception ex)
                {
                    if (ex is FormatException || ex is OverflowException)
                    {
                        this.Log.Error("Could not parse decimal.");
                    }

                    this.Log.Fatal(ex.Message, ex);
                }
            });
        }

        /// <summary>
        /// Handles order cancel reject messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SystemBoundary]
        public void OnMessage(QuickFix.FIX44.OrderCancelReject message)
        {
            this.Execute<FieldNotFoundException>(() =>
            {
                Debug.NotNull(this.tradingGateway, nameof(this.tradingGateway));

                this.Log.Debug($"{RECV}{FIX} {nameof(OrderCancelReject)}");

                var orderId = this.GetOrderId(message.OrderID);
                var rejectReasonCode = message.GetField(9025);
                var fxcmRejectCode = message.GetField(9029);
                var rejectResponseTo = FixMessageHelper.GetCxlRejResponseTo(message.CxlRejResponseTo);
                var rejectReasonText = message.GetField(Tags.CxlRejReason);
                var rejectReason = $"FXCM({fxcmRejectCode}) {FixMessageHelper.GetCancelRejectReasonString(rejectReasonCode)} ({rejectReasonText})";
                var rejectedTime = FixMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));

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
            this.Execute<FieldNotFoundException>(() =>
            {
                Debug.NotNull(this.tradingGateway, nameof(this.tradingGateway));

                switch (message.OrdStatus.Obj)
                {
                    case OrdStatus.REJECTED:
                    {
                        this.Log.Debug($"{RECV}{FIX} {nameof(ExecutionReport)}({nameof(OrdStatus.REJECTED)})");

                        this.tradingGateway?.Send(this.GenerateOrderRejectedEvent(message));
                        break;
                    }

                    case OrdStatus.PENDING_NEW:
                    {
                        this.Log.Debug($"{RECV}{FIX} {nameof(ExecutionReport)}({nameof(OrdStatus.PENDING_NEW)}).");

                        // Do nothing
                        break;
                    }

                    case OrdStatus.NEW:
                    {
                        this.Log.Debug($"{RECV}{FIX} {nameof(ExecutionReport)}({nameof(OrdStatus.NEW)})");

                        this.tradingGateway?.Send(this.GenerateOrderWorkingEvent(message));
                        break;
                    }

                    case OrdStatus.PENDING_CANCEL:
                    {
                        this.Log.Debug($"{RECV}{FIX} {nameof(ExecutionReport)}({nameof(OrdStatus.PENDING_CANCEL)}).");

                        // Do nothing
                        break;
                    }

                    case OrdStatus.CANCELED:
                    {
                        this.Log.Debug($"{RECV}{FIX} {nameof(ExecutionReport)}({nameof(OrdStatus.CANCELED)})");

                        this.tradingGateway?.Send(this.GenerateOrderCancelledEvent(message));
                        break;
                    }

                    case OrdStatus.REPLACED:
                    {
                        this.Log.Debug($"{RECV}{FIX} {nameof(ExecutionReport)}({nameof(OrdStatus.REPLACED)})");

                        this.tradingGateway?.Send(this.GenerateOrderModifiedEvent(message));
                        break;
                    }

                    case OrdStatus.EXPIRED:
                    {
                        this.Log.Debug($"{RECV}{FIX} {nameof(ExecutionReport)}({nameof(OrdStatus.EXPIRED)})");

                        this.tradingGateway?.Send(this.GenerateOrderExpiredEvent(message));
                        break;
                    }

                    case OrdStatus.STOPPED:
                    {
                        this.Log.Debug($"{RECV}{FIX} {nameof(ExecutionReport)}({nameof(OrdStatus.STOPPED)}).");

                        // Order is executing
                        break;
                    }

                    case OrdStatus.FILLED:
                    {
                        this.Log.Debug($"{RECV}{FIX} {nameof(ExecutionReport)}({nameof(OrdStatus.EXPIRED)})");

                        this.tradingGateway?.Send(this.GenerateOrderFilledEvent(message));
                        break;
                    }

                    case OrdStatus.PARTIALLY_FILLED:
                    {
                        this.Log.Debug($"{RECV}{FIX} {nameof(ExecutionReport)}({nameof(OrdStatus.PARTIALLY_FILLED)})");

                        this.tradingGateway?.Send(this.GenerateOrderPartiallyFilledEvent(message));
                        break;
                    }

                    case OrdStatus.SUSPENDED:
                    {
                        this.Log.Warning($"{RECV}{FIX} Unhandled {nameof(ExecutionReport)}({nameof(OrdStatus.SUSPENDED)}).");
                        break;
                    }

                    case OrdStatus.CALCULATED:
                    {
                        this.Log.Warning($"{RECV}{FIX} Unhandled {nameof(ExecutionReport)}({nameof(OrdStatus.CALCULATED)}).");
                        break;
                    }

                    case OrdStatus.DONE_FOR_DAY:
                    {
                        this.Log.Warning($"{RECV}{FIX} Unhandled {nameof(ExecutionReport)}({nameof(OrdStatus.DONE_FOR_DAY)}).");
                        break;
                    }

                    case OrdStatus.PENDING_REPLACE:
                    {
                        this.Log.Warning($"{RECV}{FIX} Unhandled {nameof(ExecutionReport)}({nameof(OrdStatus.PENDING_REPLACE)}).");
                        break;
                    }

                    case OrdStatus.ACCEPTED_FOR_BIDDING:
                    {
                        this.Log.Warning($"{RECV}{FIX} Unhandled {nameof(ExecutionReport)}({nameof(OrdStatus.ACCEPTED_FOR_BIDDING)}).");
                        break;
                    }

                    default:
                        ExceptionFactory.InvalidSwitchArgument(message.OrdStatus, nameof(message.OrdStatus));
                        break;
                }
            });
        }

        private ZonedDateTime GetMarketDataTimestamp()
        {
            var dateTimeString = this.mdBidGroup.GetField(Tags.MDEntryDate) + this.mdBidGroup.GetField(Tags.MDEntryTime);
            return FixMessageHelper.ParseMarketDataTimestamp(dateTimeString);
        }

        private OrderRejected GenerateOrderRejectedEvent(ExecutionReport message)
        {
            var orderId = this.GetOrderId(message.OrderID);
            var rejectedTime = FixMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));
            var rejectReasonCode = message.GetField(9025);
            var fxcmRejectCode = message.GetField(9029);
            var rejectReasonText = message.GetField(Tags.CxlRejReason);
            var rejectReason = $"FXCM({fxcmRejectCode}) {FixMessageHelper.GetCancelRejectReasonString(rejectReasonCode)} ({rejectReasonText})";

            return new OrderRejected(
                orderId,
                this.accountId,
                rejectedTime,
                rejectReason,
                this.NewGuid(),
                this.TimeNow());
        }

        private OrderAccepted GenerateOrderAcceptedEvent(ExecutionReport message)
        {
            var orderId = this.GetOrderId(message.OrderID);
            var acceptedTime = FixMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));

            return new OrderAccepted(
                orderId,
                this.accountId,
                acceptedTime,
                this.NewGuid(),
                this.TimeNow());
        }

        private OrderCancelled GenerateOrderCancelledEvent(ExecutionReport message)
        {
            var orderId = this.GetOrderId(message.OrderID);
            var cancelledTime = FixMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));

            return new OrderCancelled(
                orderId,
                this.accountId,
                cancelledTime,
                this.NewGuid(),
                this.TimeNow());
        }

        private OrderModified GenerateOrderModifiedEvent(ExecutionReport message)
        {
            var orderId = this.GetOrderId(message.OrderID);
            var orderIdBroker = new OrderIdBroker(message.OrderID.ToString());
            var orderType = FixMessageHelper.GetOrderType(message.GetField(Tags.OrdType));
            var price = FixMessageHelper.GetOrderPrice(orderType, message);
            var modifiedTime = FixMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));

            return new OrderModified(
                orderId,
                orderIdBroker,
                this.accountId,
                price,
                modifiedTime,
                this.NewGuid(),
                this.TimeNow());
        }

        private OrderWorking GenerateOrderWorkingEvent(ExecutionReport message)
        {
            var orderId = new OrderId(message.GetField(Tags.ClOrdID));
            this.orderIdIndex[message.OrderID] = orderId;  // Index order identifiers
            var orderIdBroker = new OrderIdBroker(message.GetField(Tags.OrderID));
            var symbol = this.GetSymbol(message.GetField(Tags.Symbol));
            var orderLabel = new Label(message.GetField(Tags.SecondaryClOrdID));
            var orderSide = FixMessageHelper.GetOrderSide(message.GetField(Tags.Side));
            var orderType = FixMessageHelper.GetOrderType(message.GetField(Tags.OrdType));
            var orderQty = Quantity.Create(Convert.ToInt32(message.GetField(Tags.OrderQty)));
            var price = FixMessageHelper.GetOrderPrice(orderType, message);
            var timeInForce = FixMessageHelper.GetTimeInForce(message.GetField(Tags.TimeInForce));
            var expireTime = FixMessageHelper.GetExpireTime(message);
            var workingTime = FixMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));

            return new OrderWorking(
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
                workingTime,
                this.NewGuid(),
                this.TimeNow());
        }

        private OrderExpired GenerateOrderExpiredEvent(ExecutionReport message)
        {
            var orderId = this.GetOrderId(message.OrderID);
            var expiredTime = FixMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));

            return new OrderExpired(
                orderId,
                this.accountId,
                expiredTime,
                this.NewGuid(),
                this.TimeNow());
        }

        private OrderFilled GenerateOrderFilledEvent(ExecutionReport message)
        {
            var orderId = this.GetOrderId(message.OrderID);
            var executionId = new ExecutionId(message.GetField(Tags.ExecID));
            var executionTicket = new ExecutionTicket(message.GetField(9041));
            var symbol = this.GetSymbol(message.GetField(Tags.Symbol));
            var orderSide = FixMessageHelper.GetOrderSide(message.GetField(Tags.Side));
            var filledQuantity = Quantity.Create(Convert.ToInt32(message.GetField(Tags.CumQty)));
            var averagePrice = Price.Create(Convert.ToDecimal(message.GetField(Tags.AvgPx)));
            var executionTime = FixMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));

            return new OrderFilled(
                orderId,
                this.accountId,
                executionId,
                executionTicket,
                symbol,
                orderSide,
                filledQuantity,
                averagePrice,
                executionTime,
                this.NewGuid(),
                this.TimeNow());
        }

        private OrderPartiallyFilled GenerateOrderPartiallyFilledEvent(ExecutionReport message)
        {
            var orderId = this.orderIdIndex[message.OrderID];
            var executionId = new ExecutionId(message.GetField(Tags.ExecID));
            var executionTicket = new ExecutionTicket(message.GetField(9041));
            var symbol = this.GetSymbol(message.GetField(Tags.Symbol));
            var orderSide = FixMessageHelper.GetOrderSide(message.GetField(Tags.Side));
            var filledQuantity = Quantity.Create(Convert.ToInt32(message.GetField(Tags.CumQty)));
            var averagePrice = Price.Create(Convert.ToDecimal(message.GetField(Tags.AvgPx)));
            var leavesQuantity = Quantity.Create(Convert.ToInt32(message.GetField(Tags.LeavesQty)));
            var executionTime = FixMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));

            return new OrderPartiallyFilled(
                orderId,
                this.accountId,
                executionId,
                executionTicket,
                symbol,
                orderSide,
                filledQuantity,
                leavesQuantity,
                averagePrice,
                executionTime,
                this.NewGuid(),
                this.TimeNow());
        }

        private OrderId GetOrderId(OrderID orderIdBroker)
        {
            if (this.orderIdIndex.TryGetValue(orderIdBroker, out var orderId))
            {
                return orderId;
            }

            throw new ArgumentException($"Could not find OrderId for FIX OrderID({orderIdBroker}).");
        }

        private Symbol GetSymbol(string brokerSymbolCode)
        {
            var symbolCodeQuery = this.symbolConverter.GetNautilusSymbolCode(brokerSymbolCode);
            if (symbolCodeQuery is null)
            {
                throw new ArgumentException($"Could not find Nautilus symbol for {brokerSymbolCode}.");
            }

            return this.symbolCache.Get($"{symbolCodeQuery}.{this.venue.Value}");
        }
    }
}
