//--------------------------------------------------------------------------------------------------
// <copyright file="StubBarStoreFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.BlackBox.AlphaModel.Strategy;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubBarStoreFactory
    {
        public static BarStore Create()
        {
            var barStore = new BarStore(
                new Symbol("AUDUSD", Venue.FXCM),
                new BarSpecification(QuoteType.Bid, Resolution.Minute, 5));

            foreach (var bar in StubBarBuilder.BuildList())
            {
                barStore.Update(bar);
            }

            return barStore;
        }
    }
}
