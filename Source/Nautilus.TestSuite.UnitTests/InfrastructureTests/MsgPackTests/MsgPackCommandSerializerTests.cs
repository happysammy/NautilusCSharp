// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackCommandSerializerTests.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.InfrastructureTests.MsgPackTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Execution.Messages.Commands;
    using Nautilus.MsgPack;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class MsgPackCommandSerializerTests
    {
        private readonly ITestOutputHelper output;

        public MsgPackCommandSerializerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
        }

        [Fact]
        internal void CanSerializeAndDeserialize_SubmitOrderCommands()
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
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_CancelOrderCommands()
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
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_ModifyOrderCommands()
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
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_CollateralInquiryCommands()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var command = new CollateralInquiry(Guid.NewGuid(), StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(command);
            var unpacked = serializer.Deserialize(packed) as CollateralInquiry;

            // Assert
            Assert.Equal(command, unpacked);
            this.output.WriteLine(Convert.ToBase64String(packed));
        }

        [Fact]
        internal void Deserialize_GivenSubmitOrder_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var hexString = "88a9436f6d6d616e644964da002436356138343132342d633935332d343438322d613162332d343130356630386433623731b0436f6d6d616e6454696d657374616d70b8313937302d30312d30315430303a30303a30302e3030305aab436f6d6d616e6454797065ac4f72646572436f6d6d616e64a54f72646572da017c3861613635333739366436323666366361623431353534343535353334343265343635383433346461373466373236343635373234393634626234663264333133393337333033303331333033313264333033303330333033303330326433303330333132643330333033313264333161393466373236343635373235333639363436356133343235353539613934663732363436353732353437393730363561363464343135323462343535346138353137353631366537343639373437396365303030313836613061393534363936643635373337343631366437306238333133393337333032643330333132643330333135343330333033613330333033613330333032653330333033303561613535303732363936333635613434653466346534356135346336313632363536636134346534663465343561623534363936643635343936653436366637323633363561333434343135396161343537383730363937323635353436393664363561343465346634653435ac4f72646572436f6d6d616e64ab5375626d69744f72646572a85472616465724964aa5472616465722d303031aa53747261746567794964a95343414c5045523031aa506f736974696f6e4964ad736f6d652d706f736974696f6e";

            var commandBytes = Convert.FromBase64String(hexString);

            // Act
            var command = serializer.Deserialize(commandBytes) as SubmitOrder;

            // Assert
            Assert.Equal(typeof(SubmitOrder), command?.GetType());
        }

        [Fact]
        internal void Deserialize_GivenCancelOrder_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var hexString = "86a9436f6d6d616e644964da002433373336343931642d353939332d343262302d623531662d613065613366386534626363b0436f6d6d616e6454696d657374616d70b8313937302d30312d30315430303a30303a30302e3030305aab436f6d6d616e6454797065ac4f72646572436f6d6d616e64a54f72646572da01823861613635333739366436323666366361623431353534343535353334343265343635383433346461373466373236343635373234393634613734663331333233333334333533366139346637323634363537323533363936343635613334323535353961393466373236343635373235343739373036356135346334393464343935346138353137353631366537343639373437396365303030313836613061393534363936643635373337343631366437306238333133393337333032643330333132643330333135343330333033613330333033613330333032653330333033303561613535303732363936333635613733313265333033303330333033306135346336313632363536636135353333313566353334636162353436393664363534393665343636663732363336356133343735343434616134353738373036393732363535343639366436356238333133393337333032643330333132643330333135343330333033613330333033613330333032653330333033303561ac4f72646572436f6d6d616e64ab43616e63656c4f72646572ac43616e63656c526561736f6ea745585049524544";

            var commandBytes = Convert.FromBase64String(hexString);

            // Act
            var command = serializer.Deserialize(commandBytes) as CancelOrder;

            // Assert
            Assert.Equal(typeof(CancelOrder), command?.GetType());
        }

        [Fact]
        internal void Deserialize_GivenModifyOrder_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var hexString = "86a9436f6d6d616e644964da002432613834356163332d346438382d346262372d393461332d616538353630323138393132b0436f6d6d616e6454696d657374616d70b8313937302d30312d30315430303a30303a30302e3030305aab436f6d6d616e6454797065ac4f72646572436f6d6d616e64a54f72646572da01823861613635333739366436323666366361623431353534343535353334343265343635383433346461373466373236343635373234393634613734663331333233333334333533366139346637323634363537323533363936343635613334323535353961393466373236343635373235343739373036356135346334393464343935346138353137353631366537343639373437396365303030313836613061393534363936643635373337343631366437306238333133393337333032643330333132643330333135343330333033613330333033613330333032653330333033303561613535303732363936333635613733313265333033303330333033306135346336313632363536636135353333313566353334636162353436393664363534393665343636663732363336356133343735343434616134353738373036393732363535343639366436356238333133393337333032643330333132643330333135343330333033613330333033613330333032653330333033303561ac4f72646572436f6d6d616e64ab4d6f646966794f72646572ad4d6f6469666965645072696365a7312e3030303031";

            var commandBytes = Convert.FromBase64String(hexString);

            // Act
            var command = serializer.Deserialize(commandBytes) as ModifyOrder;

            // Assert
            Assert.Equal(typeof(ModifyOrder), command?.GetType());
        }

        [Fact]
        internal void Deserialize_CollateralInquiry_FromPythonMsgPack_ReturnsExpectedCommand()
        {
            // Arrange
            var serializer = new MsgPackCommandSerializer();
            var hexString = "83a9436f6d6d616e644964da002431663530386338312d363934622d346463342d626464622d333536373764356563343764b0436f6d6d616e6454696d657374616d70b8313937302d30312d30315430303a30303a30302e3030305aab436f6d6d616e6454797065b1436f6c6c61746572616c496e7175697279";

            var commandBytes = Convert.FromBase64String(hexString);

            // Act
            var command = serializer.Deserialize(commandBytes) as CollateralInquiry;

            // Assert
            Assert.Equal(typeof(CollateralInquiry), command?.GetType());
        }
    }
}
