//--------------------------------------------------------------------------------------------------
// <copyright file="OrderIdPostfixRemoverTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.FixTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Fix;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class OrderIdPostfixRemoverTests
    {
        [Fact]
        internal void Remove_WithNormalOrderId_ReturnsExpectedOrderId()
        {
            // Arrange
            var orderId = "79812738_AUD_1";

            // Act
            var result = OrderIdPostfixRemover.Remove(orderId);

            // Assert
            Assert.Equal(orderId, result);
        }

        [Fact]
        internal void Remove_WithModifiedOrderId_ReturnsExpectedOrderId()
        {
            // Arrange
            var orderId = "79812738_AUD_1_R2";

            // Act
            var result = OrderIdPostfixRemover.Remove(orderId);

            // Assert
            Assert.Equal("79812738_AUD_1", result);
        }

        [Fact]
        internal void Remove_WithLongModifiedOrderId_ReturnsExpectedOrderId()
        {
            // Arrange
            var orderId = "79812738111_AUDUSD_51_R921";

            // Act
            var result = OrderIdPostfixRemover.Remove(orderId);

            // Assert
            Assert.Equal("79812738111_AUDUSD_51", result);
        }
    }
}
