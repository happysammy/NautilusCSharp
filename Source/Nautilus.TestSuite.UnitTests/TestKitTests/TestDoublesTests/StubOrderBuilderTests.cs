//--------------------------------------------------------------------------------------------------
// <copyright file="StubOrderBuilderTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.TestKitTests.TestDoublesTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.Stubs;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class StubOrderBuilderTests
    {
        [Fact]
        internal void Build_WithNoParametersModified_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            // Assert
            Assert.Equal(new Symbol("AUDUSD", new Venue("FXCM")), order.Symbol);
            Assert.Equal("O-123456", order.Id.Value);
            Assert.Equal("TEST_ORDER", order.Label.Value);
            Assert.Equal(OrderSide.Buy, order.OrderSide);
            Assert.Equal(OrderType.Stop, order.OrderType);
            Assert.Equal(Quantity.Create(100000), order.Quantity);
            Assert.Equal(Price.Create(1, 1), order.Price);
            Assert.Equal(TimeInForce.DAY, order.TimeInForce);
            Assert.Null(order.ExpireTime);
            Assert.Equal(StubZonedDateTime.UnixEpoch(), order.Timestamp);
        }

        [Fact]
        internal void Build_WithAllParametersModified_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder()
               .WithSymbol(new Symbol("AUDUSD", new Venue("FXCM")))
               .WithOrderId("O-678910")
               .WithLabel("TEST_ORDER2")
               .WithOrderSide(OrderSide.Sell)
               .WithQuantity(Quantity.Create(100000))
               .WithPrice(Price.Create(1.00000m, 5))
               .WithTimeInForce(TimeInForce.GTD)
               .WithExpireTime(StubZonedDateTime.UnixEpoch() + Period.FromMinutes(5).ToDuration())
               .WithTimestamp(StubZonedDateTime.UnixEpoch() + Period.FromMinutes(1).ToDuration())
               .BuildStopMarketOrder();

            // Assert
            Assert.Equal(new Symbol("AUDUSD", new Venue("FXCM")), order.Symbol);
            Assert.Equal("O-678910", order.Id.Value);
            Assert.Equal("TEST_ORDER2", order.Label.Value);
            Assert.Equal(OrderSide.Sell, order.OrderSide);
            Assert.Equal(OrderType.Stop, order.OrderType);
            Assert.Equal(Quantity.Create(100000), order.Quantity);
            Assert.Equal(Price.Create(1.00000m, 5), order.Price);
            Assert.Equal(TimeInForce.GTD, order.TimeInForce);
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Period.FromMinutes(5).ToDuration(), order.ExpireTime);
            Assert.Equal(StubZonedDateTime.UnixEpoch() + Period.FromMinutes(1).ToDuration(), order.Timestamp);
        }

        [Fact]
        internal void BuildMarket_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildMarketOrder();

            // Assert
            Assert.Equal(OrderType.Market, order.OrderType);
        }

        [Fact]
        internal void BuildStopMarket_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildStopMarketOrder();

            // Assert
            Assert.Equal(OrderType.Stop, order.OrderType);
        }

        [Fact]
        internal void BuildStopLimit_ThenReturnsExpectedOrder()
        {
            // Arrange
            // Act
            var order = new StubOrderBuilder().BuildStopLimitOrder();

            // Assert
            Assert.Equal(OrderType.StopLimit, order.OrderType);
        }
    }
}
