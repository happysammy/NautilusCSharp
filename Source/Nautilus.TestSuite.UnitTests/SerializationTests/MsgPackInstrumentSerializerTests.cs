// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackInstrumentSerializerTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.SerializationTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Nautilus.Serialization;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MsgPackInstrumentSerializerTests
    {
        private readonly ITestOutputHelper output;

        public MsgPackInstrumentSerializerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
        }

        [Fact]
        internal void CanSerializeAndDeserializeInstrument()
        {
            // Arrange
            var serializer = new MsgPackInstrumentSerializer();
            var instrument = StubInstrumentFactory.AUDUSD();

            // Act
            var packed = serializer.Serialize(instrument);
            var unpacked = serializer.Deserialize(packed);

            // Assert
            Assert.Equal(instrument, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }
    }
}
