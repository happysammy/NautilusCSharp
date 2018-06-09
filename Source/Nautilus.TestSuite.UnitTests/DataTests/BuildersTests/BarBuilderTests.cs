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
        internal void OnQuote_CorrectlyUpdatesBar()
        {
            // Arrange
            var timestamp = StubDateTime.Now();

            var quote1 = Price.Create(1.00010m, 5);

            var quote2 = Price.Create(0.99980m, 5);

            var quote3 = Price.Create(0.99950m, 5);

            var quote4 = Price.Create(0.99950m, 5);

            var barBuilder = new BarBuilder();

            // Act
            barBuilder.OnQuote(quote1, timestamp + Period.FromSeconds(60).ToDuration());
            barBuilder.OnQuote(quote2, timestamp + Period.FromSeconds(62).ToDuration());
            barBuilder.OnQuote(quote3, timestamp + Period.FromSeconds(64).ToDuration());
            barBuilder.OnQuote(quote4, timestamp + Period.FromSeconds(66).ToDuration());

            var bar = barBuilder.Build(StubDateTime.Now() + Period.FromSeconds(68).ToDuration());

            // Assert
            Assert.Equal(quote1, bar.Open);
            Assert.Equal(quote1, bar.High);
            Assert.Equal(quote4, bar.Low);
            Assert.Equal(quote4, bar.Close);
            Assert.Equal(Quantity.Create(4), bar.Volume);
            Assert.Equal(StubDateTime.Now() + Period.FromSeconds(68).ToDuration(), bar.Timestamp);
        }
    }
}
