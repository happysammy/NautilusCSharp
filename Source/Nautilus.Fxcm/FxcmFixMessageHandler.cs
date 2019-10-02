﻿//--------------------------------------------------------------------------------------------------
// <copyright file="FxcmFixMessageHandler.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.Fxcm
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
        private const string FXCM = "FXCM";
        private const string RECV = "<--";
        private const string FIX = "[FIX]";

        private readonly AccountId accountId;
        private readonly Currency accountCurrency;
        private readonly Venue venue = new Venue(FXCM);
        private readonly SymbolConverter symbolConverter;
        private readonly ObjectCache<string, Symbol> symbolCache;
        private readonly Dictionary<string, OrderId> orderIdIndex;
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
            this.orderIdIndex = new Dictionary<string, OrderId>();
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

        /// <inheritdoc />
        [SystemBoundary]
        public void OnMessage(BusinessMessageReject message)
        {
            this.Execute<FieldNotFoundException>(() =>
            {
                this.Log.Warning($"{RECV}{FIX} {nameof(BusinessMessageReject)}");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnMessage(Email message)
        {
            this.Execute<FieldNotFoundException>(() =>
            {
                this.Log.Warning($"{RECV}{FIX} {nameof(Email)}");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnMessage(SecurityList message)
        {
            this.Execute<FieldNotFoundException>(() =>
            {
                Debug.NotNull(this.dataGateway, nameof(this.dataGateway));

                var responseId = message.GetField(Tags.SecurityResponseID);
                var result = FxcmMessageHelper.GetSecurityRequestResult(message.SecurityRequestResult);
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
                    var baseCurrency = group.GetField(Tags.Currency).ToEnum<Nautilus.DomainModel.Enums.Currency>();
                    var securityType = FxcmMessageHelper.GetSecurityType(group.GetString(FxcmTags.ProductID));
                    var tickPrecision = group.GetInt(FxcmTags.SymPrecision);
                    var tickSize = group.GetDecimal(FxcmTags.SymPointSize) * 0.1m;  // Field 9002 gives 'point' size (* 0.1m to get tick size)
                    var roundLot = group.GetInt(Tags.RoundLot);
                    var minStopDistanceEntry = group.GetInt(FxcmTags.CondDistEntryStop);
                    var minLimitDistanceEntry = group.GetInt(FxcmTags.CondDistEntryLimit);
                    var minStopDistance = group.GetInt(FxcmTags.CondDistStop);
                    var minLimitDistance = group.GetInt(FxcmTags.CondDistLimit);
                    var minTradeSize = group.GetInt(FxcmTags.MinQuantity);
                    var maxTradeSize = group.GetInt(FxcmTags.MaxQuantity);
                    var rolloverInterestBuy = group.GetDecimal(FxcmTags.SymInterestBuy);
                    var rolloverInterestSell = group.GetDecimal(FxcmTags.SymInterestSell);

                    var instrument = new Instrument(
                        symbol,
                        brokerSymbol,
                        baseCurrency,
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

        /// <inheritdoc />
        [SystemBoundary]
        public void OnMessage(QuoteStatusReport message)
        {
            this.Execute<FieldNotFoundException>(() =>
            {
                this.Log.Verbose($"{RECV}{FIX} {nameof(QuoteStatusReport)}");
                this.Log.Information(message.Product.ToString());
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnMessage(TradingSessionStatus message)
        {
            this.Execute<FieldNotFoundException>(() =>
            {
                this.Log.Warning($"{RECV}{FIX} Unhandled {nameof(TradingSessionStatus)}");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnMessage(MarketDataRequestReject message)
        {
            this.Execute<FieldNotFoundException>(() =>
            {
                this.Log.Warning($"{RECV}{FIX} {nameof(MarketDataRequestReject)}(Text={message.GetField(Tags.Text)}).");
            });
        }

        /// <inheritdoc />
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
                        Price.Create(this.mdBidGroup.GetDecimal(Tags.MDEntryPx)),
                        Price.Create(this.mdAskGroup.GetDecimal(Tags.MDEntryPx)),
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        [SystemBoundary]
        public void OnMessage(CollateralReport message)
        {
            this.Execute<FieldNotFoundException>(() =>
            {
                Debug.NotNull(this.tradingGateway, nameof(this.tradingGateway));

                var inquiryId = message.GetField(Tags.CollRptID);
                var accountNumber = message.GetField(Tags.Account);
                this.Log.Debug($"{RECV}{FIX} {nameof(CollateralReport)}(InquiryId={inquiryId}, AccountNumber={accountNumber}).");

                var cashBalance = message.GetDecimal(Tags.CashOutstanding);
                var cashStartDay = message.GetDecimal(Tags.StartCash);
                var cashDaily = message.GetDecimal(FxcmTags.CashDaily);
                var marginUsedMaintenance = message.GetDecimal(FxcmTags.UsedMarginMaintenance);
                var marginUsedLiq = message.GetDecimal(FxcmTags.UsedMarginLiquidation);
                var marginRatio = message.GetDecimal(Tags.MarginRatio);
                var marginCallStatus = message.GetField(FxcmTags.MarginCall);

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

        /// <inheritdoc />
        [SystemBoundary]
        public void OnMessage(RequestForPositionsAck message)
        {
            this.Execute<FieldNotFoundException>(() =>
            {
                var accountNumber = message.GetField(Tags.Account);
                var posRequestId = message.GetField(Tags.PosReqID);
                var posRequestStatus = message.GetInt(Tags.PosReqStatus) == 0
                    ? "Completed"
                    : "Rejected";

                var posRequestResult = message.GetInt(Tags.PosReqResult);
                string posRequestResultString;
                switch (posRequestResult)
                {
                    case 0:
                        posRequestResultString = "Valid request";
                        break;
                    case 2:
                        posRequestResultString = "No positions found";
                        break;
                    default:
                        posRequestResultString = "Other";
                        break;
                }

                this.Log.Information($"{RECV}{FIX} {nameof(RequestForPositionsAck)}(" +
                                     $"Account={accountNumber}" +
                                     $"PosRequestId={posRequestId}, " +
                                     $"Account={accountNumber}" +
                                     $"Status={posRequestStatus}, " +
                                     $"Result={posRequestResultString}");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnMessage(PositionReport message)
        {
            this.Execute<FieldNotFoundException>(() =>
            {
                this.Log.Debug($"{RECV}{FIX} {nameof(PositionReport)}({message.Account})");
            });
        }

        /// <inheritdoc />
        [SystemBoundary]
        public void OnMessage(QuickFix.FIX44.OrderCancelReject message)
        {
            this.Execute<FieldNotFoundException>(() =>
            {
                Debug.NotNull(this.tradingGateway, nameof(this.tradingGateway));

                this.Log.Debug($"{RECV}{FIX} {nameof(OrderCancelReject)}");

                var orderId = this.GetOrderId(message);
                var rejectedTime = FxcmMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));
                var rejectResponseTo = FxcmMessageHelper.GetCxlRejResponseTo(message.CxlRejResponseTo);
                var rejectReason = message.GetField(FxcmTags.ErrorDetails);

                var orderCancelReject = new OrderCancelReject(
                    this.accountId,
                    orderId,
                    rejectedTime,
                    rejectResponseTo,
                    rejectReason,
                    this.NewGuid(),
                    this.TimeNow());

                this.tradingGateway?.Send(orderCancelReject);
            });
        }

        /// <inheritdoc />
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
                        var fxcmOrdStatus = message.GetField(FxcmTags.OrdStatus);
                        this.Log.Debug($"{RECV}{FIX} {nameof(ExecutionReport)}({nameof(OrdStatus.NEW)}-{fxcmOrdStatus})");

                        switch (fxcmOrdStatus)
                        {
                            case "P": // In Process
                                this.tradingGateway?.Send(this.GenerateOrderAcceptedEvent(message));
                                this.tradingGateway?.Send(this.GenerateOrderWorkingEvent(message));
                                break;
                            case "I": // Dealer Intervention
                                this.tradingGateway?.Send(this.GenerateOrderAcceptedEvent(message));
                                this.tradingGateway?.Send(this.GenerateOrderWorkingEvent(message));
                                break;
                            case "W": // Waiting (conditional order inactive state)
                                if (message.IsSetField(Tags.SecondaryOrderID))
                                {
                                    // Accepted Conditional Order
                                    this.tradingGateway?.Send(this.GenerateOrderAcceptedEvent(message));
                                }
                                else
                                {
                                    // Working Primary Order
                                    this.tradingGateway?.Send(this.GenerateOrderWorkingEvent(message));
                                }

                                break;
                            default:
                                this.Log.Error($"Cannot process event (FXCMOrdStatus {fxcmOrdStatus} not recognized).");
                                break;
                        }

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
                        throw ExceptionFactory.InvalidSwitchArgument(message.OrdStatus, nameof(message.OrdStatus));
                }
            });
        }

        private ZonedDateTime GetMarketDataTimestamp()
        {
            var dateTimeString = this.mdBidGroup.GetField(Tags.MDEntryDate) + this.mdBidGroup.GetField(Tags.MDEntryTime);
            return FxcmMessageHelper.ParseMarketDataTimestamp(dateTimeString);
        }

        private OrderRejected GenerateOrderRejectedEvent(ExecutionReport message)
        {
            var orderId = this.GetOrderId(message);
            var rejectedTime = FxcmMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));
            var rejectReason = message.GetField(FxcmTags.ErrorDetails);

            return new OrderRejected(
                this.accountId,
                orderId,
                rejectedTime,
                rejectReason,
                this.NewGuid(),
                this.TimeNow());
        }

        private OrderAccepted GenerateOrderAcceptedEvent(ExecutionReport message)
        {
            var orderId = this.GetOrderId(message);
            var orderIdBroker = new OrderIdBroker(message.GetField(Tags.OrderID));
            var orderLabel = new Label(message.GetField(Tags.SecondaryClOrdID));
            var acceptedTime = FxcmMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));

            return new OrderAccepted(
                this.accountId,
                orderId,
                orderIdBroker,
                orderLabel,
                acceptedTime,
                this.NewGuid(),
                this.TimeNow());
        }

        private OrderCancelled GenerateOrderCancelledEvent(ExecutionReport message)
        {
            var orderId = this.GetOrderId(message);
            var cancelledTime = FxcmMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));

            return new OrderCancelled(
                this.accountId,
                orderId,
                cancelledTime,
                this.NewGuid(),
                this.TimeNow());
        }

        private OrderModified GenerateOrderModifiedEvent(ExecutionReport message)
        {
            var orderId = this.GetOrderId(message);
            var orderIdBroker = new OrderIdBroker(message.GetField(Tags.OrderID));
            var orderType = FxcmMessageHelper.GetOrderType(message.GetField(Tags.OrdType));
            var quantity = Quantity.Create(message.GetInt(Tags.OrderQty));
            var price = FxcmMessageHelper.GetOrderPrice(orderType, message);
            var modifiedTime = FxcmMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));

            return new OrderModified(
                this.accountId,
                orderId,
                orderIdBroker,
                quantity,
                price,
                modifiedTime,
                this.NewGuid(),
                this.TimeNow());
        }

        private OrderWorking GenerateOrderWorkingEvent(ExecutionReport message)
        {
            var orderId = this.GetOrderId(message);
            var orderIdBroker = new OrderIdBroker(message.GetField(Tags.OrderID));
            var symbol = this.GetSymbol(message.GetField(Tags.Symbol));
            var orderLabel = new Label(message.GetField(Tags.SecondaryClOrdID));
            var orderSide = FxcmMessageHelper.GetOrderSide(message.GetField(Tags.Side));
            var orderType = FxcmMessageHelper.GetOrderType(message.GetField(Tags.OrdType));
            var quantity = Quantity.Create(message.GetInt(Tags.OrderQty));
            var price = FxcmMessageHelper.GetOrderPrice(orderType, message);
            var timeInForce = FxcmMessageHelper.GetTimeInForce(message.GetField(Tags.TimeInForce));
            var expireTime = FxcmMessageHelper.GetExpireTime(message);
            var workingTime = FxcmMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));

            return new OrderWorking(
                this.accountId,
                orderId,
                orderIdBroker,
                symbol,
                orderLabel,
                orderSide,
                orderType,
                quantity,
                price,
                timeInForce,
                expireTime,
                workingTime,
                this.NewGuid(),
                this.TimeNow());
        }

        private OrderExpired GenerateOrderExpiredEvent(ExecutionReport message)
        {
            var orderId = this.GetOrderId(message);
            var expiredTime = FxcmMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));

            return new OrderExpired(
                this.accountId,
                orderId,
                expiredTime,
                this.NewGuid(),
                this.TimeNow());
        }

        private OrderFilled GenerateOrderFilledEvent(ExecutionReport message)
        {
            var orderId = this.GetOrderId(message);
            var executionId = new ExecutionId(message.GetField(Tags.ExecID));
            var positionIdBroker = new PositionIdBroker(message.GetField(FxcmTags.PosID));
            var symbol = this.GetSymbol(message.GetField(Tags.Symbol));
            var orderSide = FxcmMessageHelper.GetOrderSide(message.GetField(Tags.Side));
            var filledQuantity = Quantity.Create(message.GetInt(Tags.CumQty));
            var averagePrice = Price.Create(message.GetDecimal(Tags.AvgPx));
            var transactionCurrency = message.GetField(Tags.Currency).ToEnum<Currency>();
            var executionTime = FxcmMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));

            return new OrderFilled(
                this.accountId,
                orderId,
                executionId,
                positionIdBroker,
                symbol,
                orderSide,
                filledQuantity,
                averagePrice,
                transactionCurrency,
                executionTime,
                this.NewGuid(),
                this.TimeNow());
        }

        private OrderPartiallyFilled GenerateOrderPartiallyFilledEvent(ExecutionReport message)
        {
            var orderId = this.GetOrderId(message);
            var executionId = new ExecutionId(message.GetField(Tags.ExecID));
            var positionIdBroker = new PositionIdBroker(message.GetField(FxcmTags.PosID));
            var symbol = this.GetSymbol(message.GetField(Tags.Symbol));
            var orderSide = FxcmMessageHelper.GetOrderSide(message.GetField(Tags.Side));
            var filledQuantity = Quantity.Create(message.GetInt(Tags.CumQty));
            var averagePrice = Price.Create(message.GetDecimal(Tags.AvgPx));
            var transactionCurrency = message.GetField(Tags.Currency).ToEnum<Currency>();
            var leavesQuantity = Quantity.Create(message.GetInt(Tags.LeavesQty));
            var executionTime = FxcmMessageHelper.ParseTransactionTime(message.GetField(Tags.TransactTime));

            return new OrderPartiallyFilled(
                this.accountId,
                orderId,
                executionId,
                positionIdBroker,
                symbol,
                orderSide,
                filledQuantity,
                leavesQuantity,
                averagePrice,
                transactionCurrency,
                executionTime,
                this.NewGuid(),
                this.TimeNow());
        }

        private OrderId GetOrderId(QuickFix.FIX44.Message message)
        {
            var brokerOrderId = message.GetField(Tags.OrderID);
            if (this.orderIdIndex.TryGetValue(brokerOrderId, out var orderId))
            {
                return orderId;
            }

            var newOrderId = new OrderId(message.GetField(Tags.ClOrdID));
            this.orderIdIndex[brokerOrderId] = newOrderId;
            return newOrderId;
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
