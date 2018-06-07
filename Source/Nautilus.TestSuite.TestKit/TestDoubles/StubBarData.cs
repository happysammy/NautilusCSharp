// -------------------------------------------------------------------------------------------------
// <copyright file="StubBarData.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    /// <summary>
    /// Provides stub <see cref="Bar"/> objects for the test suite.
    /// </summary>
    public static class StubBarData
    {
        /// <summary>
        /// Returns a stub <see cref="Bar"/> with timestamp optionally offset by the given
        /// minutes.
        /// </summary>
        /// <param name="offsetMinutes">The timestamp offset minutes.</param>
        /// <returns>A <see cref="Bar"/>.</returns>
        public static Bar Create(int offsetMinutes = 0)
        {
            return new Bar(
                1.00000m,
                1.00000m,
                1.00000m,
                1.00000m,
                1000000,
                StubZonedDateTime.UnixEpoch() + Duration.FromMinutes(offsetMinutes));
        }

        /// <summary>
        /// Returns a stub <see cref="Bar"/> with timestamp optionally offset by the given
        /// <see cref="Duration"/>.
        /// </summary>
        /// <param name="offset">The timestamp offset.</param>
        /// <returns>A <see cref="Bar"/>.</returns>
        public static Bar Create(Duration offset)
        {
            return new Bar(
                1.00000m,
                1.00000m,
                1.00000m,
                1.00000m,
                1000000,
                StubZonedDateTime.UnixEpoch() + offset);
        }
    }
}
