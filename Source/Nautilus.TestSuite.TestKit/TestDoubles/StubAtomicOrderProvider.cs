//--------------------------------------------------------------------------------------------------
// <copyright file="StubAtomicOrderProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Entities;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubAtomicOrderProvider
    {
        public static AtomicOrder Create(bool hasTakeProfit = true)
        {
            if (hasTakeProfit)
            {
                return new AtomicOrder(
                    new StubOrderBuilder().WithOrderId("O-123456-1").BuildStopMarketOrder(),
                    new StubOrderBuilder().WithOrderId("O-123456-2").BuildStopMarketOrder(),
                    new StubOrderBuilder().WithOrderId("O-123456-3").BuildLimitOrder());
            }

            return new AtomicOrder(
                new StubOrderBuilder().WithOrderId("O-123456-1").BuildStopMarketOrder(),
                new StubOrderBuilder().WithOrderId("O-123456-2").BuildStopMarketOrder());
        }
    }
}
