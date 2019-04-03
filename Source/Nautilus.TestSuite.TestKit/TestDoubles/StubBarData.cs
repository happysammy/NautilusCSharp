//--------------------------------------------------------------------------------------------------
// <copyright file="StubBarData.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.DomainModel.ValueObjects;
    using NodaTime;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public static class StubBarData
    {
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
