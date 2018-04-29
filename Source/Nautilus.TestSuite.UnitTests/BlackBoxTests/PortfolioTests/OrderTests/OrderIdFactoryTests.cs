//--------------------------------------------------------------------------------------------------
// <copyright file="OrderIdFactoryTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.PortfolioTests.OrderTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.BlackBox.Portfolio.Orders;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class OrderIdFactoryTests
    {
        private readonly ITestOutputHelper output;

        public OrderIdFactoryTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        internal void Create_ReturnsExpectedOrderId()
        {
            // Arrange
            var orderIdFactory = new OrderIdFactory(new Symbol("AUDUSD", Exchange.LMAX));

            // Act
            var result = orderIdFactory.Create(StubDateTime.Now()).ToString();

            var expected = $"19700101000001000_AUDUSD_";

            // Assert
            this.output.WriteLine(result);
            Assert.StartsWith(expected, result);
            Assert.EndsWith(orderIdFactory.OrderCount.ToString(), result);
        }
    }
}
