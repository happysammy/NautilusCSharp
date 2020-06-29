//--------------------------------------------------------------------------------------------------
// <copyright file="StubBarProvider.cs" company="Nautech Systems Pty Ltd">
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

namespace Nautilus.TestSuite.TestKit.Stubs
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public static class StubBarProvider
    {
        public static Bar Build()
        {
            return new Bar(
                Price.Create(0.80000m),
                Price.Create(0.80025m),
                Price.Create(0.79980m),
                Price.Create(0.80008m),
                Volume.Create(1000),
                StubZonedDateTime.UnixEpoch());
        }

        public static Bar BuildWithTimestamp(ZonedDateTime timestamp)
        {
            return new Bar(
                Price.Create(1.00000m),
                Price.Create(1.00000m),
                Price.Create(1.00000m),
                Price.Create(1.00000m),
                Volume.Create(1000),
                timestamp);
        }

        public static IList<Bar> BuildList()
        {
            return new List<Bar>
            {
                new Bar(Price.Create(0.80000m), Price.Create(0.80010m), Price.Create(0.80000m), Price.Create(0.80008m), Volume.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(45).ToDuration()),
                new Bar(Price.Create(0.80008m), Price.Create(0.80020m), Price.Create(0.80005m), Price.Create(0.80015m), Volume.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(40).ToDuration()),
                new Bar(Price.Create(0.80015m), Price.Create(0.80030m), Price.Create(0.80010m), Price.Create(0.80020m), Volume.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(35).ToDuration()),
                new Bar(Price.Create(0.80020m), Price.Create(0.80030m), Price.Create(0.80000m), Price.Create(0.80010m), Volume.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(30).ToDuration()),
                new Bar(Price.Create(0.80010m), Price.Create(0.80015m), Price.Create(0.79990m), Price.Create(0.79995m), Volume.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(25).ToDuration()),
                new Bar(Price.Create(0.79995m), Price.Create(0.80000m), Price.Create(0.79980m), Price.Create(0.79985m), Volume.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(20).ToDuration()),
                new Bar(Price.Create(0.80000m), Price.Create(0.80010m), Price.Create(0.80000m), Price.Create(0.80008m), Volume.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(15).ToDuration()),
                new Bar(Price.Create(0.80000m), Price.Create(0.80010m), Price.Create(0.80000m), Price.Create(0.80008m), Volume.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(10).ToDuration()),
                new Bar(Price.Create(0.80000m), Price.Create(0.80010m), Price.Create(0.80000m), Price.Create(0.80008m), Volume.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(05).ToDuration()),
                new Bar(Price.Create(0.80000m), Price.Create(0.80015m), Price.Create(0.79990m), Price.Create(0.80005m), Volume.Create(1000), StubZonedDateTime.UnixEpoch()),
            };
        }
    }
}
