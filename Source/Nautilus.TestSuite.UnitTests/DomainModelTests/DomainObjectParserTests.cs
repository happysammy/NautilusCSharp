//--------------------------------------------------------------------------------------------------
// <copyright file="DomainObjectParserTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Factories;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public sealed class DomainObjectParserTests
    {
        private readonly Symbol symbol;

        public DomainObjectParserTests()
        {
            // Fixture Setup
            this.symbol = new Symbol("AUDUSD", Venue.FXCM);
        }

        [Fact]
        internal void Create_WithValidString_ReturnsExpectedTick()
        {
            // Arrange
            var tick = new Tick(this.symbol, 1.00000m, 1.00000m, StubZonedDateTime.UnixEpoch());

            // Act
            var tickString = tick.ToString();
            var result = DomainObjectParser.ParseTick(this.symbol, tickString);

            // Assert
            Assert.Equal(tick, result);
        }

        [Fact]
        internal void Create_WithValidString_ReturnsExpectedBar()
        {
            // Arrange
            var bar = new Bar(
                Price.Create(0.80000m, 5),
                Price.Create(0.80010m, 5),
                Price.Create(0.79990m, 5),
                Price.Create(0.80001m, 5),
                Quantity.Create(1000000),
                StubZonedDateTime.UnixEpoch());

            // Act
            var barString = bar.ToString();
            var result = DomainObjectParser.ParseBar(barString);

            // Assert
            Assert.Equal(bar, result);
        }

        [Fact]
        internal void Create_WithValidString_ReturnsExpectedBarSpec()
        {
            // Arrange
            var barSpec1 = new BarSpecification(1, Resolution.MINUTE, QuoteType.BID);
            var barSpec2 = new BarSpecification(1, Resolution.HOUR, QuoteType.MID);

            var string1 = barSpec1.ToString();
            var string2 = barSpec2.ToString();

            // Act
            var result1 = DomainObjectParser.ParseBarSpecification(string1);
            var result2 = DomainObjectParser.ParseBarSpecification(string2);

            // Assert
            Assert.Equal(barSpec1, result1);
            Assert.Equal(barSpec2, result2);
        }
    }
}
