//--------------------------------------------------------------------------------------------------
// <copyright file="InstrumentTests.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.UnitTests.DomainModelTests.EntitiesTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Core.Extensions;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;
    using Nautilus.Redis;
    using Nautilus.TestSuite.TestKit.TestDoubles;
    using ServiceStack.Text;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class InstrumentTests
    {
        public InstrumentTests()
        {
            RedisServiceStack.ConfigureServiceStack();
        }

        [Fact]
        internal void Test_can_serialize_and_deserialize()
        {
            // Arrange
            var instrument = StubInstrumentFactory.AUDUSD();

            // Act
            var serialized = JsonSerializer.SerializeToString(instrument);
            var deserialized = JsonSerializer.DeserializeFromString<JsonObject>(serialized);

            var symbol = deserialized["Symbol"].ToStringDictionary();

            var deserializedInstrument = new Instrument(
                new Symbol(symbol["Code"], symbol["Venue"].ToEnum<Venue>()),
                new InstrumentId(symbol["Value"]),
                new BrokerSymbol("AUD/USD"),
                deserialized["CurrencyCode"].ToEnum<CurrencyCode>(),
                deserialized["SecurityType"].ToEnum<SecurityType>(),
                Convert.ToInt32(deserialized["TickDecimals"]),
                Convert.ToDecimal(deserialized["TickSize"]),
                Convert.ToInt32(deserialized["TickValue"]),
                Convert.ToInt32(deserialized["TargetDirectSpread"]),
                Convert.ToInt32(deserialized["RoundLotSize"]),
                Convert.ToInt32(deserialized["ContractSize"]),
                Convert.ToInt32(deserialized["MinStopDistanceEntry"]),
                Convert.ToInt32(deserialized["MinLimitDistanceEntry"]),
                Convert.ToInt32(deserialized["MinStopDistance"]),
                Convert.ToInt32(deserialized["MinLimitDistance"]),
                Convert.ToInt32(deserialized["MinTradeSize"]),
                Convert.ToInt32(deserialized["MaxTradeSize"]),
                Convert.ToInt32(deserialized["MarginRequirement"]),
                Convert.ToDecimal(deserialized["RollOverInterestBuy"]),
                Convert.ToDecimal(deserialized["RollOverInterestSell"]),
                deserialized["Timestamp"].ToZonedDateTimeFromIso());

            // Assert
            Assert.Equal(instrument, deserializedInstrument);
        }
    }
}
