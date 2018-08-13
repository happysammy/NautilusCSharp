//--------------------------------------------------------------------------------------------------
// <copyright file="StubTradeProfileFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "*",
        Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubTradeProfileFactory
    {
        public static TradeProfile Create(int tradePeriod)
        {
            return new TradeProfile(
                new TradeType("TestTrade"),
                new BarSpecification(
                    QuoteType.Bid,
                    Resolution.Minute,
                    5),
                tradePeriod,
                2,
                1000,
                180,
                30,
                2,
                0,
                1,
                StubZonedDateTime.UnixEpoch());
        }
    }
}
