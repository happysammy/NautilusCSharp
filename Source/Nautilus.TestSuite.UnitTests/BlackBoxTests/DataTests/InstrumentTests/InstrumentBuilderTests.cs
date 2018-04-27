// -------------------------------------------------------------------------------------------------
// <copyright file="InstrumentBuilderTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.BlackBoxTests.DataTests.InstrumentTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.BlackBox.Data.Instrument;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class InstrumentBuilderTests
    {
        private readonly ITestOutputHelper output;

        public InstrumentBuilderTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
        }

        [Fact]
        internal void Build_WithValidInstrument_ReturnsExpectedInstrument()
        {
            // Arrange
            var audusd = StubInstrumentFactory.AUDUSD();
            var instrumentBuilder = new InstrumentBuilder(audusd);

            // Act
            var result = instrumentBuilder.Build(StubDateTime.Now());

            // Assert
            Assert.Equal(audusd, result);
        }

        [Fact]
        internal void Update_WithValidInstrument_ReturnsExpectedInstrument()
        {
            // Arrange
            var audusd = StubInstrumentFactory.AUDUSD();
            var instrumentBuilder = new InstrumentBuilder(audusd);

            // Act
            var result = instrumentBuilder
               .Update(audusd)
               .Build(StubDateTime.Now());

            // Assert
            Assert.Equal(audusd, result);
        }

        [Fact]
        internal void Update_WithChanges_ReturnsNewInstrument()
        {
            // Arrange
            var audusd = StubInstrumentFactory.AUDUSD();
            var instrumentBuilder = new InstrumentBuilder(audusd);

            var instrument = new Instrument(
                new Symbol("AUDUSD", Exchange.FXCM),
                new EntityId("NONE"),
                new EntityId("NONE"),
                CurrencyCode.CAD,
                SecurityType.Bond,
                0.25m,
                10m,
                3,
                1000,
                1,
                1,
                1,
                1,
                10,
                10000,
                90,
                45,
                45,
                StubDateTime.Now());

            // Act
            var result = instrumentBuilder
               .Update(instrument)
               .Build(StubDateTime.Now());

            // Assert
            Assert.Equal(12, instrumentBuilder.Changes.Count);

            foreach (var change in instrumentBuilder.Changes)
            {
                this.output.WriteLine(change);
            }
        }
    }
}