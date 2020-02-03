//--------------------------------------------------------------------------------------------------
// <copyright file="StubTickProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
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
    using NodaTime;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubTickProvider
    {
        private static readonly IList<Bar> StubBarList = StubBarProvider.BuildList();
        private static readonly decimal LastAsk = StubBarList[^1].Close + 0.00001m;
        private static readonly decimal LastBid = StubBarList[^1].Close.Value;

        public static Tick Create(Symbol symbol)
        {
            return new Tick(
                symbol,
                Price.Create(LastBid),
                Price.Create(LastAsk),
                Volume.One(),
                Volume.One(),
                StubZonedDateTime.UnixEpoch());
        }

        public static Tick Create(Symbol symbol, ZonedDateTime timestamp)
        {
            return new Tick(
                symbol,
                Price.Create(LastBid),
                Price.Create(LastAsk),
                Volume.One(),
                Volume.One(),
                timestamp);
        }
    }
}
