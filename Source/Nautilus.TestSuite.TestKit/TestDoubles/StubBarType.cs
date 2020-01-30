//--------------------------------------------------------------------------------------------------
// <copyright file="StubBarType.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubBarType
    {
        public static BarType AUDUSD()
        {
            return new BarType(
                new Symbol("AUDUSD", new Venue("FXCM")),
                new BarSpecification(
                    1,
                    BarStructure.Minute,
                    PriceType.Ask));
        }

        public static BarType GBPUSD()
        {
            return new BarType(
                new Symbol("GBPUSD", new Venue("FXCM")),
                new BarSpecification(
                    1,
                    BarStructure.Minute,
                    PriceType.Bid));
        }

        public static BarType GBPUSD_Second()
        {
            return new BarType(
                new Symbol("GBPUSD", new Venue("FXCM")),
                new BarSpecification(
                    1,
                    BarStructure.Second,
                    PriceType.Mid));
        }

        public static BarType USDJPY()
        {
            return new BarType(
                new Symbol("USDJPY", new Venue("FXCM")),
                new BarSpecification(
                    1,
                    BarStructure.Minute,
                    PriceType.Bid));
        }

        public static BarType CADHKD()
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
