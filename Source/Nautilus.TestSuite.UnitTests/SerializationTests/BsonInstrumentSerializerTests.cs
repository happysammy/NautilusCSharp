// -------------------------------------------------------------------------------------------------
// <copyright file="BsonInstrumentSerializerTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   https://nautechsystems.io
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.SerializationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Nautilus.Serialization.DataSerializers;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.Fixtures;
    using Nautilus.TestSuite.TestKit.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class BsonInstrumentSerializerTests : TestBase
    {
        public BsonInstrumentSerializerTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        internal void CanSerializeAndDeserializeInstrument()
        {
            // Arrange
            var serializer = new InstrumentDataSerializer();
            var instrument = StubInstrumentProvider.AUDUSD();

            // Act
            var packed = serializer.Serialize(instrument);
            var unpacked = serializer.Deserialize(packed);

            // Assert
            Assert.Equal(instrument, unpacked);
            this.Output.WriteLine(Convert.ToBase64String(packed));
            this.Output.WriteLine(Encoding.UTF8.GetString(packed));
        }
    }
}
