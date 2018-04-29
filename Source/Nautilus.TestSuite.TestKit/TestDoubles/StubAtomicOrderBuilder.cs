//--------------------------------------------------------------------------------------------------
// <copyright file="StubAtomicOrderBuilder.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Entities;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubAtomicOrderBuilder
    {
        public static AtomicOrder Build()
        {
            return new AtomicOrder(
                new TradeType("TestTrade"),
                new StubOrderBuilder().WithOrderId("EntryOrderId").BuildStopMarket(),
                new StubOrderBuilder().WithOrderId("StoplossOrderId").BuildStopMarket(),
                new StubOrderBuilder().WithOrderId("ProfitTargetOrderId").BuildStopMarket());
        }
    }
}