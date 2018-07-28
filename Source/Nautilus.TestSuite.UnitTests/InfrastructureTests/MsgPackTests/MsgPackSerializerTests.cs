// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackSerializerTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.InfrastructureTests.MsgPackTests
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.MsgPack;
    using Nautilus.TestSuite.TestKit;
    using Xunit;
    using global::MsgPack;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MsgPackSerializerTests
    {
        [Fact]
        internal void Test_can_serialize_and_deserialize()
        {
            // Arrange
            var msgPack = new MessagePackObjectDictionary
            {
                {new MessagePackObject("header"), "the_msg_header"},
                {new MessagePackObject("value1"), 42}
            }.Freeze();

            // Act
            var packed = MsgPackSerializer.Serialize(msgPack);
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(packed);

            // Assert
            Assert.Equal(msgPack.Count, unpacked.Count);
        }

        [Fact]
        internal void Test_can_deserialize_from_python_packed_msg()
        {
            // Arrange
            // From Python dict.
            // message = {
            // 'header': 'order_cancelled',
            // 'symbol': 'AUDUSD.FXCM',
            // 'order_id': 'O123456',
            // 'timestamp': '1970-01-01T00:00:00.000Z'}
            var python_hex = "84a6686561646572af6f726465725f63616e63656c6c6564a673796d626f6cab415544" +
                             "5553442e4658434da86f726465725f6964a74f313233343536a974696d657374616d70" +
                             "b8313937302d30312d30315430303a30303a30302e3030305a";
            var data = HexConverter.HexStringToByteArray(python_hex);

            // Act
            var unpacked = MsgPackSerializer.Deserialize<MessagePackObjectDictionary>(data);

            // Assert
            Assert.Equal(4, unpacked.Count);
            Assert.Equal("order_cancelled", unpacked["header"]);
            Assert.Equal("AUDUSD.FXCM", unpacked["symbol"]);
            Assert.Equal("O123456", unpacked["order_id"]);
            Assert.Equal("1970-01-01T00:00:00.000Z", unpacked["timestamp"]);
        }
    }
}
