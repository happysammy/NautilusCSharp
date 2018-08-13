//--------------------------------------------------------------------------------------------------
// <copyright file="StubTickFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubTickFactory
    {
        private static readonly IList<Bar> StubBarList = StubBarBuilder.BuildList();
        private static readonly decimal LastAsk = StubBarList[StubBarList.Count - 1].Close + 0.00001m;
        private static readonly decimal LastBid = StubBarList[StubBarList.Count - 1].Close.Value;

        public static Tick Create(Symbol symbol)
        {
            return new Tick(
                symbol,
                Price.Create(LastBid, 0.00001m),
                Price.Create(LastAsk, 0.00001m),
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
                Price.Create(bid, 0.00001m),
                Price.Create(ask, 0.00001m),
                StubZonedDateTime.UnixEpoch());
        }
    }
}
