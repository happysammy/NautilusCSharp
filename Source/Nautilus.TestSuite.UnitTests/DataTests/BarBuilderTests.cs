//--------------------------------------------------------------------------------------------------
// <copyright file="BarBuilderTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DataTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Data;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class BarBuilderTests
    {
        [Fact]
        internal void Build_WithOneQuotes_ReturnsExpectedBar()
        {
            // Arrange
            var timestamp = StubDateTime.Now();
            var quote = Price.Create(1.00000m, 5);

            var barBuilder = new BarBuilder();

            // Act
            barBuilder.OnQuote(quote, timestamp);

            var bar = barBuilder.Build(timestamp);

            // Assert
            Assert.Equal(quote, bar.Open);
            Assert.Equal(quote, bar.High);
            Assert.Equal(quote, bar.Low);
            Assert.Equal(quote, bar.Close);
            Assert.Equal(Quantity.Create(1), bar.Volume);
            Assert.Equal(StubDateTime.Now(), bar.Timestamp);
        }

        [Fact]
        internal void Build_WithVariousQuotes1_ReturnsExpectedBar()
        {
            // Arrange
            var timestamp = StubDateTime.Now();
            var quote1 = Price.Create(1.00010m, 5);
            var quote2 = Price.Create(0.99980m, 5);
            var quote3 = Price.Create(0.99950m, 5);
            var quote4 = Price.Create(0.99950m, 5);

            var barBuilder = new BarBuilder();

            // Act
            barBuilder.OnQuote(quote1, timestamp + Period.FromSeconds(1).ToDuration());
            barBuilder.OnQuote(quote2, timestamp + Period.FromSeconds(2).ToDuration());
            barBuilder.OnQuote(quote3, timestamp + Period.FromSeconds(3).ToDuration());
            barBuilder.OnQuote(quote4, timestamp + Period.FromSeconds(4).ToDuration());

            var bar = barBuilder.Build(timestamp + Period.FromSeconds(5).ToDuration());

            // Assert
            Assert.Equal(quote1, bar.Open);
            Assert.Equal(quote1, bar.High);
            Assert.Equal(quote4, bar.Low);
            Assert.Equal(quote4, bar.Close);
            Assert.Equal(Quantity.Create(4), bar.Volume);
            Assert.Equal(timestamp + Period.FromSeconds(5).ToDuration(), bar.Timestamp);
        }

        [Fact]
        internal void Build_WithVariousQuotes2_ReturnsExpectedBar()
        {
            // Arrange
            var timestamp = StubDateTime.Now();
            var quote1 = Price.Create(1.00010m, 5);
            var quote2 = Price.Create(0.99980m, 5);
            var quote3 = Price.Create(1.00090m, 5);
            var quote4 = Price.Create(0.99800m, 5);

            var barBuilder = new BarBuilder();

            // Act
            barBuilder.OnQuote(quote1, timestamp + Period.FromMinutes(1).ToDuration());
            barBuilder.OnQuote(quote2, timestamp + Period.FromMinutes(2).ToDuration());
            barBuilder.OnQuote(quote3, timestamp + Period.FromMinutes(3).ToDuration());
            barBuilder.OnQuote(quote4, timestamp + Period.FromMinutes(4).ToDuration());

            var bar = barBuilder.Build(timestamp + Period.FromMinutes(5).ToDuration());

            // Assert
            Assert.Equal(quote1, bar.Open);
            Assert.Equal(quote3, bar.High);
            Assert.Equal(quote4, bar.Low);
            Assert.Equal(quote4, bar.Close);
            Assert.Equal(Quantity.Create(4), bar.Volume);
            Assert.Equal(timestamp + Period.FromMinutes(5).ToDuration(), bar.Timestamp);
        }

        [Fact]
        internal void Build_WithVariousQuotes3_ReturnsExpectedBar()
        {
            // Arrange
            var timestamp = StubDateTime.Now();
            var quote1 = Price.Create(0.99999m, 5);
            var quote2 = Price.Create(1.00001m, 5);
            var quote3 = Price.Create(1.00000m, 5);
            var quote4 = Price.Create(1.00000m, 5);

            var barBuilder = new BarBuilder();

            // Act
            barBuilder.OnQuote(quote1, timestamp);
            barBuilder.OnQuote(quote2, timestamp);
            barBuilder.OnQuote(quote3, timestamp + Period.FromMilliseconds(1).ToDuration());
            barBuilder.OnQuote(quote4, timestamp + Period.FromMilliseconds(2).ToDuration());

            var bar = barBuilder.Build(timestamp + Period.FromMilliseconds(2).ToDuration());

            // Assert
            Assert.Equal(quote1, bar.Open);
            Assert.Equal(quote2, bar.High);
            Assert.Equal(quote1, bar.Low);
            Assert.Equal(quote4, bar.Close);
            Assert.Equal(Quantity.Create(4), bar.Volume);
            Assert.Equal(timestamp + Period.FromMilliseconds(2).ToDuration(), bar.Timestamp);
        }
    }
}
