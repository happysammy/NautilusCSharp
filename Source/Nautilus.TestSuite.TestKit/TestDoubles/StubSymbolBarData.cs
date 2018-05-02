﻿// -------------------------------------------------------------------------------------------------
// <copyright file="StubBarSpecification.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using Nautilus.Database.Core.Types;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    public static class StubSymbolBarData
    {
        public static SymbolBarSpec AUDUSD()
        {
            return new SymbolBarSpec(
                new Symbol("AUDUSD", Exchange.Dukascopy),
                new BarSpecification(
                    BarQuoteType.Ask,
                    BarResolution.Minute,
                    1));
        }

        public static SymbolBarSpec GBPUSD()
        {
            return new SymbolBarSpec(
                new Symbol("GBPUSD", Exchange.Dukascopy),
                new BarSpecification(
                    BarQuoteType.Bid,
                    BarResolution.Minute,
                    1));
        }

        public static SymbolBarSpec USDJPY()
        {
            return new SymbolBarSpec(
                new Symbol("USDJPY", Exchange.Dukascopy),
                new BarSpecification(
                    BarQuoteType.Bid,
                    BarResolution.Minute,
                    1));
        }

        public static SymbolBarSpec CADHKD()
        {
            return new SymbolBarSpec(
                new Symbol("CADHKD", Exchange.Dukascopy),
                new BarSpecification(
                    BarQuoteType.Bid,
                    BarResolution.Minute,
                    1));
        }
    }
}
