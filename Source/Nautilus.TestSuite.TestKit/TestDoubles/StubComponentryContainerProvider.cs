//--------------------------------------------------------------------------------------------------
// <copyright file="StubComponentryContainerProvider.cs" company="Nautech Systems Pty Ltd">
//  Copyright (C) 2015-2020 Nautech Systems Pty Ltd. All rights reserved.
//  The use of this source code is governed by the license as found in the LICENSE.txt file.
//  https://nautechsystems.io
// </copyright>
//--------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Extensions.Logging;
    using Nautilus.Common.Componentry;
    using Nautilus.Common.Interfaces;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Test Suite")]
    public sealed class StubComponentryContainerProvider
    {
        public StubComponentryContainerProvider()
        {
            this.Clock = new StubClock();
            this.Clock.FreezeSetTime(StubZonedDateTime.UnixEpoch());
            this.Logger = new MockLogger();
        }

        public StubClock Clock { get; }

        public MockLogger Logger { get; }

        public IComponentryContainer Create()
        {
            return new ComponentryContainer(
                this.Clock,
                new GuidFactory(),
                new LoggerFactory());
        }
    }
}
