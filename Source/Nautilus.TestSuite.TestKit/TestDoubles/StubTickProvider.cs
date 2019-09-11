//--------------------------------------------------------------------------------------------------
// <copyright file="StubTickProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.Identifiers;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubTickProvider
    {
        private static readonly IList<Bar> StubBarList = StubBarBuilder.BuildList();
        private static readonly decimal LastAsk = StubBarList[StubBarList.Count - 1].Close + 0.00001m;
        private static readonly decimal LastBid = StubBarList[StubBarList.Count - 1].Close.Value;

        public static Tick Create(Symbol symbol)
        {
            return new Tick(
                symbol,
                Price.Create(LastBid, 5),
                Price.Create(LastAsk, 5),
                StubZonedDateTime.UnixEpoch());
        }

        public static Tick Create(
            Symbol symbol,
            Price bid,
            Price ask)
        {
            return new Tick(
                symbol,
                bid,
                ask,
                StubZonedDateTime.UnixEpoch());
        }

        public static Tick Create(
            Symbol symbol,
            decimal bid,
            decimal ask)
        {
            return new Tick(
                symbol,
                Price.Create(bid, 5),
                Price.Create(ask, 5),
                StubZonedDateTime.UnixEpoch());
        }
    }
}
