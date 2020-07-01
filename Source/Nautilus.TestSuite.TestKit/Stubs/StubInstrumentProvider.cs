//--------------------------------------------------------------------------------------------------
// <copyright file="StubInstrumentProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  https://nautechsystems.io
//
//  Licensed under the GNU Lesser General Public License Version 3.0 (the "License");
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at https://www.gnu.org/licenses/lgpl-3.0.en.html
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// </copyright>
//--------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nautilus.DomainModel.Entities;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;

namespace Nautilus.TestSuite.TestKit.Stubs
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Test Suite")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public static class StubInstrumentProvider
    {
        public static ForexInstrument AUDUSD()
        {
            var instrument = new ForexInstrument(
                    new Symbol("AUDUSD", new Venue("FXCM")),
                    new BrokerSymbol("AUD/USD"),
                    5,
                    0,
                    0,
                    0,
                    0,
                    0,
                    Price.Create(0.00001m, 5),
                    Quantity.Create(1000m),
                    Quantity.Create(1m),
                    Quantity.Create(50000000m),
                    1,
                    1,
                    StubZonedDateTime.UnixEpoch());

            return instrument;
        }

        public static ForexInstrument EURUSD()
        {
            var instrument = new ForexInstrument(
                    new Symbol("EURUSD", new Venue("FXCM")),
                    new BrokerSymbol("EUR/USD"),
                    5,
                    0,
                    0,
                    0,
                    0,
                    0,
                    Price.Create(0.00001m, 5),
                    Quantity.Create(1000m),
                    Quantity.Create(1m),
                    Quantity.Create(50000000m),
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
                    0,
                    0,
                    0,
                    0,
                    0,
                    Price.Create(0.001m, 3),
                    Quantity.Create(1000m),
                    Quantity.Create(1m),
                    Quantity.Create(50000000m),
                    1,
                    1,
                    StubZonedDateTime.UnixEpoch());

            return instrument;
        }
    }
}
