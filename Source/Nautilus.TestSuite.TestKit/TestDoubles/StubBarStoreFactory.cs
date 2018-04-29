//--------------------------------------------------------------------------------------------------
// <copyright file="StubBarStoreFactory.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using Nautilus.BlackBox.AlphaModel.Strategy;
    using Nautilus.DomainModel.Enums;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The stub bar factory.
    /// </summary>
    public static class StubBarStoreFactory
    {
        /// <summary>
        /// The get bar store.
        /// </summary>
        /// <returns>
        /// The <see cref="BarStore"/>.
        /// </returns>
        public static BarStore Create()
        {
            var barStore = new BarStore(
                new Symbol("AUDUSD", Exchange.FXCM),
                new BarSpecification(BarQuoteType.Bid, BarResolution.Minute, 5));

            foreach (var bar in StubBarBuilder.BuildList())
            {
                barStore.Update(bar);
            }

            return barStore;
        }
    }
}
