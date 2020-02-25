//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentBuilderTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.IntegrationTests.RedisTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Redis.Data.Internal;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Fixtures;
    using Nautilus.TestSuite.TestKit.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class InstrumentBuilderTests : TestBase
    {
        public InstrumentBuilderTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        internal void Build_WithValidInstrument_ReturnsExpectedInstrument()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            var instrumentBuilder = new InstrumentBuilder(audusd);

            // Act
            var result = instrumentBuilder.Build(StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal(audusd, result);
        }

        [Fact]
        internal void Update_WithValidInstrument_ReturnsExpectedInstrument()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            var instrumentBuilder = new InstrumentBuilder(audusd);

            // Act
            var result = instrumentBuilder
               .Update(audusd)
               .Build(StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal(audusd, result);
        }

        [Fact]
        internal void Update_WithChanges_ReturnsNewInstrument()
        {
            // Arrange
            var audusd = StubInstrumentProvider.AUDUSD();
            var instrumentBuilder = new InstrumentBuilder(audusd);

            var instrument = new Instrument(
                new Symbol("SPX500", new Venue("FXCM")),
                new BrokerSymbol("SPX500"),
                Currency.CAD,
                SecurityType.Bond,
                2,
                0,
                1,
                1,
                1,
                1,
                Price.Create(0.01m, 2),
                Quantity.Create(1m),
                Quantity.Create(10m),
                Quantity.Create(10000m),
                45,
                45,
                StubZonedDateTime.UnixEpoch());

            // Act
            var result = instrumentBuilder
               .Update(instrument)
               .Build(StubZonedDateTime.UnixEpoch());

            // Assert
            Assert.Equal(8, instrumentBuilder.Changes.Count);

            foreach (var change in instrumentBuilder.Changes)
            {
                this.Output.WriteLine(change);
            }
        }
    }
}
