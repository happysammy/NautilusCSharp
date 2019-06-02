// -------------------------------------------------------------------------------------------------
// <copyright file="MsgPackRequestSerializerTests.cs" company="Nautech Systems Pty Ltd">
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
    using Nautilus.Data.Messages.Requests;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Serialization;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using NodaTime;
    using Xunit;
    using Xunit.Abstractions;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public sealed class MsgPackRequestSerializerTests
    {
        private readonly ITestOutputHelper output;

        public MsgPackRequestSerializerTests(ITestOutputHelper output)
        {
            // Fixture Setup
            this.output = output;
        }

        [Fact]
        internal void CanSerializeAndDeserialize_TickDataRequests()
        {
            // Arrange
            var serializer = new MsgPackRequestSerializer();

            var request = new TickDataRequest(
                new Symbol("AUDUSD", Venue.FXCM),
                StubZonedDateTime.UnixEpoch(),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(request);
            var unpacked = (TickDataRequest)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(request, unpacked);

            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_BarDataRequests()
        {
            // Arrange
            var serializer = new MsgPackRequestSerializer();

            var request = new BarDataRequest(
                new Symbol("AUDUSD", Venue.FXCM),
                new BarSpecification(1, Resolution.MINUTE, QuoteType.BID),
                StubZonedDateTime.UnixEpoch(),
                StubZonedDateTime.UnixEpoch(),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(request);
            var unpacked = (BarDataRequest)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(request, unpacked);

            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_InstrumentRequests()
        {
            // Arrange
            var serializer = new MsgPackRequestSerializer();

            var request = new InstrumentRequest(
                new Symbol("AUDUSD", Venue.FXCM),
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(request);
            var unpacked = (InstrumentRequest)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(request, unpacked);

            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }

        [Fact]
        internal void CanSerializeAndDeserialize_InstrumentsRequests()
        {
            // Arrange
            var serializer = new MsgPackRequestSerializer();

            var request = new InstrumentsRequest(
                Venue.FXCM,
                Guid.NewGuid(),
                StubZonedDateTime.UnixEpoch());

            // Act
            var packed = serializer.Serialize(request);
            var unpacked = (InstrumentsRequest)serializer.Deserialize(packed);

            // Assert
            Assert.Equal(request, unpacked);

            this.output.WriteLine(Convert.ToBase64String(packed));
            this.output.WriteLine(Encoding.UTF8.GetString(packed));
        }
    }
}
