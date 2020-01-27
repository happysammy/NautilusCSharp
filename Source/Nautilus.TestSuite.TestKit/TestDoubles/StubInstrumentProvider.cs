//--------------------------------------------------------------------------------------------------
// <copyright file="StubInstrumentProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Identifiers;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubInstrumentProvider
    {
        public static ForexInstrument AUDUSD()
        {
            var instrument = new ForexInstrument(
                    new Symbol($"AUDUSD", new Venue("FXCM")),
                    new BrokerSymbol("AUD/USD"),
                    5,
                    0.00001m,
                    1000,
                    0,
                    0,
                    0,
                    0,
                    1,
                    50000000,
                    1,
                    1,
                    StubZonedDateTime.UnixEpoch());

            return instrument;
        }

        public static ForexInstrument EURUSD()
        {
            var instrument = new ForexInstrument(
                    new Symbol($"EURUSD", new Venue("FXCM")),
                    new BrokerSymbol("EUR/USD"),
                    5,
                    0.00001m,
                    1000,
                    0,
                    0,
                    0,
                    0,
                    1,
                    50000000,
                    1,
                    1,
                    StubZonedDateTime.UnixEpoch());

            return instrument;
        }

        public static ForexInstrument USDJPY()
        {
            var instrument = new ForexInstrument(
                    new Symbol("USDJPY", new Venue("FXCM")),
                    new BrokerSymbol("USD/JPY"),
                    3,
                    0.001m,
                    1000,
                    0,
                    0,
                    0,
                    0,
                    1,
                    50000000,
                    1,
                    1,
                    StubZonedDateTime.UnixEpoch());

            return instrument;
        }
    }
}
