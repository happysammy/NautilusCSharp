//--------------------------------------------------------------------------------------------------
// <copyright file="StubComponentryContainerFactory.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2019 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;
    using Nautilus.Common.Logging;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK within the Test Suite.")]
    public class StubComponentryContainerFactory
    {
        public StubComponentryContainerFactory()
        {
            this.Clock = new StubClock();
            this.Clock.FreezeSetTime(StubZonedDateTime.UnixEpoch());
            this.GuidFactory = new GuidFactory();
            this.LoggingAdapter = new MockLoggingAdapter();
            this.LoggerFactory = new LoggerFactory(this.LoggingAdapter);
        }

        public StubClock Clock { get; }

        public IGuidFactory GuidFactory { get; }

        public MockLoggingAdapter LoggingAdapter { get; }

        public ILoggerFactory LoggerFactory { get; }

        public IComponentryContainer Create()
        {
            return new ComponentryContainer(
                this.Clock,
                this.GuidFactory,
                this.LoggerFactory);
        }
    }
}
