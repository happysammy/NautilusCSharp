// -------------------------------------------------------------------------------------------------
// <copyright file="StubSetupContainer.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Nautilus.TestSuite.TestKit.TestDoubles
{
    using Nautilus.Common.Interfaces;

    public class StubSetupContainer : IComponentryContainer
    {
        public StubSetupContainer(
            IZonedClock clock,
            IGuidFactory guidFactory,
            ILoggerFactory loggerFactory)
        {
            this.Clock = clock;
            this.GuidFactory = guidFactory;
            this.LoggerFactory = loggerFactory;
        }

        public IZonedClock Clock { get; }
        public IGuidFactory GuidFactory { get; }
        public ILoggerFactory LoggerFactory { get; }
    }
}
