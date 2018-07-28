// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackEventSerializerTests.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.InfrastructureTests.MsgPackTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Commands;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.MsgPack;
    using Nautilus.TestSuite.TestKit;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MsgPackCommandSerializerTests
    {
        private readonly ITestOutputHelper output;

        public MsgPackCommandSerializerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_submit_order_commands()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();
            var command = new SubmitOrder(
                order,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(command);
            var unpacked = serializer.Deserialize(packed) as SubmitOrder;

            // Assert
            Assert.Equal(command, unpacked);
            this.output.WriteLine(HexConverter.ByteArrayToHexString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_cancel_order_commands()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();
            var command = new CancelOrder(
                order,
                "EXPIRED",
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(command);
            var unpacked = serializer.Deserialize(packed) as CancelOrder;

            // Assert
            Assert.Equal(command, unpacked);
            this.output.WriteLine(HexConverter.ByteArrayToHexString(packed));
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize_modify_order_commands()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var order = new StubOrderBuilder().BuildMarketOrder();
            var command = new ModifyOrder(
                order,
                Price.Create(1.50000m, 5),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(command);
            var unpacked = serializer.Deserialize(packed) as ModifyOrder;

            // Assert
            Assert.Equal(command, unpacked);
            this.output.WriteLine(HexConverter.ByteArrayToHexString(packed));
        }

        [Fact]
        internal void Test_can_deserialize_submit_order_from_python_msgpack()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var hexString = "85ac636f6d6d616e645f74797065ad6f726465725f636f6d6d616e64a56f72646572" +
                            "da016e38616136373337393664363236663663616234313535343435353533343432" +
                            "65343635383433346461383666373236343635373235663639363461373466333133" +
                            "32333333343335333661353663363136323635366361633533343334313463353034" +
                            "35353233303331356635333463616136663732363436353732356637333639363436" +
                            "35613334323535353961613666373236343635373235663734373937303635613634" +
                            "64343135323462343535346138373137353631366537343639373437396365303030" +
                            "31383661306139373436393664363537333734363136643730623733323330333133" +
                            "38326433303337326433323338353433303331336133333337336133303339326533" +
                            "36333333366135373037323639363336356134346534663465343561643734363936" +
                            "64363535663639366535663636366637323633363561333434343135396162363537" +
                            "3837303639373236353566373436393664363561343465346634653435aa636f6d6d" +
                            "616e645f6964da002439396237643132362d613162302d343639332d386465612d32" +
                            "3235366137343464623439b1636f6d6d616e645f74696d657374616d70b831393730" +
                            "2d30312d30315430303a30303a30302e3030305aad6f726465725f636f6d6d616e64" +
                            "ac7375626d69745f6f72646572";

            var commandBytes = HexConverter.HexStringToByteArray(hexString);

            // Act
            var command = serializer.Deserialize(commandBytes) as SubmitOrder;

            // Assert
            Assert.Equal(typeof(SubmitOrder), command?.GetType());
        }
    }
}
