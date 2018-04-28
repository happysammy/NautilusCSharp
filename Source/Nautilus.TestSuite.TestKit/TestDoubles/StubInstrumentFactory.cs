//--------------------------------------------------------------
// <copyright file="StubInstrumentFactory.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// Creates a stub instrument with default values.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubInstrumentFactory
    {
        public static Instrument AUDUSD()
        {
            var symbol = new Symbol($"AUDUSD", Exchange.FXCM);

            var instrument = new Instrument(
                    symbol,
                    new EntityId(symbol.ToString()),
                    new EntityId("AUD/USD"),
                    CurrencyCode.AUD,
                    SecurityType.Forex,
                    0.00001m,
                    1,
                    5,
                    1,
                    0,
                    0,
                    0,
                    0,
                    1,
                    50000000,
                    9,
                    1,
                    1,
                    StubDateTime.Now());

            return instrument;
        }

        public static Instrument EURUSD()
        {
            var symbol = new Symbol($"EURUSD", Exchange.FXCM);

            var instrument = new Instrument(
                    symbol,
                    new EntityId(symbol.ToString()),
                    new EntityId("EUR/USD"),
                    CurrencyCode.EUR,
                    SecurityType.Forex,
                    0.00001m,
                    1,
                    5,
                    1,
                    0,
                    0,
                    0,
                    0,
                    1,
                    50000000,
                    9,
                    1,
                    1,
                    StubDateTime.Now());

            return instrument;
        }

        public static Instrument USDJPY()
        {
            var symbol = new Symbol($"USDJPY", Exchange.FXCM);

            var instrument = new Instrument(
                   symbol,
                   new EntityId(symbol.ToString()),
                   new EntityId("USD/JPY"),
                   CurrencyCode.JPY,
                   SecurityType.Forex,
                   0.001m,
                   0.1m,
                   5,
                   1,
                   0,
                   0,
                   0,
                   0,
                   1,
                   50000000,
                   10,
                   1,
                   1,
                   StubDateTime.Now());

            return instrument;
        }
    }
}
