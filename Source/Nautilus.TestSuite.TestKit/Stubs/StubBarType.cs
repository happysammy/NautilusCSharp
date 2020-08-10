//--------------------------------------------------------------------------------------------------
// <copyright file="StubBarType.cs" company="Nautech Systems Pty Ltd">
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
using Nautilus.DomainModel.Enums;
using Nautilus.DomainModel.Identifiers;
using Nautilus.DomainModel.ValueObjects;

namespace Nautilus.TestSuite.TestKit.Stubs
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public static class StubBarType
    {
        public static BarType AUDUSD_OneMinuteBid()
        {
            return new BarType(
                new Symbol("AUD/USD", new Venue("FXCM")),
                new BarSpecification(
                    1,
                    BarStructure.Minute,
                    PriceType.Bid));
        }

        public static BarType AUDUSD_OneMinuteAsk()
        {
            return new BarType(
                new Symbol("AUD/USD", new Venue("FXCM")),
                new BarSpecification(
                    1,
                    BarStructure.Minute,
                    PriceType.Ask));
        }

        public static BarType AUDUSD_OneMinuteMid()
        {
            return new BarType(
                new Symbol("AUD/USD", new Venue("FXCM")),
                new BarSpecification(
                    1,
                    BarStructure.Minute,
                    PriceType.Mid));
        }

        public static BarType GBPUSD_OneMinuteBid()
        {
            return new BarType(
                new Symbol("GBP/USD", new Venue("FXCM")),
                new BarSpecification(
                    1,
                    BarStructure.Minute,
                    PriceType.Bid));
        }

        public static BarType GBPUSD_OneSecondMid()
        {
            return new BarType(
                new Symbol("GBP/USD", new Venue("FXCM")),
                new BarSpecification(
                    1,
                    BarStructure.Second,
                    PriceType.Mid));
        }

        public static BarType USDJPY_OneMinuteBid()
        {
            return new BarType(
                new Symbol("USD/JPY", new Venue("FXCM")),
                new BarSpecification(
                    1,
                    BarStructure.Minute,
                    PriceType.Bid));
        }

        public static BarType CADHKD_OneMinuteBid()
        {
            return new BarType(
                new Symbol("CAD/HKD", new Venue("FXCM")),
                new BarSpecification(
                    1,
                    BarStructure.Minute,
                    PriceType.Bid));
        }
    }
}
