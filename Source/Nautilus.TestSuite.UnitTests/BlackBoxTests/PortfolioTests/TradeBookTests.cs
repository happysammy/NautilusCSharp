//--------------------------------------------------------------------------------------------------
// <copyright file="TradeBookTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.PortfolioTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Nautilus.BlackBox.Portfolio;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Aggregates;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Extensions;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class TradeBookTests
    {
        private readonly ITestOutputHelper output;
        private readonly MockLoggingAdatper mockLoggingAdatper;
        private readonly TradeBook tradeBook;

        public TradeBookTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;

            var setupFactory = new StubSetupContainerFactory();
            var setupContainer = setupFactory.Create();
            this.mockLoggingAdatper = setupFactory.LoggingAdatper;

            var instrument = StubInstrumentFactory.AUDUSD();
            this.tradeBook = new TradeBook(setupContainer, instrument.Symbol);
        }

        [Fact]
        internal void GetAllActiveOrderIds_NewlyZonedDateTimeiated_ReturnsEmptyList()
        {
            // Arrange
            // Act
            var result = this.tradeBook.GetAllActiveOrderIds().ToList().Count;

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        internal void GetTrade_NoTrades_ReturnsNull()
        {
            // Arrange
            // Act
            var result = this.tradeBook.GetTradesByTradeType(new TradeType("Scalp"));

            // Assert
            Assert.Equal(new List<Trade>(), result);
        }

        [Fact]
        internal void GetTradeForOrder_NoTrades_ReturnsNullTrade()
        {
            // Arrange
            // Act
            var result = this.tradeBook.GetTradeForOrder(new EntityId("some_order_id"));

            // Assert
            Assert.True(result.IsFailure);
        }

        [Fact]
        internal void GetTrade_AddedStubTrade_ReturnsTrade()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            this.tradeBook.AddTrade(trade);

            // Act
            var result = this.tradeBook.GetTradesByTradeType(trade.TradeType);

            // Assert
            Assert.Equal(trade, result[0]);
        }

        [Fact]
        internal void GetTradeForOrder_AddedStubTrade_ReturnsTrade()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            this.tradeBook.AddTrade(trade);

            var orderIdList = trade.OrderIdList.ToList();

            // Act
            var result = this.tradeBook.GetTradeForOrder(orderIdList[0]);

            // Assert
            Assert.Equal(trade, result.Value);
        }

        [Fact]
        internal void GetAllActiveOrderIds_AddedStubTrade_ReturnsAllOrderIds()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            this.tradeBook.AddTrade(trade);

            // Act
            var result = this.tradeBook.GetAllActiveOrderIds();

            // Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        internal void GetTradeStatus_AllOrdersCancelled_ReturnsTradeStatusCompletedMarketPositionFlat()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            this.tradeBook.AddTrade(trade);

            var order1 = trade.TradeUnits[0].Entry;

            // Act
            trade.Apply(StubEventMessages.OrderCancelledEvent(trade.TradeUnits[0].Entry));
            trade.Apply(StubEventMessages.OrderCancelledEvent(trade.TradeUnits[0].StopLoss));
            trade.Apply(StubEventMessages.OrderCancelledEvent(trade.TradeUnits[0].ProfitTarget.Value));

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order1.OrderStatus);
            Assert.Equal(TradeStatus.Completed, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].TradeStatus);
            Assert.Equal(MarketPosition.Flat, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].MarketPosition);
        }

        [Fact]
        internal void GetTradeStatus_EntryExpiredOtherOrdersCancelled_ReturnsTradeStatusCompletedMarketPositionFlat()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            this.tradeBook.AddTrade(trade);

            var order1 = trade.TradeUnits[0].Entry;

            // Act
            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[0].Entry));
            trade.Apply(StubEventMessages.OrderExpiredEvent(trade.TradeUnits[0].Entry));
            trade.Apply(StubEventMessages.OrderCancelledEvent(trade.TradeUnits[0].StopLoss));
            trade.Apply(StubEventMessages.OrderCancelledEvent(trade.TradeUnits[0].ProfitTarget.Value));

            // Assert
            Assert.Equal(OrderStatus.Expired, order1.OrderStatus);
            Assert.Equal(TradeStatus.Completed, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].TradeStatus);
            Assert.Equal(MarketPosition.Flat, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].MarketPosition);
        }

        [Fact]
        internal void GetTradeStatus_BuyEntryOrderPartiallyFilled_ReturnsTradeStatusActiveMarketPositionLong()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            this.tradeBook.AddTrade(trade);

            var order = trade.TradeUnits[0].Entry;

            // Act
            trade.Apply(StubEventMessages.OrderWorkingEvent(order));
            trade.Apply(StubEventMessages.OrderPartiallyFilledEvent(order, 50000, 50000));

            // Assert
            Assert.Equal(100000, order.Quantity.Value);
            Assert.Equal(50000, order.FilledQuantity.Value);
            Assert.Equal(OrderStatus.PartiallyFilled, order.OrderStatus);
            Assert.Equal(TradeStatus.Active, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].TradeStatus);
            Assert.Equal(MarketPosition.Long, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].MarketPosition);
        }

        [Fact]
        internal void GetTradeStatus_BuyEntryOrderFilled_ReturnsTradeStatusActiveMarketPositionLong()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyOneUnit();
            this.tradeBook.AddTrade(trade);

            // Act
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            // Assert
            Assert.Equal(TradeStatus.Active, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].TradeStatus);
            Assert.Equal(MarketPosition.Long, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].MarketPosition);
        }

        [Fact]
        internal void GetTradeStatus_SellEntryOrderFilled_ReturnsTradeStatusActiveMarketPositionShort()
        {
            // Arrange
            var trade = StubTradeBuilder.SellOneUnit();
            this.tradeBook.AddTrade(trade);

            // Act
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            // Assert
            Assert.Equal(TradeStatus.Active, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].TradeStatus);
            Assert.Equal(MarketPosition.Short, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].MarketPosition);
        }

        [Fact]
        internal void GetTradeStatus_SellEntryOrderFilledThenStoplossFilled_ReturnsTradeStatusCompletedMarketPositionFlat()
        {
            // Arrange
            var trade = StubTradeBuilder.SellOneUnit();
            this.tradeBook.AddTrade(trade);

            // Act
            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[0].Entry));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));

            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[0].StopLoss));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].StopLoss));

            trade.Apply(StubEventMessages.OrderCancelledEvent(trade.TradeUnits[0].ProfitTarget.Value));

            // Assert
            Assert.Equal(TradeStatus.Completed, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].TradeStatus);
            Assert.Equal(MarketPosition.Flat, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].MarketPosition);
        }

        [Fact]
        internal void GetTradeStatus_BuyEntryOrdersFilledThenFirstProfitTargetFilled_ReturnsTradeStatusActiveMarketPositionLong()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyThreeUnits();
            this.tradeBook.AddTrade(trade);

            // Act
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[1].Entry));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[2].Entry));

            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].ProfitTarget.Value));
            trade.Apply(StubEventMessages.OrderCancelledEvent(trade.TradeUnits[0].StopLoss));

            // Assert
            Assert.Equal(TradeStatus.Active, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].TradeStatus);
            Assert.Equal(MarketPosition.Long, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].MarketPosition);
        }

        [Fact]
        internal void GetTradeStatus_BuyEntryOrdersFilledThenAllProfitTargetsFilledThenStoppedOut_ReturnsTradeStatusCompletedMarketPositionFlat()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyThreeUnits();
            this.tradeBook.AddTrade(trade);

            // Act
            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[0].Entry));
            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[1].Entry));
            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[2].Entry));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[1].Entry));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[2].Entry));

            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[0].ProfitTarget.Value));
            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[0].StopLoss));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].ProfitTarget.Value));
            trade.Apply(StubEventMessages.OrderCancelledEvent(trade.TradeUnits[0].StopLoss));

            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[1].ProfitTarget.Value));
            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[1].StopLoss));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[1].ProfitTarget.Value));
            trade.Apply(StubEventMessages.OrderCancelledEvent(trade.TradeUnits[1].StopLoss));

            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[2].StopLoss));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[2].StopLoss));

            // the null profit target order 3 is queried IsCompleted=True

            // Assert
            Assert.Equal(TradeStatus.Completed, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].TradeStatus);
            Assert.Equal(MarketPosition.Flat, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].MarketPosition);
        }

        [Fact]
        internal void GetTradeStatus_SellEntryOrdersFilledThenOneProfitTargetsFilledThenStoppedOut_ReturnsTradeStatusCompletedMarketPositionFlat()
        {
            // Arrange
            var trade = StubTradeBuilder.SellThreeUnits();
            this.tradeBook.AddTrade(trade);

            // Act
            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[0].Entry));
            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[1].Entry));
            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[2].Entry));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[1].Entry));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[2].Entry));

            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[0].ProfitTarget.Value));
            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[0].StopLoss));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].ProfitTarget.Value));
            trade.Apply(StubEventMessages.OrderCancelledEvent(trade.TradeUnits[0].StopLoss));

            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[1].ProfitTarget.Value));
            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[1].StopLoss));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[1].StopLoss));
            trade.Apply(StubEventMessages.OrderCancelledEvent(trade.TradeUnits[1].ProfitTarget.Value));

            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[2].StopLoss));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[2].StopLoss));

            // the null profit target order 3 is queried IsCompleted=True

            // Assert
            Assert.Equal(TradeStatus.Completed, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].TradeStatus);
            Assert.Equal(MarketPosition.Flat, this.tradeBook.GetTradesByTradeType(trade.TradeType)[0].MarketPosition);
        }

        [Fact]
        internal void Process_WhenTradeComplete_RemovesTradeFromTradeBook()
        {
            // Arrange
            var trade = StubTradeBuilder.BuyThreeUnits();
            this.tradeBook.AddTrade(trade);

            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[0].Entry));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].Entry));
            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[1].Entry));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[1].Entry));
            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[2].Entry));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[2].Entry));

            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[0].StopLoss));
            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[0].ProfitTarget.Value));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[0].ProfitTarget.Value));
            trade.Apply(StubEventMessages.OrderCancelledEvent(trade.TradeUnits[0].StopLoss));

            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[1].StopLoss));
            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[1].ProfitTarget.Value));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[1].ProfitTarget.Value));
            trade.Apply(StubEventMessages.OrderCancelledEvent(trade.TradeUnits[1].StopLoss));

            trade.Apply(StubEventMessages.OrderWorkingEvent(trade.TradeUnits[2].StopLoss));
            trade.Apply(StubEventMessages.OrderFilledEvent(trade.TradeUnits[2].StopLoss));

            // Act
            // the null profit target order 3 is queried IsCompleted=True
            this.tradeBook.Process(trade.TradeType);

            // Assert
            LogDumper.Dump(this.mockLoggingAdatper, this.output);

            CustomAssert.EventuallyContains(
                "TradeBook-AUDUSD.FXCM: Trade removed (TestTrade), TradeStatus=Completed)",
                this.mockLoggingAdatper,
                EventuallyContains.TimeoutMilliseconds,
                EventuallyContains.PollIntervalMilliseconds);
        }
    }
}