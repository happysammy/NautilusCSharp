//--------------------------------------------------------------------------------------------------
// <copyright file="BarBuilderTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests.BuildersTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Data.Builders;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BarBuilderTests
    {
        [Fact]
        internal void GivenNewInstantion_InitializesCorrectly()
        {
            // Arrange
            var quote = new Tick(
                new Symbol("AUDUSD", Exchange.FXCM),
                Price.Create(1.00000m, 0.00001m),
                Price.Create(1.00010m, 0.00001m),
                StubDateTime.Now() + Period.FromSeconds(60).ToDuration());

            // Act
            var barBuilder = new BarBuilder(
                quote.Bid,
                quote.Timestamp - Period.FromSeconds(1).ToDuration());

            // Assert
            Assert.Equal(StubDateTime.Now() + Period.FromSeconds(59).ToDuration(), barBuilder.StartTime);
            Assert.Equal(quote.Timestamp - Period.FromSeconds(1).ToDuration(), barBuilder.Timestamp);
        }

        [Fact]
        internal void OnQuote_CorrectlyUpdatesBar()
        {
            // Arrange
            var quote1 = new Tick(
                new Symbol("AUDUSD", Exchange.FXCM),
                Price.Create(1.00000m, 0.00001m),
                Price.Create(1.00010m, 0.00001m),
                StubDateTime.Now() + Period.FromSeconds(60).ToDuration());

            var quote2 = new Tick(
                new Symbol("AUDUSD", Exchange.FXCM),
                Price.Create(1.00010m, 0.00001m),
                Price.Create(0.99980m, 0.00001m),
                StubDateTime.Now() + Period.FromSeconds(62).ToDuration());

            var quote3 = new Tick(
                new Symbol("AUDUSD", Exchange.FXCM),
                Price.Create(1.00020m, 0.00001m),
                Price.Create(0.99950m, 0.00001m),
                StubDateTime.Now() + Period.FromSeconds(64).ToDuration());

            var quote4 = new Tick(
                new Symbol("AUDUSD", Exchange.FXCM),
                Price.Create(0.99980m, 0.00001m),
                Price.Create(0.99950m, 0.00001m),
                StubDateTime.Now() + Period.FromSeconds(66).ToDuration());

            var barBuilder = new BarBuilder(
                quote1.Bid,
                quote1.Timestamp - Period.FromSeconds(1).ToDuration());

            // Act
            barBuilder.OnQuote(quote2.Bid, quote2.Timestamp);
            barBuilder.OnQuote(quote3.Bid, quote3.Timestamp);
            barBuilder.OnQuote(quote4.Bid, quote4.Timestamp);
            var bar = barBuilder.Build(StubDateTime.Now() + Period.FromSeconds(68).ToDuration());

            // Assert
            Assert.Equal(quote1.Bid, bar.Open);
            Assert.Equal(quote3.Bid, bar.High);
            Assert.Equal(quote4.Bid, bar.Low);
            Assert.Equal(quote4.Bid, bar.Close);
            Assert.Equal(Quantity.Create(4), bar.Volume);
            Assert.Equal(StubDateTime.Now() + Period.FromSeconds(68).ToDuration(), bar.Timestamp);
        }
    }
}
