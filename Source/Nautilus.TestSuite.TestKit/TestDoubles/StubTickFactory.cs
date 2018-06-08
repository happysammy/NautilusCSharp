//--------------------------------------------------------------------------------------------------
// <copyright file="StubTickFactory.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.ValueObjects;

    /// <summary>
    /// The stub tick factory.
    /// </summary>
    public static class StubTickFactory
    {
        private static readonly IList<Bar> StubBarList = StubBarBuilder.BuildList();
        private static readonly decimal LastAsk = StubBarList[StubBarList.Count - 1].Close + 0.00001m;
        private static readonly decimal LastBid = StubBarList[StubBarList.Count - 1].Close.Value;

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        /// <returns>
        /// The <see cref="Tick"/>.
        /// </returns>
        public static Tick Create(Symbol symbol)
        {
            return new Tick(
                symbol,
                Price.Create(LastBid, 0.00001m),
                Price.Create(LastAsk, 0.00001m),
                StubDateTime.Now());
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        /// <param name="bid">
        /// The bid.
        /// </param>
        /// <param name="ask">
        /// The ask.
        /// </param>
        /// <returns>
        /// The <see cref="Tick"/>.
        /// </returns>
        public static Tick Create(
            Symbol symbol,
            Price bid,
            Price ask)
        {
            return new Tick(
                symbol,
                bid,
                ask,
                StubDateTime.Now());
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
                StubDateTime.Now());
        }
    }
}
