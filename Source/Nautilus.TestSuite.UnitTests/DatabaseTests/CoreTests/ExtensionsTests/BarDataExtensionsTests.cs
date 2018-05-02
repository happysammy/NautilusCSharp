// -------------------------------------------------------------------------------------------------
// <copyright file="BarDataExtensionsTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DatabaseTests.CoreTests.ExtensionsTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Database.Core.Extensions;
    using Nautilus.Database.Core.Types;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BarDataExtensionsTests
    {
        [Fact]
        internal void ToBarData_WithValidBarBytes_ReturnsExpectedBar()
        {
            // Arrange
            var barSpec = StubSymbolBarData.AUDUSD();
            var bar = new BarData(
                0.80000M,
                0.80010M,
                0.79990M,
                0.80005M,
                500000,
                StubZonedDateTime.UnixEpoch());
            var barBytes = bar.ToUtf8Bytes();

            // Act
            var result = barBytes.ToBarData();

            // Assert
            Assert.Equal(bar, result);
        }

        [Fact]
        internal void ToBarData_WithValidBarString_ReturnsExpectedBar()
        {
            // Arrange
            var barSpec = StubSymbolBarData.AUDUSD();
            var bar = new BarData(
                0.80000M,
                0.80010M,
                0.79990M,
                0.80005M,
                500000,
                StubZonedDateTime.UnixEpoch());
            var barString = bar.ToString();

            // Act
            var result = barString.ToBarData();

            // Assert
            Assert.Equal(bar, result);
        }
    }
}
