//--------------------------------------------------------------------------------------------------
// <copyright file="StubBarType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.Stubs
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public static class StubBarType
    {
        public static BarType AUDUSD_OneMinuteAsk()
        {
            return new BarType(
                new Symbol("AUDUSD", new Venue("FXCM")),
                new BarSpecification(
                    1,
                    BarStructure.Minute,
                    PriceType.Ask));
        }

        public static BarType GBPUSD_OneMinuteBid()
        {
            return new BarType(
                new Symbol("GBPUSD", new Venue("FXCM")),
                new BarSpecification(
                    1,
                    BarStructure.Minute,
                    PriceType.Bid));
        }

        public static BarType GBPUSD_OneSecondMid()
        {
            return new BarType(
                new Symbol("GBPUSD", new Venue("FXCM")),
                new BarSpecification(
                    1,
                    BarStructure.Second,
                    PriceType.Mid));
        }

        public static BarType USDJPY_OneMinuteBid()
        {
            return new BarType(
                new Symbol("USDJPY", new Venue("FXCM")),
                new BarSpecification(
                    1,
                    BarStructure.Minute,
                    PriceType.Bid));
        }

        public static BarType CADHKD_OneMinuteBid()
        {
            return new BarType(
                new Symbol("CADHKD", new Venue("FXCM")),
                new BarSpecification(
                    1,
                    BarStructure.Minute,
                    PriceType.Bid));
        }
    }
}
