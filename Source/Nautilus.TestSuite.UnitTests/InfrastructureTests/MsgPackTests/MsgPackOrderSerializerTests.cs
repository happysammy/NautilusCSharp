// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackOrderSerializerTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.InfrastructureTests.MsgPackTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Events;
    using Nautilus.MsgPack;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MsgPackOrderSerializerTests
    {
        private readonly ITestOutputHelper output;

        public MsgPackOrderSerializerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_market_orders()
        {
            // Arrange
            var serializer = new MsgPackOrderSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();

            // Act
            var packed = serializer.Serialize(order);
            var unpacked = serializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            this.output.WriteLine(ByteHelpers.ByteArrayToString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_limit_orders()
        {
            // Arrange
            var serializer = new MsgPackOrderSerializer();
            var order = new StubOrderBuilder().BuildStopLimitOrder();

            // Act
            var packed = serializer.Serialize(order);
            var unpacked = serializer.Deserialize(packed);

            // Assert
            Assert.Equal(order, unpacked);
            this.output.WriteLine(ByteHelpers.ByteArrayToString(packed));
        }
    }
}
