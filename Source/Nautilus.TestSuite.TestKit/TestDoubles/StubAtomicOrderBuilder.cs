//--------------------------------------------------------------------------------------------------
// <copyright file="StubAtomicOrderBuilder.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubAtomicOrderBuilder
    {
        public static AtomicOrder Build()
        {
            return new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().WithOrderId("EntryOrderId").BuildStopMarketOrder(),
                new StubOrderBuilder().WithOrderId("StopLossOrderId").BuildStopMarketOrder(),
                new StubOrderBuilder().WithOrderId("ProfitTargetOrderId").BuildStopMarketOrder());
        }
    }
}
