//--------------------------------------------------------------------------------------------------
// <copyright file="StubBarBuilder.cs" company="Nautech Systems Pty Ltd">
//   Copyright (C) 2015-2017 Nautech Systems Pty Ltd. All rights reserved.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Collections.Generic;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// The stub bar factory.
    /// </summary>
    public static class StubBarBuilder
    {
        /// <summary>
        /// The build.
        /// </summary>
        /// <returns>
        /// The <see cref="Bar"/>.
        /// </returns>
        public static Bar Build()
        {
            return new Bar(
                Price.Create(0.80000m, 0.00001m),
                Price.Create(0.80025m, 0.00001m),
                Price.Create(0.79980m, 0.00001m),
                Price.Create(0.80008m, 0.00001m),
                Quantity.Create(1000),
                StubZonedDateTime.UnixEpoch());
        }

        /// <summary>
        /// The get bar list.
        /// </summary>
        /// <returns>
        /// The list.
        /// </returns>
        public static IList<Bar> BuildList()
        {
            return new List<Bar>
            {
                new Bar(Price.Create(0.80000m, 0.00001m), Price.Create(0.80010m, 0.00001m), Price.Create(0.80000m, 0.00001m), Price.Create(0.80008m, 0.00001m), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(45).ToDuration()),
                new Bar(Price.Create(0.80008m, 0.00001m), Price.Create(0.80020m, 0.00001m), Price.Create(0.80005m, 0.00001m), Price.Create(0.80015m, 0.00001m), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(40).ToDuration()),
                new Bar(Price.Create(0.80015m, 0.00001m), Price.Create(0.80030m, 0.00001m), Price.Create(0.80010m, 0.00001m), Price.Create(0.80020m, 0.00001m), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(35).ToDuration()),
                new Bar(Price.Create(0.80020m, 0.00001m), Price.Create(0.80030m, 0.00001m), Price.Create(0.80000m, 0.00001m), Price.Create(0.80010m, 0.00001m), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(30).ToDuration()),
                new Bar(Price.Create(0.80010m, 0.00001m), Price.Create(0.80015m, 0.00001m), Price.Create(0.79990m, 0.00001m), Price.Create(0.79995m, 0.00001m), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(25).ToDuration()),
                new Bar(Price.Create(0.79995m, 0.00001m), Price.Create(0.80000m, 0.00001m), Price.Create(0.79980m, 0.00001m), Price.Create(0.79985m, 0.00001m), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(20).ToDuration()),
                new Bar(Price.Create(0.80000m, 0.00001m), Price.Create(0.80010m, 0.00001m), Price.Create(0.80000m, 0.00001m), Price.Create(0.80008m, 0.00001m), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(15).ToDuration()),
                new Bar(Price.Create(0.80000m, 0.00001m), Price.Create(0.80010m, 0.00001m), Price.Create(0.80000m, 0.00001m), Price.Create(0.80008m, 0.00001m), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(10).ToDuration()),
                new Bar(Price.Create(0.80000m, 0.00001m), Price.Create(0.80010m, 0.00001m), Price.Create(0.80000m, 0.00001m), Price.Create(0.80008m, 0.00001m), Quantity.Create(1000), StubZonedDateTime.UnixEpoch() - Period.FromMinutes(05).ToDuration()),
                new Bar(Price.Create(0.80000m, 0.00001m), Price.Create(0.80015m, 0.00001m), Price.Create(0.79990m, 0.00001m), Price.Create(0.80005m, 0.00001m), Quantity.Create(1000), StubZonedDateTime.UnixEpoch())
            };
        }
    }
}
