//--------------------------------------------------------------------------------------------------
// <copyright file="StubClock.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Interfaces;
    using NodaTime;

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "*", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public sealed class StubClock : IZonedClock
    {
        private bool isTimeFrozen;
        private ZonedDateTime frozenTime;

        public ZonedDateTime TimeNow()
        {
            if (this.isTimeFrozen)
            {
                return this.frozenTime;
            }

            return SystemClock.Instance.GetCurrentInstant().InUtc();
        }

        public Instant InstantNow()
        {
            if (this.isTimeFrozen)
            {
                return this.frozenTime.ToInstant();
            }

            return SystemClock.Instance.GetCurrentInstant();
        }

        public void UnfreezeTime()
        {
            this.isTimeFrozen = false;
        }

        public void FreezeTimeNow()
        {
            this.isTimeFrozen = true;
            this.frozenTime = this.TimeNow();
        }

        public void FreezeSetTime(ZonedDateTime dateTime)
        {
            this.isTimeFrozen = true;
            this.frozenTime = dateTime;
        }

        public DateTimeZone GetTimeZone()
        {
            return this.frozenTime.Zone;
        }
    }
}
